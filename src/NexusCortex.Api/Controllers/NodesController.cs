using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NexusCortex.Application.Services;
using NexusCortex.Domain;
using NexusCortex.Api.Dtos;

namespace NexusCortex.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NodesController : ControllerBase
    {
        private readonly INodeService _nodeService;
        private readonly IMomentumService _momentumService;

        public NodesController(INodeService nodeService, IMomentumService momentumService)
        {
            _nodeService = nodeService;
            _momentumService = momentumService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] NodeType? type, [FromQuery] Guid? parentId, [FromQuery] NodeStatus? status)
        {
            var nodes = await _nodeService.GetNodesAsync(type, parentId, status);
            return Ok(nodes);
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            var nodes = await _nodeService.GetTodayActionsAsync();
            return Ok(nodes);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateNodeRequest request)
        {
            var node = await _nodeService.CreateNodeAsync(request.Name, (NodeType)request.Type, (NodeStatus)request.Status, request.DueDate);
            return Ok(node);
        }

        [HttpGet("{id}/hierarchy")]
        public async Task<IActionResult> GetHierarchy(Guid id)
        {
            var hierarchy = await _nodeService.GetNodeHierarchyAsync(id);
            if (hierarchy == null) return NotFound();
            return Ok(hierarchy);
        }

        [HttpGet("{id}/impacts")]
        public async Task<IActionResult> GetImpacts(Guid id)
        {
            var impacts = await _nodeService.GetImpactedNodesAsync(id);
            return Ok(impacts);
        }

        [HttpPost("{id}/calculate-momentum")]
        public async Task<IActionResult> CalculateMomentum(Guid id)
        {
            var score = await _momentumService.CalculateMomentumAsync(id);
            return Ok(new { MomentumScore = score });
        }
    }
}
