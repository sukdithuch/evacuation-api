using Evacuation.Core.DTOs.Requests;
using Evacuation.Core.Interfaces.Services;
using Evacuation.Infrastructure.Database.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Evacuation.API.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var result = await _vehicleService.GetVehiclesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(VehicleRequest req)
        {
            try
            {
                var result = await _vehicleService.CreateVehicleAsync(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
