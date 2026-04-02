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

        public async Task<IEnumerable<Node>> GetNextBestActionsAsync(int limit = 5)
        {
            var pendingActions = (await _nodeRepository.GetAllAsync(NodeType.Action, null, NodeStatus.Pending)).ToList();
            if (!pendingActions.Any()) return new List<Node>();

            var allRelationships = await _relationshipRepository.GetAllAsync();
            var allNodes = await _nodeRepository.GetAllAsync();
            var nodeDict = allNodes.ToDictionary(n => n.Id);

            var actionScores = new Dictionary<Guid, decimal>();

            foreach (var action in pendingActions)
            {
                decimal priorityScore = 0;
                var links = allRelationships.Where(r => r.SourceNodeId == action.Id).ToList();

                foreach (var link in links)
                {
                    if (nodeDict.TryGetValue(link.TargetNodeId, out var targetNode))
                    {
                        // Priority algorithm: Base points minus target's momentum.
                        // Actions connected to nodes with lower momentum get higher priority.
                        decimal basePoints = link.Type == RelationshipType.Impacts ? 50m : 20m;
                        decimal scoreContribution = basePoints - targetNode.MomentumScore;
                        
                        // If it belongs to a Project, also factor in the Area's momentum.
                        if (targetNode.Type == NodeType.Project)
                        {
                            var projectLinks = allRelationships.Where(r => r.SourceNodeId == targetNode.Id && r.Type == RelationshipType.BelongsTo).ToList();
                            foreach (var pLink in projectLinks)
                            {
                                if (nodeDict.TryGetValue(pLink.TargetNodeId, out var areaNode))
                                {
                                    scoreContribution += (30m - areaNode.MomentumScore);
                                }
                            }
                        }

                        priorityScore += scoreContribution;
                    }
                }
                
                actionScores[action.Id] = priorityScore;
            }

            return pendingActions
                .OrderByDescending(a => actionScores.TryGetValue(a.Id, out var score) ? score : 0)
                .Take(limit);
        }
    }
}
