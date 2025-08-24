using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Evacuation.API.Controllers
{
    [ApiController]
    [Route("api/evacuation-zones")]
    public class EvacuationZonesController : ControllerBase
    {
        private readonly IEvacuationZoneService _evacuationZoneService;

        public EvacuationZonesController(IEvacuationZoneService evacuationZoneService)
        {
            _evacuationZoneService = evacuationZoneService ?? throw new ArgumentNullException(nameof(evacuationZoneService));
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var result = await _evacuationZoneService.GetEvacuationZonesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(EvacuationZoneRequest req)
        {
            try
            {
                var result = await _evacuationZoneService.CreateEvacuationZoneAsync(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
