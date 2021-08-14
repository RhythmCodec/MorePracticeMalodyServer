namespace MorePracticeMalodyServer.Model.DbModel
{
    public class EventChart
    {
        public int Id { get; set; }
        public Chart Chart { get; set; }
        public Song Song { get; set; }
    }
}