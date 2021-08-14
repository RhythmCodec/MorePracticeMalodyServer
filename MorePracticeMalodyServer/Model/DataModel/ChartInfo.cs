using MorePracticeMalodyServer.Model.DbModel;

namespace MorePracticeMalodyServer.Model.DataModel
{
    /// <summary>
    ///     Info of a chart.
    /// </summary>
    public class ChartInfo
    {
        public int Cid { get; set; }
        public int Uid { get; set; }
        public string Creator { get; set; }
        public string Version { get; set; }
        public int Level { get; set; }
        public int Length { get; set; }
        public ChartState Type { get; set; }
        public int Size { get; set; }
        public int Mode { get; set; }
    }
}