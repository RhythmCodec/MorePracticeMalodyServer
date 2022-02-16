using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MorePracticeMalodyServer.Model.DataModel;
using MorePracticeMalodyServer.Model.DbModel;

namespace MorePracticeMalodyServer.Data.Helper
{
    public static class SongHelper
    {
        // TODO: 使用out传出HasMore，减少一次数据库交互，所有的辅助方法都这么处理
        private static async Task<IEnumerable<SongInfo>> InternalQuerySong(DataContext context, string word, int org,
            int mode, int lvge, int lvle, int beta, int from, int maxCount)
        {
            // Build temp query chain to find songs
            var temp = context.Songs
                .AsNoTracking();
            if (mode != -1) temp = temp.Where(c => ((c.Mode >> mode) & 1) == 1);

            // We don't know the difficulty now. So just skip this.
            // If we can know difficulty in the future, I will fix this.
            if (lvge != 0) ;

            if (lvle != 0) ;

            // And now we set all chart Stable.
            if (beta != 0) ;

            if (word is not null)
                temp.Where(s => s.SearchString.Contains(word) || s.OriginalSearchString.Contains(word));

            // Select songs.
            List<Song> result;
            if (maxCount > 0)
                result = await temp
                    .OrderBy(s => s.SongId)
                    .Skip(from * maxCount)
                    .Take(maxCount)
                    .AsNoTracking()
                    .ToListAsync();
            else // Select all songs
                result = await temp
                    .AsNoTracking()
                    .ToListAsync();

            var list = new List<SongInfo>();

            // Add song to list.
            foreach (var song in result)
                list.Add(new SongInfo
                {
                    // To see if requires original strings.
                    Artist = org == 0 ? song.Artist : song.OriginalArtist,
                    Bpm = song.Bpm,
                    Cover = song.Cover,
                    Length = song.Length,
                    Mode = song.Mode,
                    Sid = song.SongId,
                    Time = Util.GetTimeStamp(song.Time),
                    Title = org == 0 ? song.Title : song.OriginalTitle
                });

            return list;
        }

        private static async Task<SongInfo> InternalQuerySong(DataContext context, int sid, int org)
        {
            var result = await context.Songs
                .AsNoTracking()
                .FirstAsync(s => s.SongId == sid);

            return new SongInfo
            {
                Sid = result.SongId,
                Artist = result.Artist,
                Bpm = result.Bpm,
                Cover = result.Cover,
                Length = result.Length,
                Mode = result.Mode,
                Time = Util.GetTimeStamp(result.Time),
                Title = org != 0 ? result.OriginalTitle : result.Title
            };
        }

        /// <summary>
        ///     Query songs with a max count of <see cref="Consts.MaxItem" />
        /// </summary>
        /// <param name="context"></param>
        /// <param name="word">Key word</param>
        /// <param name="org">Return original language song title</param>
        /// <param name="mode">Chart mode</param>
        /// <param name="lvge">Min level</param>
        /// <param name="lvle">Max level</param>
        /// <param name="beta">Return beta charts</param>
        /// <param name="from">Page index</param>
        /// <returns></returns>
        internal static async Task<IEnumerable<SongInfo>> QuerySong(this DataContext context, string word, int org,
            int mode, int lvge, int lvle, int beta, int from)
        {
            return await InternalQuerySong(context, word, org, mode, lvge, lvle, beta, from, Consts.MaxItem);
        }

        /// <summary>
        ///     Query all songs.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="word">Key word</param>
        /// <param name="org">Return original language song title</param>
        /// <param name="mode">Chart mode</param>
        /// <param name="lvge">Min level</param>
        /// <param name="lvle">Max level</param>
        /// <param name="beta">Return beta charts</param>
        /// <param name="from">Page index</param>
        /// <returns></returns>
        internal static async Task<IEnumerable<SongInfo>> QueryAllSong(this DataContext context, string word, int org,
            int mode, int lvge, int lvle, int beta, int from)
        {
            return await InternalQuerySong(context, word, org, mode, lvge, lvle, beta, from,
                -1); // -1 makes a query without max number limit.
        }

        internal static async Task<SongInfo> QuerySongWithId(this DataContext context, int sid, int org)
        {
            return await InternalQuerySong(context, sid, org);
        }

        internal static async Task<SongInfo> QuerySongWithCid(this DataContext context, int cid, int org)
        {
            var result = await context.Charts
                .Include(c => c.Song)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstAsync(c => c.ChartId == cid);

            var song = result.Song;

            return new SongInfo
            {
                Artist = org == 0 ? song.Artist : song.OriginalArtist,
                Bpm = song.Bpm,
                Cover = song.Cover,
                Length = song.Length,
                Mode = song.Mode,
                Sid = song.SongId,
                Time = Util.GetTimeStamp(song.Time),
                Title = org != 0 ? song.OriginalTitle : song.Title
            };
        }
    }
}