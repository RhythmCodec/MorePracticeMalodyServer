using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MorePracticeMalodyServer.Model.DbModel
{
    [Index(nameof(Active))]
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Cover { get; set; }
        public string Sponsor { get; set; }
        public List<EventChart> EventCharts { get; set; }
    }
}