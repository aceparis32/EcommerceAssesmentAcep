using Application.Features.Product.Commands;
using Application.Features.UserTransaction.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssesmentAcep.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTransactionController : BaseController
    {
        [Authorize]
        [HttpPost("buy")]
        public async Task<IActionResult> BuyProductUserTransaction([FromBody] BuyProductCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [Authorize]
        [HttpPatch("pay")]
        public async Task<IActionResult> PayProductUserTransaction([FromQuery] PayProductCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [Authorize]
        [HttpPatch("ship")]
        public async Task<IActionResult> ShipProductUserTransaction([FromQuery] ShipProductCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [Authorize]
        [HttpPatch("accept")]
        public async Task<IActionResult> AcceptUserTransaction([FromQuery] AcceptTransactionCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [Authorize]
        [HttpPatch("complete")]
        public async Task<IActionResult> CompleteUserTransaction([FromQuery] CompleteTransactionCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [Authorize]
        [HttpPatch("reject")]
        public async Task<IActionResult> RejectUserTransaction([FromQuery] RejectTransactionCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [Authorize]
        [HttpPatch("cancel")]
        public async Task<IActionResult> CancelUserTransaction([FromQuery] CancelTransactionCommand request)
        {
            return Ok(await Mediator.Send(request));
        }
    }
}
