using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NexusCortex.Domain;

namespace NexusCortex.Application.Interfaces
{
    public interface IRelationshipRepository
    {
        Task InsertAsync(Relationship relationship);
        Task<IEnumerable<Relationship>> GetAllAsync();
        Task<IEnumerable<Relationship>> GetBySourceNodeIdAsync(Guid sourceNodeId);
    }
}