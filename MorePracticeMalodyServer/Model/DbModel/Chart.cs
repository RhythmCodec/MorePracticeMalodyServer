using Microsoft.EntityFrameworkCore;

namespace MorePracticeMalodyServer.Model.DbModel
{
    /// <summary>
    ///     A chart in database.
    /// </summary>
    [Index("Type", "Mode", "Level")]
    public class Chart
    {
        /// <summary>
        ///     AKA Cid.
        /// </summary>
        public int ChartId { get; set; }

        /// <summary>
        ///     .
        ///     AKA Uid
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Username of creator.
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        ///     Chart version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     Chart difficulty.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        ///     Chart playable length. In seconds.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        ///     Chart state.See <see cref="ChartState" />
        /// </summary>
        public ChartState Type { get; set; }

        /// <summary>
        ///     Download size of chart.Unit in bytes.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        ///     Chart mode.
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        ///     Navigation to <see cref="Song" />.
        /// </summary>
        public Song Song { get; set; }
    }

    public enum ChartState
    {
        Alpha,
        Beta,
        Stable
    }
}