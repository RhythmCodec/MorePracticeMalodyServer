/* Copyright (C) 2021 RhythmCodec
 * See @RhythmCodec at https://github.com/RhythmCodec
 * 
 * Last Update: 2021/08/22
 * Author: Kami11
 * Last Modifier: kami11
 * Description:
 *      
 *      Map Store Controller of MorePractice Malody Server. 
 *      Make sure turning off the DEBUG mode.
 *      Just provide a valid sid and cid, and the server will 
 *      return the charts.
 * 
 * Providing datas:
 *      
 *      Promoted charts, sid/cid searching, song charts
 *
 * Known bugs: 
 * 
 * 
 */

using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MorePracticeMalodyServer.Data;
using MorePracticeMalodyServer.Data.Helper;
using MorePracticeMalodyServer.Model;
using MorePracticeMalodyServer.Model.DataModel;
using MorePracticeMalodyServer.Model.DbModel;
using EventInfo = MorePracticeMalodyServer.Model.DataModel.EventInfo;

namespace MorePracticeMalodyServer.Controllers;

/// <summary>
///     Map Store Controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class StoreController : ControllerBase
{
    private readonly IMemoryCache cache;
    private readonly DataContext context;
    private readonly ILogger<StoreController> logger;

    public StoreController(ILogger<StoreController> logger, DataContext context, IMemoryCache cache)
    {
        this.logger = logger;
        this.context = context;
        this.cache = cache;
    }

    /// <summary>
    ///     Provide a list of charts under the specified query conditions.
    /// </summary>
    /// <param name="uid">int: User id</param>
    /// <param name="api">int: Api version</param>
    /// <param name="word">string: Search keyword</param>
    /// <param name="org">int: Whether to return the original title</param>
    /// <param name="mode">int: Returns the chart of the specified mode</param>
    /// <param name="lvge">int: Returns the chart whose level is greater than this value</param>
    /// <param name="lvle">int: Returns the chart whose level is less than this value</param>
    /// <param name="beta">int: Return to non-stable chart</param>
    /// <param name="from">int: Paging start</param>
    /// <returns>A warped json list of song infos.</returns>
    [Route("list")]
    [HttpGet]
    public async Task<Response<SongInfo>> GetList(int uid, string word, int org, int mode, int lvge,
        int lvle, int beta, int from)
    {
        var resp = new Response<SongInfo>();

        // TODO: 缓存歌曲列表。
        // TODO: 过滤搜索关键字。可能使用某些字符标明全局歌曲列表缓存。
        if (string.IsNullOrWhiteSpace(word)) // If word is null, Query all songs.
        {
            var total = context.Songs.Count();
            if (total - from * Consts.MaxItem > Consts.MaxItem)
            {
                resp.HasMore = true;
                resp.Next = from + 1;
            }
            else
            {
                resp.HasMore = false;
            }

            IEnumerable<SongInfo> result;
            try
            {
                result = await context.QuerySong(word, org, mode, lvge, lvle, beta, from);
            }
            catch (Exception e)
            {
                logger.LogError("Error occurs when query songs");
#if DEBUG
                throw;
#else
                    resp.Code = -1;
                    resp.HasMore = false;
                    return resp; // Return a empty list with error code -1.
#endif
            }

            resp.Code = 0;
            // Add song to return value.
            resp.Data.AddRange(result);

            return resp;
        }

        // Try to find data from memory cache first.
        // Here is not the best way to cache song list. I will optimize this later.
        // Or you can optimize this too.
        var keyword = Util.TrimSpecial(word).ToLower();
        if (cache.TryGetValue(keyword, out IEnumerable<SongInfo> cachedSongList))
        {
        }
        else // Query db and write result to cache
        {
            // Query all song match our meet.
            IEnumerable<SongInfo> result;
            try
            {
                result = await context.QueryAllSong(word, org, mode, lvge, lvle, beta, from);
            }
            catch (Exception e)
            {
                logger.LogError("Error occurs when query songs");
#if DEBUG
                throw;
#else
                    resp.Code = -1;
                    resp.HasMore = false;
                    return resp; // Return a empty list with error code -1.
#endif
            }

            // Create new cache entry. Set value abd expiration.
            var cacheEntry = cache.CreateEntry(keyword);
            cacheEntry.Value = result;
            cacheEntry.AbsoluteExpirationRelativeToNow = new TimeSpan(0, 2, 0);

            cachedSongList = result;
        }

        resp.Code = 0;

        // If has more than MaxItem left, set HasMore to true, and set next index.
        if (cachedSongList.Count() - from * Consts.MaxItem > Consts.MaxItem)
        {
            resp.HasMore = true;
            resp.Next = from + 1;
        }
        else
        {
            resp.HasMore = false;
        }

        // Init SongInfo and write data to it.
        // We select all songs to cache, so we should take correct number of song info.
        resp.Data.AddRange(cachedSongList.Skip(from * Consts.MaxItem).Take(Consts.MaxItem));

        return resp;
    }

    /// <summary>
    ///     Provide a list of promoted song.
    /// </summary>
    /// <param name="uid">int: User id</param>
    /// <param name="api">int: Api version</param>
    /// <param name="org">int: Whether to return the original title</param>
    /// <param name="mode">int: Returns the chart of the specified mode</param>
    /// <param name="from">int: Paging start</param>
    /// <returns>A warped json list of song infos.</returns>
    [Route("promote")]
    [HttpGet]
    public async Task<Response<SongInfo>> GetPromote(int uid, int org, int mode, int from)
    {
        var resp = new Response<SongInfo>();

        // Query promotes from database.
        IEnumerable<Song> result; // TODO: Cache result?
        try
        {
            result = await context.QueryAllPromotion();
        }
        catch (Exception e)
        {
            logger.LogError("Error occurs when query promotes.");
#if DEBUG
            throw;
#else
                    resp.Code = -1;
                    resp.HasMore = false;
                    return resp; // Return a empty list with error code -1.
#endif
        }

        resp.Code = 0;
        // To see if has more.
        if (result.Count() - Consts.MaxItem * from > Consts.MaxItem)
        {
            resp.HasMore = true;
            resp.Next = from + 1;
        }
        else
        {
            resp.HasMore = false;
        }

        // Insert to Data
        var songs = result as List<Song>;
        for (var i = from * Consts.MaxItem;
             resp.HasMore ? i != from * Consts.MaxItem + Consts.MaxItem : i != result.Count();
             i++)
        {
            var song = songs[i];
            resp.Data.Add(new SongInfo
            {
                Artist = org == 0 ? song.Artist : song.OriginalArtist,
                Bpm = song.Bpm,
                Cover = song.Cover,
                Length = song.Length,
                Mode = song.Mode,
                Sid = song.SongId,
                Time = Util.GetTimeStamp(song.Time),
                Title = org == 0 ? song.Title : song.OriginalTitle
            });
        }

        return resp;
    }

    /// <summary>
    ///     Provide all charts of a song.
    /// </summary>
    /// <param name="uid">int:User id</param>
    /// <param name="api">int: Api version</param>
    /// <param name="sid">int: Song id</param>
    /// <param name="beta">int: Whether to return unstable charts</param>
    /// <param name="mode">int: Return given chart Mode</param>
    /// <param name="from">int: Paging Start</param>
    /// <param name="promote">int: If song comes from promote list</param>
    /// <returns name="resp">Response a chart info</returns>
    [Route("charts")]
    [HttpGet]
    public async Task<Response<ChartInfo>> GetChart(int uid, int sid, int beta, int mode, int from,
        int promote)
    {
        var resp = new Response<ChartInfo>();

        // Try to find song with correct id.
        List<ChartInfo> result;
        try
        {
            // 目前返回所有的chart
            result = await context.QueryChartsFromSid(sid, beta, mode) as List<ChartInfo>;
        }
        catch (InvalidOperationException) // Song doesn't exist.
        {
            logger.LogError("Could not find Song with Id {id}", sid);
#if DEBUG
            throw;
#else
                return resp;
#endif
        }

        // If has more than MaxItem left, set HasMore to true, and set next index.
        if (result.Count - from * Consts.MaxItem > Consts.MaxItem)
        {
            //resp.HasMore = true;
            resp.HasMore = false;
            resp.Next = from + 1;
        }
        else
        {
            resp.HasMore = false;
        }

        // Add charts to resp.
        resp.Data.AddRange(result);

        return resp;
    }

    /// <summary>
    ///     Main method of seaching charts using given sid and cid.
    /// </summary>
    /// <param name="uid">int: User id</param>
    /// <param name="api">int: Api version</param>
    /// <param name="sid">int: Song id</param>
    /// <param name="cid">int: Chart id</param>
    /// <param name="org">int: Whether to return original title</param>
    /// <returns name="resp">Response a song info</returns>
    [Route("query")]
    [HttpGet]
    public async Task<Response<SongInfo>> QuerySong(int uid, int sid = -1, int cid = -1, int org = 0)
    {
        // If not providing a valid SID or CID, throw a exception.
        if (sid == -1 && cid == -1)
        {
            logger.LogError("Query for sid {sid} or cid {cid} is not valid!", sid, cid);
            throw new ArgumentException("A valid SID or CID not provided.");
        }

        var resp = new Response<SongInfo>();

        if (sid != -1) // Use sid to select song
            try
            {
                var result = await context.QuerySongWithId(sid, org);

                // Success.
                resp.Code = 0;
                resp.Data.Add(result);

                return resp;
            }
            catch (InvalidOperationException e) // Song doesn't exist.
            {
                logger.LogInformation("Could not find a song with sid {sid}", sid);
#if DEBUG
                throw;
#else
                    return resp;
#endif
            }

        if (cid != -1) // Use chart ID.
            try
            {
                var result = await context.QuerySongWithCid(cid, org);

                // Success
                resp.Code = 0;

                resp.Data.Add(result);

                return resp;
            }
            catch (InvalidOperationException e) // None Charts found.
            {
                logger.LogInformation("Could not find a song with cid {cid}", cid);
#if DEBUG
                throw;
#else
                    return resp;
#endif
            }
#if DEBUG
        logger.LogCritical("WTF goes here? Check QuerySong() for some detail.");
        return resp;
#else
            return resp;
#endif
    }

    /// <summary>
    ///     Provide download link of a specified charts.
    /// </summary>
    /// <param name="uid">int: User id</param>
    /// <param name="api">int: Api version</param>
    /// <param name="cid">int: Chart id</param>
    /// <returns name="resp">Response a download link</returns>
    [Route("download")]
    [HttpGet]
    public async Task<DownloadResponse> GetDownload(int uid, int cid)
    {
        var resp = new DownloadResponse();
        Chart chart = null;

        // Try to find the chart with cid given.
        try
        {
            chart = await context.Charts
                .Include(c => c.Downloads)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstAsync(c => c.ChartId == cid);

            var dls = chart.Downloads;

            if (dls.Any())
            {
                resp.Code = 0;
                resp.Sid = chart.Song.SongId;
                resp.Cid = chart.ChartId;
                foreach (var dl in dls)
                    resp.Items.Add(new DownloadInfo
                    {
                        File = dl.File,
                        Hash = dl.Hash,
                        Name = dl.Name
                    });
            }

            return resp;
        }
        catch (InvalidOperationException) // No chart found.
        {
            logger.LogWarning("No chart with cid {cid} find!", cid);
            // Set response code to -2, indicates no chart found.
            resp.Code = -2;
            return resp;
        }
    }

    /// <summary>
    ///     Provide all events.
    /// </summary>
    /// <param name="uid">int: User id</param>
    /// <param name="api">int: Api version</param>
    /// <param name="active">int: Return ongoing events</param>
    /// <param name="from">int: Paging start</param>
    /// <returns name="resp">return a EventInfo Response</returns>
    [Route("events")]
    [HttpGet]
    public async Task<Response<EventInfo>> GetEvents(int uid, int active, int from)
    {
        var resp = new Response<EventInfo>();

        try
        {
            // Success.
            var result = await context.QueryEvents(active, from) as List<Event>; // TODO: Save events to cache?

            resp.Code = 0;
            // To see if has more to send.
            if (context.Events.Count() - Consts.MaxItem * from > Consts.MaxItem)
            {
                resp.HasMore = true;
                resp.Next = from + 1;
            }
            else
            {
                resp.HasMore = false;
            }

            // Write to response.Data
            foreach (var @event in result)
                resp.Data.Add(new EventInfo
                {
                    Active = @event.Active,
                    Cover = @event.Cover,
                    Eid = @event.EventId,
                    End = @event.End.ToString("yyyy-MM-dd"),
                    Name = @event.Name,
                    Start = @event.Start.ToString("yyyy-MM-dd"),
                    Sponsor = @event.Sponsor
                });

            return resp;
        }
        catch (ArgumentNullException) // Something impossible happened.
        {
            logger.LogError("¿");
            throw;
            // I DONT WANT TO WRITE HERE ANY MORE.
        }
    }

    /// <summary>
    ///     Provide all charts under a specific event.
    /// </summary>
    /// <param name="uid">int: User id</param>
    /// <param name="api">int: Api version</param>
    /// <param name="eid">int: Event id</param>
    /// <param name="org">int: Whether to return the original title</param>
    /// <param name="from">int: Paging start</param>
    /// <returns name="resp">Return EventChartInfo Response</returns>
    [Route("event")]
    [HttpGet]
    public async Task<Response<EventChartInfo>> GetEvent(int uid, int eid, int org, int from)
    {
        var maxItem = 50; // Max item server will return.
        var resp = new Response<EventChartInfo>();

        try
        {
            // Try to find event with eid.
            var @event = await context.QueryEventById(eid); // TODO: Save event to cache?

            // success.
            resp.Code = 0;
            var charts = @event.EventCharts;

            // See if has more to send?
            if (charts.Count - from * maxItem > maxItem)
            {
                resp.HasMore = true;
                resp.Next = from + 1;
            }
            else
            {
                resp.HasMore = false;
            }


            // insert into data[]
            for (var i = from * maxItem; resp.HasMore ? i != from * maxItem + maxItem : i != charts.Count; i++)
            {
                var song = charts[i].Chart.Song;
                var chart = charts[i].Chart;
                resp.Data.Add(new EventChartInfo
                {
                    Artist = org == 0 ? song.Artist : song.OriginalArtist,
                    Cid = chart.ChartId,
                    Cover = song.Cover,
                    Creator = chart.Creator,
                    Length = chart.Length,
                    Level = chart.Level,
                    Mode = chart.Mode,
                    Sid = song.SongId,
                    Time = Util.GetTimeStamp(song.Time),
                    Title = org != 0 ? song.OriginalTitle : song.Title,
                    Type = chart.Type,
                    Uid = chart.UserId,
                    Version = chart.Version
                });
            }

            return resp;
        }
        catch (InvalidOperationException) // No event found.
        {
            logger.LogError("No event with id {eid} found.", eid);
#if DEBUG
            throw;
#else
                resp.Code = 0;
                return resp;
#endif
        }
    }

    /// <summary>
    ///     Get server info.
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="api"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("info")]
    public async Task<object> GetInfo(int uid, int api)
    {
        return new
        {
            Code = 0,
            Api = Consts.API_VERSION,
            Min = Consts.MIN_SUPPORT,
            Welcome = $"MP Server v{Assembly.GetExecutingAssembly().GetName().Version} Online!"
        };
    }
}