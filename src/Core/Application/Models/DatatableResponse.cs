namespace Application.Models
{
    public class DatatableResponse
    {
        public DatatableResponse()
        {
            RecordsTotal = 0;
            RecordsFiltered = 0;
            Data = new List<object>();
        }
        public long RecordsTotal { get; set; }
        public long RecordsFiltered { get; set; }
        public object Data { get; set; }
    }
}
