namespace Application.Models
{
    public class DatatableRequest
    {
        public DatatableRequest()
        {
            if (string.IsNullOrEmpty(OrderType))
                OrderType = "asc";
            Length = 10;
            Start = 0;
        }
        public string? Keyword { get; set; }
        public int Length { get; set; }
        public int Start { get; set; }
        public string? OrderCol { get; set; }
        public string OrderType { get; set; }
    }
}
