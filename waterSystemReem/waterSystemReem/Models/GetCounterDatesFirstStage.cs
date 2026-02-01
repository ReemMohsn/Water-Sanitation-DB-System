using System;
using System.Collections.Generic;

namespace waterSystemReem.Models
{
    public partial class GetCounterDatesFirstStage
    {
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int CounterId { get; set; }
    }
}
