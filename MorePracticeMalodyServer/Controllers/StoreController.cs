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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MorePracticeMalodyServer.Data;
using MorePracticeMalodyServer.Model;
using MorePracticeMalodyServer.Model.DataModel;
using MorePracticeMalodyServer.Model.DbModel;
using EventInfo = MorePracticeMalodyServer.Model.DataModel.EventInfo;

namespace MorePracticeMalodyServer.Controllers
{
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
        public async Task<Response<SongInfo>> GetList(int uid, int api, string word, int org, int mode, int lvge,
            int lvle, int beta, int from)
        {
            // If not support the api version, throw a exception.
            Util.CheckVersion(api);

            var resp = new Response<SongInfo>();
            // Max Items server will return.
            var maxItem = 50;

            if (word is null) // If word is null, Query all songs.
            {
                var total = context.Songs.Count();
                if (total - from * maxItem > maxItem)
                {
                    resp.HasMore = true;
                    resp.Next = from + 1;
                }
                else
                {
                    resp.HasMore = false;
                }

                var temp = context.Charts
                    .AsNoTracking();
                if (mode != -1) temp = temp.Where(c => c.Mode == mode);

                if (lvge != 0) temp = temp.Where(c => c.Level > lvge);

                if (lvle != 0) temp = temp.Where(c => c.Level < lvle);

                var result = await temp
                    .Select(c => c.Song)
                    .Distinct()
                    .OrderBy(s => s.SongId)
                    .Skip(from * maxItem)
                    .Take(maxItem)
                    .ToListAsync(); //TODO: Save to cache.

                resp.Code = 0;
                // Add song to return value.
                foreach (var song in result)
                    resp.Data.Add(new SongInfo
                    {
                        Artist = org == 0 ? song.Artist : song.OriginalArtist,
                        Bpm = song.Bpm,
                        Cover = song.Cover,
                        Length = song.Length,
                        Mode = song.Mode,
                        Sid = song.SongId,
                        Time = GetTimeStamp(song.Time),
                        Title = org == 0 ? song.Title : song.OriginalTitle
                    });

                return resp;
            }

            // Try to find data from memory cache first.
            var keyword = Util.TrimSpecial(word).ToLower();
            if (cache.TryGetValue(keyword, out List<Song> cachedSongList))
            {
            }
            else // Query db and write result to cache
            {
                var result = await context.Songs
                    .Include(s => s.Charts)
                    .Where(s => s.SearchString.Contains(keyword) ||
                                s.OriginalSearchString.Contains(keyword))
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToListAsync();

                // Create new cache entry. Set value abd expiration.
                var cacheEntry = cache.CreateEntry(keyword);
                cacheEntry.Value = result;
                cacheEntry.AbsoluteExpirationRelativeToNow = new TimeSpan(0, 2, 0);

                cachedSongList = result;
            }

            resp.Code = 0;

            // Filter cached song to match query string.
            foreach (var song in cachedSongList)
            {
                if (beta == 0) // Hide all Beta and Alpha chart if don't want to see beta.
                    song.Charts = song.Charts
                        .Where(c => c.Type == ChartState.Stable)
                        .ToList();
                if (lvle != 0 && lvge != 0)
                    song.Charts = song.Charts
                        .Where(c => c.Level > lvge && c.Level < lvle) // select level
                        .ToList();

                if (mode != -1)
                    song.Charts = song.Charts
                        .Where(c => c.Mode == mode) // select mode
                        .ToList();
            }

            // Delete all song if it don't have any chart match query string.
            cachedSongList = cachedSongList
                .Where(s => s.Charts.Any())
                .ToList();

            // If has more than MaxItem left, set HasMore to true, and set next index.
            if (cachedSongList.Count - from * maxItem > maxItem)
            {
                resp.HasMore = true;
                resp.Next = from + 1;
            }
            else
            {
                resp.HasMore = false;
            }

            // Init SongInfo and write data to it.
            Parallel.For(from * maxItem,
                from * maxItem - cachedSongList.Count > maxItem ? from * maxItem + maxItem : cachedSongList.Count,
                index =>
                {
                    resp.Data.Add(new SongInfo
                    {
                        Artist = cachedSongList[index].Artist,
                        Bpm = cachedSongList[index].Bpm,
                        Cover = cachedSongList[index].Cover,
                        Length = cachedSongList[index].Length,
                        Mode = cachedSongList[index].Mode,
                        Sid = cachedSongList[index].SongId,
                        Time = GetTimeStamp(cachedSongList[index].Time),
                        Title = org == 0 ? cachedSongList[index].Title : cachedSongList[index].OriginalTitle
                    });
                });

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
        public async Task<Response<SongInfo>> GetPromote(int uid, int api, int org, int mode, int from)
        {
            // If not support the api version, throw a exception.
            Util.CheckVersion(api);

            var resp = new Response<SongInfo>();
            var maxCount = 50;

            // Query promotes from database.
            var result = await context.Promotions
                .Include(p => p.Song)
                .AsNoTracking()
                .ToListAsync(); // TODO: Cache result?

            resp.Code = 0;
            // To see if has more.
            if (result.Count - maxCount * from > maxCount)
            {
                resp.HasMore = true;
                resp.Next = from + 1;
            }
            else
            {
                resp.HasMore = false;
            }

            // Insert to Data
            for (var i = from * maxCount; resp.HasMore ? i != from * maxCount + maxCount : i != result.Count; i++)
            {
                var song = result[i].Song;
                resp.Data.Add(new SongInfo
                {
                    Artist = org == 0 ? song.Artist : song.OriginalArtist,
                    Bpm = song.Bpm,
                    Cover = song.Cover,
                    Length = song.Length,
                    Mode = song.Mode,
                    Sid = song.SongId,
                    Time = GetTimeStamp(song.Time),
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
        public async Task<Response<ChartInfo>> GetChart(int uid, int api, int sid, int beta, int mode, int from,
            int promote)
        {
            // If not support the api version, throw a exception.
            Util.CheckVersion(api);

            var resp = new Response<ChartInfo>();
            // Max Items server will return.
            var maxItem = 50;

            // Try to find song with correct id.
            try
            {
                var result = await context.Songs
                    .Include(s => s.Charts)
                    .AsNoTracking()
                    .FirstAsync(s => s.SongId == sid);
                // Success
                resp.Code = 0;
                var charts = result.Charts.ToList();

                // Select stable if don't want to see beta.
                if (beta == 0)
                    result.Charts = charts
                        .Where(c => c.Type == ChartState.Stable)
                        .ToList();

                // Select mode if give a mode.
                if (mode != -1)
                    result.Charts = charts
                        .Where(c => c.Mode == mode)
                        .ToList();

                // If has more than MaxItem left, set HasMore to true, and set next index.
                if (charts.Count - from * maxItem > maxItem)
                {
                    resp.HasMore = true;
                    resp.Next = from + 1;
                }
                else
                {
                    resp.HasMore = false;
                }

                // Add charts to resp.
                for (var i = from * maxItem; resp.HasMore ? i != from * maxItem + maxItem : i != charts.Count; i++)
                    resp.Data.Add(new ChartInfo
                    {
                        Cid = charts[i].ChartId,
                        Creator = charts[i].Creator,
                        Length = charts[i].Length,
                        Level = charts[i].Level,
                        Mode = charts[i].Mode,
                        Size = charts[i].Size,
                        Type = charts[i].Type,
                        Uid = charts[i].UserId,
                        Version = charts[i].Version
                    });
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
        public async Task<Response<SongInfo>> QuerySong(int uid, int api, int sid = -1, int cid = -1, int org = 0)
        {
            // If not support the api version, throw a exception.
            Util.CheckVersion(api);

            // If not providing a valid SID or CID, throw a exception.
            if (sid == -1 && cid == -1)
            {
                logger.LogError("Query for sid {sid} or cid {cid} is not valid!", sid, cid);
                throw new ArgumentException("A valid SID or CID not provided.");
            }

            //
            var resp = new Response<SongInfo>();

            if (sid != -1) // Use sid to select song
                try
                {
                    var result = await context.Songs
                        .AsNoTracking()
                        .FirstAsync(s => s.SongId == sid);

                    // Success.
                    resp.Code = 0;
                    resp.Data.Add(new SongInfo // Add song to response.
                    {
                        Sid = result.SongId,
                        Artist = result.Artist,
                        Bpm = result.Bpm,
                        Cover = result.Cover,
                        Length = result.Length,
                        Mode = result.Mode,
                        Time = GetTimeStamp(result.Time),
                        Title = org != 0 ? result.OriginalTitle : result.Title
                    });

                    return resp;
                }
                catch (InvalidOperationException e) // Song doesn't exist.
                {
                    logger.LogError("Could not find a song with sid {sid}", sid);
#if DEBUG
                    throw;
#else
                    return resp;
#endif
                }

            if (cid != -1) // Use chart ID.
                try
                {
                    var result = await context.Charts
                        .Include(c => c.Song)
                        .AsSplitQuery()
                        .AsNoTracking()
                        .FirstAsync(c => c.ChartId == cid);

                    resp.Code = 0;

                    var song = result.Song;

                    resp.Data.Add(new SongInfo
                    {
                        Artist = org == 0 ? song.Artist : song.OriginalArtist,
                        Bpm = song.Bpm,
                        Cover = song.Cover,
                        Length = song.Length,
                        Mode = song.Mode,
                        Sid = song.SongId,
                        Time = GetTimeStamp(song.Time),
                        Title = org != 0 ? song.OriginalTitle : song.Title
                    });

                    return resp;
                }
                catch (InvalidOperationException e) // None Charts found.
                {
                    logger.LogError("Could not find a song with cid {cid}", cid);
#if DEBUG
                    throw;
#else
                    return resp;
#endif
                }
#if DEBUG
            logger.LogCritical("WTF goes here? Check QuerySong() for some sucking detail.");
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
        public async Task<DownloadResponse> GetDownload(int uid, int api, int cid)
        {
            // If not support the api version, throw a exception.
            Util.CheckVersion(api);

            var resp = new DownloadResponse();
            Chart chart = null;

            // Try to find the chart with cid given.
            try
            {
                chart = await context.Charts
                    .Include(c => c.Song)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .FirstAsync(c => c.ChartId == cid);
            }
            catch (InvalidOperationException) // No chart found.
            {
                logger.LogWarning("No chart with cid {cid} find!", cid);
                // Set response code to -2, indicates no chart found.
                resp.Code = -2;
                return resp;
            }

            // Try to find the download records with chart.
            try
            {
                var dls = await context.Downloads
                    .Include(d => d.Chart.Song)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToListAsync();

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

                    return resp;
                }
            }
            catch (ArgumentNullException e) // Something wrong because chart is null. This is impossible.
            {
                logger.LogError(e, "Query downloads for chart {cid} meets something wired.");
#if DEBUG
                throw;
#else
                resp.Code = -2;
                return resp;
#endif
            }

            return resp;
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
        public async Task<Response<EventInfo>> GetEvents(int uid, int api, int active, int from)
        {
            // If not support the api version, throw a exception.
            Util.CheckVersion(api);

            var maxItem = 50;
            var resp = new Response<EventInfo>();

            try
            {
                // Try query events.
                var query = context.Events
                    .AsNoTracking();

                if (active == 1)
                    query = query.Where(e => e.Active);

                // Success.
                var result = await query.ToListAsync(); // TODO: Save events to cache?

                resp.Code = 0;
                // To see if has more to send.
                if (result.Count - maxItem * from > maxItem)
                {
                    resp.HasMore = true;
                    resp.Next = from + 1;
                }
                else
                {
                    resp.HasMore = false;
                }

                // Write to response.Data
                for (var i = maxItem * from; resp.HasMore ? i != from * maxItem + maxItem : i != result.Count; i++)
                    resp.Data.Add(new EventInfo
                    {
                        Active = result[i].Active,
                        Cover = result[i].Cover,
                        Eid = result[i].EventId,
                        End = result[i].End.ToString("yyyy-MM-dd"),
                        Name = result[i].Name,
                        Start = result[i].Start.ToString("yyyy-MM-dd")
                    });

                return resp;
            }
            catch (ArgumentNullException) // Something impossible happened.
            {
                logger.LogError("¿");
                throw;
                // I DONT WANT TO WRITE HERE ANY MORE.
            }

            // Also something impossible.
            return resp;
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
        public async Task<Response<EventChartInfo>> GetEvent(int uid, int api, int eid, int org, int from)
        {
            // If not support the api version, throw a exception.
            Util.CheckVersion(api);

            var maxItem = 50; // Max item server will return.
            var resp = new Response<EventChartInfo>();

            try
            {
                // Try to find event with eid.
                var @event = await context.Events
                    .Include(e => e.EventCharts)
                    .ThenInclude(c => c.Chart)
                    .ThenInclude(c => c.Song)
                    .FirstAsync(e => e.EventId == eid); // TODO: Save event to cache?

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
                        Time = GetTimeStamp(song.Time),
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

        private long GetTimeStamp() // Get timestamp. 
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            var nowTime = DateTime.Now;
            var unixTime = (long)Math.Round((nowTime - startTime).TotalSeconds, MidpointRounding.AwayFromZero);
            return unixTime;
        }

        // We only need 10 digits timestamp
        private long GetTimeStamp(DateTime time) // Get specified timestamp.
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            var nowTime = time;
            var unixTime = (long)Math.Round((nowTime - startTime).TotalSeconds, MidpointRounding.AwayFromZero);
            return unixTime;
        }
    }
}