using System;
using System.Collections.Generic;

namespace waterSystemReem.Models
{
    public partial class CounterDetailsView
    {
        public int CounterId { get; set; }
        public long CounterNumber { get; set; }
        public string CounterLocation { get; set; } = null!;
        public DateTime DateLastReading { get; set; }
        public double PreviseBalance { get; set; }
        public string SubscriberName { get; set; } = null!;
        public string CounterTypeName { get; set; } = null!;
        public int MeterRang { get; set; }
        public int LocalComcils { get; set; }
    }
}
