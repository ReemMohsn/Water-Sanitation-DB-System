using System;
using System.Collections.Generic;

namespace waterSystemReem.Models
{
    public partial class Invoice
    {
        public int InvoiceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToData { get; set; }
        public double Balance { get; set; }
        public bool IsPaied { get; set; }
        public int CounterId { get; set; }
        public int WaterValue { get; set; }
        public int SanitationFees { get; set; }

        public virtual Counter Counter { get; set; } = null!;
    }
}
