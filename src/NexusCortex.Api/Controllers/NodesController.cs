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

        public NodesController(INodeService nodeService)
        {
            _nodeService = nodeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var nodes = await _nodeService.GetNodesAsync();
            return Ok(nodes);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateNodeRequest request)
        {
            var node = await _nodeService.CreateNodeAsync(request.Name, (NodeType)request.Type);
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
    }
}
