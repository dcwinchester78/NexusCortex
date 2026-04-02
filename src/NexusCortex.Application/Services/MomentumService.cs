using System;
using System.Linq;
using System.Threading.Tasks;
using NexusCortex.Application.Interfaces;
using NexusCortex.Domain;

namespace NexusCortex.Application.Services
{
    public class MomentumService : IMomentumService
    {
        private readonly INodeRepository _nodeRepository;
        private readonly IRelationshipRepository _relationshipRepository;

        public MomentumService(INodeRepository nodeRepository, IRelationshipRepository relationshipRepository)
        {
            _nodeRepository = nodeRepository;
            _relationshipRepository = relationshipRepository;
        }

        public async Task<decimal> CalculateMomentumAsync(Guid nodeId)
        {
            var node = await _nodeRepository.GetByIdAsync(nodeId);
            if (node == null) return 0;

            var incomingLinks = await _relationshipRepository.GetByTargetNodeIdAsync(nodeId);

            decimal score = 0;
            foreach (var link in incomingLinks)
            {
                var sourceNode = await _nodeRepository.GetByIdAsync(link.SourceNodeId);
                
                // Add momentum if an incoming node is a Completed Action
                if (sourceNode != null && sourceNode.Type == NodeType.Action && sourceNode.Status == NodeStatus.Completed)
                {
                    // BelongsTo might be worth 10 points, Impacts might be worth 15
                    score += link.Type == RelationshipType.Impacts ? 15 : 10;
                }
            }

            node.MomentumScore = score;
            await _nodeRepository.UpdateAsync(node);

            return score;
        }
    }
}
