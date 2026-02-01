namespace waterSystemReem.Models
{
    public class InvoiceCreateViewModel
    {
        public int CounterId { get; set; }
        public int PrevReading { get; set; }
        public int CurrReading { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
