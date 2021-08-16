using MorePracticeMalodyServer.Model.DbModel;

namespace MorePracticeMalodyServer.Model.DataModel
{
    /// <summary>
    ///     Info of a chart.
    /// </summary>
    public class ChartInfo
    {
        /// <summary>
        ///     Chart id
        /// </summary>
        /// <value></value>
        public int Cid { get; set; }
        /// <summary>
        ///     Chart Creator id
        /// </summary>
        /// <value></value>
        public int Uid { get; set; }
        /// <summary>
        ///     Chart Creator Name
        /// </summary>
        /// <value></value>
        public string Creator { get; set; }
        /// <summary>
        ///     Chart version
        /// </summary>
        /// <value></value>
        public string Version { get; set; }
        /// <summary>
        ///     Chart level
        /// </summary>
        /// <value></value>
        public int Level { get; set; }
        /// <summary>
        ///     Chart length
        /// </summary>
        /// <value></value>
        public int Length { get; set; }
        /// <summary>
        ///     Chart type
        /// </summary>
        /// <value></value>
        public ChartState Type { get; set; }
        /// <summary>
        ///     Chart size
        /// </summary>
        /// <value></value>
        public int Size { get; set; }
        /// <summary>
        ///     Chart mode
        /// </summary>
        /// <value></value>
        public int Mode { get; set; }
    }
}