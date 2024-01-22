using Application.Exceptions;
using Application.Features.ErrorLog.Commands;
using Application.Features.Product.Models;
using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Features.Product.Queries
{
    public class GetProductDatatableQuery : DatatableRequest, IRequest<DatatableResponse>
    {

    }

    public class GetProductDatatableQueryHandler : IRequestHandler<GetProductDatatableQuery, DatatableResponse>
    {
        private readonly IApplicationDbContext dbContext;
        private readonly IRedisActionDbService redisActionDbService;
        private readonly IMediator mediator;

        public GetProductDatatableQueryHandler(IApplicationDbContext dbContext, IRedisActionDbService redisActionDbService, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.redisActionDbService = redisActionDbService;
            this.mediator = mediator;
        }
        public async Task<DatatableResponse> Handle(GetProductDatatableQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await redisActionDbService.GetUserAllowedAction("READ_PRODUCT");

                var query = dbContext.Products.Where(x => x.DeletedDt == null).AsQueryable();

                var totalRecord = query.Count();

                switch (request.OrderCol)
                {
                    case "name":
                        query = request.OrderType.ToLower() == "asc" ?
                            query.OrderBy(x => x.Name) :
                            query.OrderByDescending(x => x.Name);
                        break;
                    case "price":
                        query = request.OrderType.ToLower() == "asc" ?
                            query.OrderBy(x => x.Price) :
                            query.OrderByDescending(x => x.Price);
                        break;
                    case "rating":
                        query = request.OrderType.ToLower() == "asc" ?
                            query.OrderBy(x => x.Rating) :
                            query.OrderByDescending(x => x.Rating);
                        break;
                    case "stock":
                        query = request.OrderType.ToLower() == "asc" ?
                            query.OrderBy(x => x.Stock) :
                            query.OrderByDescending(x => x.Stock);
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(request.Keyword))
                    query = query.Where(x => x.Name.ToLower().Contains(request.Keyword.ToLower()));

                if (query == null)
                {
                    return new DatatableResponse
                    {
                        RecordsFiltered = 0,
                        RecordsTotal = 0,
                        Data = new GetProductModel(),
                    };
                }

                var data = query.Skip(request.Start).Take(request.Length)
                    .Select(x => new GetProductModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Price = x.Price,
                        Rating = x.Rating,
                        Stock = x.Stock,
                        Brand = x.Brand,
                        Category = x.Category
                    });

                return new DatatableResponse
                {
                    RecordsTotal = totalRecord,
                    RecordsFiltered = data.Count(),
                    Data = data
                };
            }
            catch (Exception e)
            {
                await mediator.Send(new CreateErrorLogCommand
                {
                    ErrorMessage = e.Message
                });
                throw new BadRequestException(e.Message);
            }
        }
    }
}
