namespace Application.Features.Product.Models
{
    public class InitialProductModel
    {
        public InitialProductModel()
        {
            Products = new List<GetInitialProducts>();
        }
        public List<GetInitialProducts> Products { get; set; }
    }

    public class GetInitialProducts
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
