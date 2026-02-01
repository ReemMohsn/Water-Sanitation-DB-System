using System;
using System.Collections.Generic;

namespace waterSystemReem.Models
{
    public partial class Counter
    {
        public Counter()
        {
            Invoices = new HashSet<Invoice>();
        }

        public int CounterId { get; set; }
        public long CounterNumber { get; set; }
        public string CounterLocation { get; set; } = null!;
        public DateTime DateLastReading { get; set; }
        public double PreviseBalance { get; set; }
        public int SubscriberId { get; set; }
        public int CounterTypeId { get; set; }

        public virtual CounterType CounterType { get; set; } = null!;
        public virtual Subscriber Subscriber { get; set; } = null!;
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
