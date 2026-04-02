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

        public RelationshipService(IRelationshipRepository relationshipRepository)
        {
            _relationshipRepository = relationshipRepository;
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
            return relationship;
        }

        public async Task<IEnumerable<Relationship>> GetRelationshipsAsync()
        {
            return await _relationshipRepository.GetAllAsync();
        }
    }
}