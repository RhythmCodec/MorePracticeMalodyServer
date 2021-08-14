using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace MorePracticeMalodyServer.Model.DbModel
{
    /// <summary>
    ///     A song record in the database.
    /// </summary>
    [Index("Title", "Artist", "Mode")]
    public class Song
    {
        /// <summary>
        ///     AKA Sid.
        /// </summary>
        public int SongId { get; set; }

        /// <summary>
        ///     Full url to song cover.
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        ///     Length of song.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        ///     Bpm of song.
        /// </summary>
        [DefaultValue(0)]
        public double Bpm { get; set; }

        /// <summary>
        ///     Title of song.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Title in original language.
        /// </summary>
        public string OriginalTitle { get; set; }

        /// <summary>
        ///     Artist of song.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        ///     Mode bitmask of song.
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        ///     Last update time.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        ///     Navigation To <see cref="Chart" />.
        /// </summary>
        public ICollection<Chart> Charts { get; set; }
    }
}