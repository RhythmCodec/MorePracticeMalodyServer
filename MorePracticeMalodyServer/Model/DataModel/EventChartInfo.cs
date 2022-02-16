namespace MorePracticeMalodyServer.Model.DataModel
{
    /// <summary>
    ///     Reference to <see cref="Model.DataModel.ChartInfo" />.
    /// </summary>
    public class EventChartInfo : ChartInfo
    {
        public int Sid { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Cover { get; set; }
        public long Time { get; set; }
    }
}