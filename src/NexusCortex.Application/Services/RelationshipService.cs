using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NexusCortex.Application.Interfaces;
using NexusCortex.Domain;

namespace NexusCortex.Application.Services
{
    public class RelationshipService : IRelationshipService
    {
        private readonly IRelationshipRepository _relationshipRepository;
        private readonly INodeRepository _nodeRepository;

        public RelationshipService(IRelationshipRepository relationshipRepository, INodeRepository nodeRepository)
        {
            _relationshipRepository = relationshipRepository;
            _nodeRepository = nodeRepository;
        }

        public async Task<Relationship> CreateRelationshipAsync(Guid sourceId, Guid targetId, RelationshipType type)
        {
            var relationship = new Relationship
            {
                Id = Guid.NewGuid(),
                SourceNodeId = sourceId,
                TargetNodeId = targetId,
                Type = type
            };

            await _relationshipRepository.InsertAsync(relationship);

            // Update activity recursively up the tree for the target node
            await _nodeRepository.UpdateActivityRecursivelyAsync(targetId, DateTime.UtcNow);

            return relationship;
        }

        public async Task<IEnumerable<Relationship>> GetRelationshipsAsync()
        {
            return await _relationshipRepository.GetAllAsync();
        }
    }
}