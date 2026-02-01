using System;
using System.Collections.Generic;

namespace waterSystemReem.Models
{
    public partial class CounterType
    {
        public CounterType()
        {
            Counters = new HashSet<Counter>();
        }

        public int CounterTypeId { get; set; }
        public string CounterTypeName { get; set; } = null!;
        public int MeterRang { get; set; }
        public int LocalComcils { get; set; }

        public virtual ICollection<Counter> Counters { get; set; }
    }
}
