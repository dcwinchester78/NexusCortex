using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NexusCortex.Domain;

namespace NexusCortex.Application.Services
{
    public interface IRelationshipService
    {
        Task<Relationship> CreateRelationshipAsync(Guid sourceId, Guid targetId, RelationshipType type);
        Task<IEnumerable<Relationship>> GetRelationshipsAsync();
    }
}