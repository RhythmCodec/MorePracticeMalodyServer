namespace MorePracticeMalodyServer.Model.DataModel
{
    /// <summary>
    /// </summary>
    public class EventInfo
    {
        /// <summary>
        ///     Event id
        /// </summary>
        public int Eid { get; set; }

        /// <summary>
        ///     Event name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Event start time
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        ///     Event end time
        /// </summary>
        public string End { get; set; }

        /// <summary>
        ///     Event active status
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     Event cover picture
        /// </summary>
        public string Cover { get; set; }
    }
}