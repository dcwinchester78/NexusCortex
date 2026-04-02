using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NexusCortex.Domain;

namespace NexusCortex.Application.Services
{
    public interface INodeService
    {
        Task<Node> CreateNodeAsync(string name, NodeType type);
        Task<IEnumerable<Node>> GetNodesAsync();
    }
}