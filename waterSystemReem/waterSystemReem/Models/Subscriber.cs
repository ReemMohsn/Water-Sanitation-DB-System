using System;
using System.Collections.Generic;

namespace waterSystemReem.Models
{
    public partial class Subscriber
    {
        public Subscriber()
        {
            Counters = new HashSet<Counter>();
        }

        public int SubscriberId { get; set; }
        public long IdentificationNumber { get; set; }
        public string SubscriberName { get; set; } = null!;
        public string SubscriberPhone { get; set; } = null!;
        public DateTime? CreateDate { get; set; }

        public virtual ICollection<Counter> Counters { get; set; }
    }
}
