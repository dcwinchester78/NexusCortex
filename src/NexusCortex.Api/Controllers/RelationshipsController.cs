using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NexusCortex.Application.Services;
using NexusCortex.Domain;
using NexusCortex.Api.Dtos;

namespace NexusCortex.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RelationshipsController : ControllerBase
    {
        private readonly IRelationshipService _relationshipService;

        public RelationshipsController(IRelationshipService relationshipService)
        {
            _relationshipService = relationshipService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var relationships = await _relationshipService.GetRelationshipsAsync();
            return Ok(relationships);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateRelationshipRequest request)
        {
            var relationship = await _relationshipService.CreateRelationshipAsync(
                request.SourceNodeId, 
                request.TargetNodeId, 
                (RelationshipType)request.Type);
            return Ok(relationship);
        }
    }
}
