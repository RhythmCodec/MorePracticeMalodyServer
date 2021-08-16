using MorePracticeMalodyServer.Model.DbModel;

namespace MorePracticeMalodyServer.Model.DataModel
{
    /// <summary>
    ///     Reference to <see cref="Model.DataModel.ChartInfo" />.
    /// </summary>
    public class EventChartInfo
    {
        public int Cid { get; set; }
        public int Sid { get; set; }
        public int Uid { get; set; }
        public string Creator { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Version { get; set; }
        public int Level { get; set; }
        public int Length { get; set; }
        public ChartState Type { get; set; }
        public string Cover { get; set; }
        public long Time { get; set; }
        public int Mode { get; set; }
    }
}