using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.Interfaces.Services;
using Evacuation.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Evacuation.API.Controllers
{
    [ApiController]
    [Route("api/evacuations")]
    public class EvacuationsController : Controller
    {
        private readonly IEvacuationPlanService _evacuationPlanService;
        private readonly IEvacuationStatusService _evacuationStatusService;

        public EvacuationsController(IEvacuationPlanService evacuationPlanService,
            IEvacuationStatusService evacuationStatusService)
        {
            _evacuationPlanService = evacuationPlanService ?? throw new ArgumentNullException(nameof(evacuationPlanService));
            _evacuationStatusService = evacuationStatusService ?? throw new ArgumentNullException(nameof(evacuationStatusService));
        }

        [HttpPost("plan")]
        public async Task<ActionResult> GeneratePlan()
        {
            try
            {
                var result = await _evacuationPlanService.GeneratePlans();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            try
            {
                var result = await _evacuationStatusService.GetStatusesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult> UpdateStatus(EvacuationStatusRequest req)
        {
            try
            {
                await _evacuationStatusService.UpdateStatusAsync(req);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("clear")]
        public async Task<ActionResult> ClearPlans()
        {
            try
            {
                await _evacuationPlanService.ClearPlansAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
