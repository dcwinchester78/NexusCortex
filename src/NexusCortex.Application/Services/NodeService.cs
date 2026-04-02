using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NexusCortex.Application.Interfaces;
using NexusCortex.Domain;

namespace NexusCortex.Application.Services
{
    public class NodeService : INodeService
    {
        private readonly INodeRepository _nodeRepository;

        public NodeService(INodeRepository nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }

        public async Task<Node> CreateNodeAsync(string name, NodeType type)
        {
            var node = new Node
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            await _nodeRepository.InsertAsync(node);
            return node;
        }

        public async Task<IEnumerable<Node>> GetNodesAsync()
        {
            return await _nodeRepository.GetAllAsync();
        }
    }
}