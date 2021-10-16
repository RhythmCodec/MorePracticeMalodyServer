/* Copyright (C) 2021 RhythmCodec
 * See @RhythmCodec at https://github.com/RhythmCodec
 *
 * Last Update: 2021/08/22
 * Author: Kami11
 * Last Modifier: soloopooo
 * Description:
 *      
 *      The Upload module of server.
 *      Provides Chart Sign up and charts uploading.
 *      
 * Need datas: 
 *     
 *      Hmm. looks like just your File...
 * 
 * Providing datas: cid, sid, Response Code
 *      
 *      
 *
 * Known bugs: 
 *      
 *      TODO: get the length of song file.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MorePracticeMalodyServer.Data;
using MorePracticeMalodyServer.Model;
using MorePracticeMalodyServer.Model.DbModel;
using MorePracticeMalodyServer.Model.FileModel;
using NVorbis;

namespace MorePracticeMalodyServer.Controllers
{
    /// <summary>
    ///     Controllers that process all thing about uploading.
    /// </summary>
    [Route("api/store/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly DataContext context;
        private readonly ILogger<UploadController> logger;

        /// <summary>
        ///     Init controller.
        /// </summary>
        /// <param name="context">DataContext</param>
        /// <param name="logger">logger</param>
        public UploadController(DataContext context, ILogger<UploadController> logger, IConfiguration configuration)
        {
            this.context = context;
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        ///     Upload stage 1: Get sid and cid, upload main info.
        /// </summary>
        /// <param name="uid">int: User id</param>
        /// <param name="api">int: Api version</param>
        /// <param name="sid">int: Song id. Must have</param>
        /// <param name="cid">int: Chart id. Must have</param>
        /// <param name="name">string: File name. Seperated with comma</param>
        /// <param name="hash">string: File hashs. Seperated with comma</param>
        /// <returns></returns>
        [Route("sign")]
        [HttpPost]
        public async Task<SignResponse> PostSign(int uid, int api, [FromForm] int sid, [FromForm] int cid,
            [FromForm] string name, [FromForm] string hash)
        {
            // If not support the api version, throw a exception.
            Util.CheckVersion(api);

            logger.LogInformation("Upload sign phase!");
            logger.LogInformation("User {uid} trying to upload chart {cid} for song {sid}.", uid, cid, sid);

            var resp = new SignResponse();
            Song song;
            // Try to find song first.
            try
            {
                song = await context.Songs
                    .Include(s => s.Charts)
                    .AsSplitQuery()
                    .FirstAsync(s => s.SongId == sid);
                logger.LogInformation("Find song {sid} in database.", sid);
            }
            catch (InvalidOperationException)
            {
                // Create song if not found.
                song = new Song();
                song.SongId = sid;
                song.Time = DateTime.Now;
                context.Songs.Add(song);
                logger.LogInformation("Song {sid} not found! Created record with id {sid}.", sid, sid);
            }

            // To see if this chart is already exist.
            if (song.Charts is not null && song.Charts.Any(c => c.ChartId == cid))
            {
                logger.LogInformation("Chart exists. Trying to update it.");
                // Now song should be tracked by context.

                // Update chart.
                // And all we can do is update Time...
                song.Time = DateTime.Now;
            }
            else
            {
                logger.LogInformation("Add new chart to song {sid}.", sid);
                // Add new chart to song.
                var chart = new Chart
                {
                    ChartId = cid,
                    UserId = uid,
                    Song = song,
                    Type = ChartState.NotUploaded
                };
                context.Charts.Add(chart);

                await context.SaveChangesAsync();
            }

            // We just check if name and hash has the same count.
            var names = name.Split(',');
            var hashes = hash.Split(',');
            if (names.Length != hashes.Length)
            {
                resp.Code = 0;
                resp.ErrorIndex = names.Length - 1;
                resp.ErrorMsg = $"{names.Length} file(s) provide. But only {hashes.Length} hash(es) give to server.";

                return resp;
            }

            // Now we can upload.
            resp.Code = 0;
            resp.ErrorMsg = "";
            // Malody seems send one file a time.
            // Each file should have a meta Item, otherwise game will think something wrong.
            foreach (var h in hashes)
                resp.Meta.Add(new
                {
                    Sid = sid,
                    Cid = cid,
                    Hash = h
                });
            if (configuration["Storage:Provider"]?.ToLower() != "self") // Use other storage provider.
                resp.Host = configuration["Storage:Provider"];
            else // Use self storage provide.
                resp.Host =
                    $"http://{Request.Host.Value}/api/SelfUpload/receive";

            return resp;
        }

        /// <summary>
        ///     Upload Stage 3: Finish stage.
        /// </summary>
        /// <param name="uid">int: user id. Must have</param>
        /// <param name="api">int: Api version</param>
        /// <param name="sid">int: song id. Must have</param>
        /// <param name="cid">int: chart id. Must have</param>
        /// <param name="name">string: The Filename of all files. Seperated with comma</param>
        /// <param name="hash">string: hashs(md5) of files. Seperated with comma</param>
        /// <param name="size">int: File size in total</param>
        /// <param name="main">string: the CHART's md5</param>
        /// <returns>Response Code</returns>
        [HttpPost]
        [Route("finish")]
        public async Task<object> FinishCheck(int uid, int api, [FromForm] int sid, [FromForm] int cid,
            [FromForm] string name, [FromForm] string hash, [FromForm] int size,
            [FromForm] string main)
        {
            Util.CheckVersion(api);

            var selfProvide = true;

            // Check if chart exist.
            if (!context.Charts.Any(c => c.ChartId == cid)) return new { Code = -2 };

            // Check if name and hash has the same count.
            var names = name.Split(',');
            var hashes = hash.Split(',');
            if (names.Length != hashes.Length) return new { Code = -1 };

            // Check if self provide mode.
            if (configuration["Storage:Provider"]?.ToLower() != "self") selfProvide = false;

            if (selfProvide)
            {
                var chart = await context.Charts
                    .Include(c => c.Downloads)
                    .FirstAsync(c => c.ChartId == cid);


                // Update download links.
                if (!chart.Downloads.Any())
                {
                    for (var i = 0; i != names.Length; i++)
                    {
                        var encodedName = HttpUtility.UrlEncode(names[i]);

                        context.Downloads.Add(new Download
                        {
                            ChartId = cid,
                            File =
                                $"http://{Request.Host.Value}/{sid}/{cid}/{encodedName}", // Fix issue #1
                            Hash = hashes[i],
                            Name = encodedName
                        });
                    }
                }
                else
                {
                    // Map name to hash.
                    Dictionary<string, string> nameToHash = new();
                    for (var i = 0; i != names.Length; i++) nameToHash[names[i]] = hashes[i];

                    // Find what's new.
                    var adds = names.AsEnumerable()
                        .Except(chart.Downloads.Select(d => HttpUtility.UrlDecode(d.Name))) // Fix issue #1
                        .ToList();
                    foreach (var add in adds)
                        // Add new files.
                    {
                        var encodedName = HttpUtility.UrlEncode(add);

                        context.Downloads.Add(new Download
                        {
                            ChartId = cid,
                            File =
                                $"http://{Request.Host.Value}/{sid}/{cid}/{encodedName}", // Fix issue #1
                            Hash = nameToHash[encodedName],
                            Name = encodedName
                        });
                    }

                    // Find what should delete.
                    var dels = chart.Downloads.Select(d => HttpUtility.UrlDecode(d.Name)) // Fix issue #1
                        .Except(names)
                        .ToList();
                    foreach (var del in dels)
                        chart.Downloads.RemoveAll(d => d.Name == HttpUtility.UrlEncode(del)); // Fix issue #1

                    //Update others.
                    foreach (var d in chart.Downloads) d.Hash = nameToHash[HttpUtility.UrlDecode(d.Name)];
                }

                await context.SaveChangesAsync();
            }

            // Parse mc file and update info.
            if (selfProvide)
            {
                // Map hash to name.
                Dictionary<string, string> hashToName = new();
                for (var i = 0; i != names.Length; i++) hashToName[hashes[i]] = names[i];

                try
                {
                    // Open file.
                    var file = await Task.Run(() => McFile.ParseLocalFile(Path.Combine(Environment.CurrentDirectory,
                            "wwwroot",
                            sid.ToString(), cid.ToString(),
                            HttpUtility.UrlEncode(hashToName[main])),
                        hashToName[main])); // Fix issue #1

                    var chartMeta = file.Meta;
                    var songMeta = file.Meta.Song;

                    var chart = await context.Charts
                        .Include(c => c.Song)
                        .FirstAsync(c => c.ChartId == cid);

                    // Try to find music file name.
                    string soundFileName = null;
                    try
                    {
                        // If only has one ogg, that's it!
                        soundFileName = names.Single(n => n.Contains(".ogg"));
                    }
                    catch (InvalidOperationException)
                    {
                        // If has more than one, we can check song meta.
                        // But this may be null.
                        // If you choose music file when uploading, this will have a value.
                        soundFileName = songMeta.File;
                    }

                    // Now read sound file to get length.
                    var soundLength = 0;
                    if (soundFileName is not null)
                    {
                        soundFileName = HttpUtility.UrlEncode(soundFileName);
                        using var song = new VorbisReader(Path.Combine(Environment.CurrentDirectory, "wwwroot",
                            sid.ToString(), cid.ToString(), soundFileName));
                        soundLength = (int)song.TotalTime.TotalSeconds;
                    }

                    soundFileName = HttpUtility.UrlEncode(soundFileName);

                    // Update our db.
                    chart.Creator = chartMeta.Creator;
                    chart.Length = soundLength;
                    chart.Level = 0; // This doesn't matter..Maybe //TODO: find a way to kown the difficulty.
                    chart.Mode = chartMeta.Mode;
                    chart.Size = size;
                    chart.Type = ChartState.Stable; // Now we have no way to know if this is beta or stable.
                    chart.UserId = uid;
                    chart.Version = chartMeta.Version;
                    chart.Song.Artist = songMeta.Artist;
                    chart.Song.Bpm = songMeta.Bpm;
                    chart.Song.Cover =
                        $"http://{Request.Host.Value}/{sid}/{cid}/{HttpUtility.UrlEncode(chartMeta.Background)}"; // Fix issue #1
                    chart.Song.Length = soundLength;
                    chart.Song.Mode |= 1 << chartMeta.Mode;
                    chart.Song.OriginalArtist = songMeta.Artistorg ?? songMeta.Artist;
                    chart.Song.OriginalTitle = songMeta.Titleorg ?? songMeta.Title;
                    chart.Song.Title = songMeta.Title;

                    // Prepare search string for searching.
                    chart.Song.SearchString =
                        $"{Util.TrimSpecial(songMeta.Artist).ToLower()}-{Util.TrimSpecial(songMeta.Title).ToLower()}";
                    chart.Song.OriginalSearchString =
                        $"{Util.TrimSpecial(chart.Song.OriginalArtist).ToLower()}-{Util.TrimSpecial(chart.Song.OriginalTitle).ToLower()}";

                    await context.SaveChangesAsync();
                }
                catch (FileNotFoundException) // WHY??
                {
                    logger.LogError("No chart file {name} found in file system.",
                        HttpUtility.UrlEncode(hashToName[main]));
                    return new { Code = -2 };
                }
            }


            return new { Code = 0 };
        }
    }
}