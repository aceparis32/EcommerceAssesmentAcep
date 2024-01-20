using Application.Features.Product.Models;
using Application.Interfaces;
using MediatR;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Application.Features.Product.Commands
{
    public class CreateInitialProductCommand : IRequest<Unit>
    {

    }

    public class CreateInitialProductCommandHandler : IRequestHandler<CreateInitialProductCommand, Unit>
    {
        private readonly IApplicationDbContext dbContext;
        private static HttpClient client;
        public CreateInitialProductCommandHandler(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            client = new HttpClient();
        }
        public async Task<Unit> Handle(CreateInitialProductCommand request, CancellationToken cancellationToken)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(new Uri("https://dummyjson.com/products?limit=100"));

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<InitialProductModel>(jsonString);

                foreach (var product in products.Products)
                {
                    var newProduct = new Domain.Entities.Product
                    {
                        Name = product.Title,
                        Description = product.Description,
                        Price = product.Price * 10000,
                        Rating = product.Rating,
                        Stock = product.Stock,
                        Brand = product.Brand,
                        Category = product.Category,
                        CreatedBy = "system",
                        CreatedDt = DateTime.UtcNow,
                    };
                    await dbContext.Products.AddAsync(newProduct);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}
