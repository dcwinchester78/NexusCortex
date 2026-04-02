using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NexusCortex.Domain;

namespace NexusCortex.Application.Interfaces
{
    public interface INodeRepository
    {
        Task InsertAsync(Node node);
        Task<Node?> GetByIdAsync(Guid id);
        Task<IEnumerable<Node>> GetAllAsync();
    }
}