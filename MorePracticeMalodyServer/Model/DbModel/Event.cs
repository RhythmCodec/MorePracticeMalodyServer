using System;
using System.Collections.Generic;

namespace MorePracticeMalodyServer.Model.DbModel
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Cover { get; set; }
        public List<EventChart> EventCharts { get; set; }
    }
}