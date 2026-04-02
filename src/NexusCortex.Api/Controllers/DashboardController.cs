using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NexusCortex.Application.Services;

namespace NexusCortex.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IStagnationService _stagnationService;

        public DashboardController(IDashboardService dashboardService, IStagnationService stagnationService)
        {
            _dashboardService = dashboardService;
            _stagnationService = stagnationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dashboard = await _dashboardService.GetDashboardAsync();
            return Ok(dashboard);
        }

        [HttpGet("stagnant")]
        public async Task<IActionResult> GetStagnant([FromQuery] int days = 7)
        {
            var stagnantNodes = await _stagnationService.GetStagnantNodesAsync(days);
            return Ok(stagnantNodes);
        }
    }
}
