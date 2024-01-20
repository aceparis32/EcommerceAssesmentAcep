using Application.Features.Product.Commands;
using Application.Features.Product.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssesmentAcep.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {
        [HttpPost("init")]
        public async Task<IActionResult> CreateInitialProduct()
        {
            return Ok(await Mediator.Send(new CreateInitialProductCommand()));
        }

        [Authorize]
        [HttpGet("datatable")]
        public async Task<IActionResult> GetDatatableProduct([FromQuery] GetProductDatatableQuery request)
        {
            return Ok(await Mediator.Send(request));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct([FromQuery] DeleteProductCommand request)
        {
            return Ok(await Mediator.Send(request));
        }
    }
}
