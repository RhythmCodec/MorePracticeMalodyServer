/* Copyright (C) 2021 RhythmCodec
 * See @RhythmCodec at https://github.com/RhythmCodec
 *
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MorePracticeMalodyServer.Data;
using MorePracticeMalodyServer.Model;
using MorePracticeMalodyServer.Model.DbModel;

namespace MorePracticeMalodyServer.Controllers
{
    /// <summary>
    ///     Controllers that process all thing about uploading.
    /// </summary>
    [Route("api/store/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly DataContext context;
        private readonly ILogger<UploadController> logger;

        /// <summary>
        ///     Init controller.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public UploadController(DataContext context, ILogger<UploadController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        [Route("sign")]
        [HttpPost]
        public async Task<SignResponse> PostSign(int uid, int api, int sid, int cid, string name, string hash)
        {
            // If not support the api version, throw a exception.
            if (api != Consts.API_VERSION)
                throw new NotSupportedException($"This server does not support api version {api}.");
            logger.LogInformation("Upload sign phase!");
            logger.LogInformation("User {uid} trying to upload chart {cid} for song {sid}.", uid, cid, sid);

            var resp = new SignResponse();
            Song song;
            // Try to find song first.
            try
            {
                song = await context.Songs
                    .FirstAsync(s => s.SongId == sid);
                logger.LogInformation("Find song {sid} in database.", sid);
            }
            catch (InvalidOperationException)
            {
                // Create song if not found.
                song = new Song();
                song.SongId = sid;
                logger.LogInformation("Song {sid} not found! Created record with id {sid}.", sid, sid);
            }

            // To see if this chart is already exist.
            if (song.Charts.Any(c => c.ChartId == cid))
            {
                // If exists, we may need to update the chart.
                // If chart is stable, we won't update anyway.
                // TODO: we will do this later.
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
                    Type = ChartState.NotUpdated
                };
                song.Charts.Add(chart);
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
            resp.Meta.Add("sid", sid.ToString());
            resp.Meta.Add("cid", cid.ToString());
            resp.Meta.Add("hash", hash);
            resp.Host =
                $"http://{HttpContext.Connection.LocalIpAddress}:{HttpContext.Connection.LocalPort}/store/upload/upload";

            return resp;
        }

        // TODO: Chart upload controllers.
    }
}