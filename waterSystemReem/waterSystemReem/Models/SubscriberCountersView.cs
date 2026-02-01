using System;
using System.Collections.Generic;

namespace waterSystemReem.Models
{
    public partial class SubscriberCountersView
    {
        public int SubscriberId { get; set; }
        public long IdentificationNumber { get; set; }
        public string SubscriberName { get; set; } = null!;
        public string SubscriberPhone { get; set; } = null!;
        public int CounterId { get; set; }
        public long CounterNumber { get; set; }
        public int CounterTypeId { get; set; }
        public string CounterLocation { get; set; } = null!;
        public double PreviseBalance { get; set; }
        public string CounterTypeName { get; set; } = null!;
    }
}
