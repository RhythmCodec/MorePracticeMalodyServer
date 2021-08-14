namespace MorePracticeMalodyServer.Model.DataModel
{
    /// <summary>
    ///     Info of a song
    /// </summary>
    public class SongInfo
    {
        /// <summary>
        ///     Song Id, unique.
        /// </summary>
        public int Sid { get; set; }

        /// <summary>
        ///     Full URL to cover.
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        ///     Length of a song. Unit in seconds.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        ///     BPM of song.
        /// </summary>
        public double Bpm { get; set; }

        /// <summary>
        ///     Title of song.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Artist of song.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        ///     Bitmask of modes that the song contains.<br />
        ///     <example>
        ///         Example:<br />If the song has <see cref="Consts.MODE_KEY" /> and <see cref="Consts.MODE_CATCH" /> mode, the
        ///         bitmask is
        ///         <code>(1 &lt;&lt; 0) | (1 &lt;&lt; 3) = 9</code>
        ///     </example>
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        ///     Last update time of a song.
        /// </summary>
        public long Time { get; set; }
    }
}