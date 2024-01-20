using Application.Features.AuditTrail.Queries;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssesmentAcep.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditTrailController : BaseController
    {
        [Authorize]
        [HttpGet("datatable")]
        public async Task<IActionResult> GetAuditTrailDatatable([FromQuery] GetAuditTrailDatatableQuery request)
        {
            return Ok(await Mediator.Send(request));
        }
    }
}
