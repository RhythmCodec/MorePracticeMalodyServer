namespace MorePracticeMalodyServer.Model.DbModel
{
    /// <summary>
    ///     Promoted song list
    /// </summary>
    public class Promotion
    {
        public int Id { get; set; }

        /// <summary>
        ///     Promoted song.
        /// </summary>
        public Song Song { get; set; }
    }
}