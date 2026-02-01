namespace waterSystemReem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AddCounterViewModel
    {
        [Required(ErrorMessage = "Counter number is required.")]
        [Display(Name = "Counter Number")]
        public long CounterNumber { get; set; }

        [Required(ErrorMessage = "Counter location is required.")]
        [Display(Name = "Counter Location")]
        public string CounterLocation { get; set; } = string.Empty;


        // We default the previous balance to 0; not editable by user.
        public double PreviseBalance { get; set; } = 0;

        [Required(ErrorMessage = "Subscriber id is required.")]
        [Display(Name = "Subscriber Id")]
        public int SubscriberId { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Counter Type must be 1 (Commercial) or 2 (Home).")]
        public int CounterTypeId { get; set; }


    }
}
