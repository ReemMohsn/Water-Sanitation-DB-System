namespace waterSystemReem.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Keyless]
    public class CounterListViewModel
    {
        [Column("counter_id")]
        public int CounterId { get; set; }

        [Column("counter_number")]
        public long CounterNumber { get; set; }

        [Column("counter_location")]
        public string CounterLocation { get; set; }

        [Column("date_last_reading")]
        public DateTime DateLastReading { get; set; }

        [Column("previse_balance")]
        public double PreviseBalance { get; set; }

        [Column("counter_type_name")]
        public string CounterTypeName { get; set; }
    }


}
