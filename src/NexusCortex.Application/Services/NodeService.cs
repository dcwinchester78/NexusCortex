using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusCortex.Application.Dtos;
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

        public async Task<NodeHierarchyDto?> GetNodeHierarchyAsync(Guid rootNodeId)
        {
            var flatNodes = (await _nodeRepository.GetHierarchyAsync(rootNodeId)).ToList();
            if (!flatNodes.Any()) return null;

            var lookup = flatNodes.ToDictionary(n => n.Id, n => new NodeHierarchyDto
            {
                Id = n.Id,
                Name = n.Name,
                Type = n.Type,
                CreatedAt = n.CreatedAt
            });

            NodeHierarchyDto? root = null;

            foreach (var flatNode in flatNodes)
            {
                if (flatNode.ParentId == null)
                {
                    root = lookup[flatNode.Id];
                }
                else if (lookup.TryGetValue(flatNode.ParentId.Value, out var parent))
                {
                    parent.Children.Add(lookup[flatNode.Id]);
                }
            }

            return root;
        }

        public async Task<IEnumerable<Node>> GetImpactedNodesAsync(Guid sourceNodeId)
        {
            return await _nodeRepository.GetImpactedNodesAsync(sourceNodeId);
        }
    }
}
