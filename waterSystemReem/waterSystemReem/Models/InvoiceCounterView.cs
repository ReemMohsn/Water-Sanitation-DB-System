using System;
using System.Collections.Generic;

namespace waterSystemReem.Models
{
    public partial class InvoiceCounterView
    {
        public string CounterTypeName { get; set; } = null!;
        public int LocalComcils { get; set; }
        public int MeterRang { get; set; }
        public long CounterNumber { get; set; }
        public int CounterId { get; set; }
        public string CounterLocation { get; set; } = null!;
        public int InvoiceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToData { get; set; }
        public double Balance { get; set; }
        public bool IsPaied { get; set; }
        public int WaterValue { get; set; }
        public int SanitationFees { get; set; }
        public string SubscriberName { get; set; } = null!;
    }
}
