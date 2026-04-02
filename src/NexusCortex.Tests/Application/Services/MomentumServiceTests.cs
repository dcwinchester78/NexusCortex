using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NexusCortex.Application.Interfaces;
using NexusCortex.Application.Services;
using NexusCortex.Domain;
using Xunit;

namespace NexusCortex.Tests.Application.Services
{
    public class MomentumServiceTests
    {
        private readonly Mock<INodeRepository> _nodeRepoMock;
        private readonly Mock<IRelationshipRepository> _relRepoMock;
        private readonly MomentumService _momentumService;

        public MomentumServiceTests()
        {
            _nodeRepoMock = new Mock<INodeRepository>();
            _relRepoMock = new Mock<IRelationshipRepository>();
            _momentumService = new MomentumService(_nodeRepoMock.Object, _relRepoMock.Object);
        }

        [Fact]
        public async Task CalculateMomentumAsync_NodeDoesNotExist_ReturnsZero()
        {
            // Arrange
            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Node?)null);

            // Act
            var score = await _momentumService.CalculateMomentumAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(0, score);
        }

        [Fact]
        public async Task CalculateMomentumAsync_NoIncomingLinks_ReturnsZero()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            var targetNode = new Node { Id = nodeId, Type = NodeType.Project };
            
            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(nodeId)).ReturnsAsync(targetNode);
            _relRepoMock.Setup(repo => repo.GetByTargetNodeIdAsync(nodeId)).ReturnsAsync(new List<Relationship>());

            // Act
            var score = await _momentumService.CalculateMomentumAsync(nodeId);

            // Assert
            Assert.Equal(0, score);
            _nodeRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Node>(n => n.MomentumScore == 0)), Times.Once);
        }

        [Fact]
        public async Task CalculateMomentumAsync_WithPendingAction_ReturnsZero()
        {
            // Arrange
            var targetNodeId = Guid.NewGuid();
            var sourceNodeId = Guid.NewGuid();

            var targetNode = new Node { Id = targetNodeId, Type = NodeType.Project };
            var pendingAction = new Node { Id = sourceNodeId, Type = NodeType.Action, Status = NodeStatus.Pending };
            
            var relationship = new Relationship 
            { 
                SourceNodeId = sourceNodeId, 
                TargetNodeId = targetNodeId, 
                Type = RelationshipType.BelongsTo 
            };

            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(targetNodeId)).ReturnsAsync(targetNode);
            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(sourceNodeId)).ReturnsAsync(pendingAction);
            _relRepoMock.Setup(repo => repo.GetByTargetNodeIdAsync(targetNodeId)).ReturnsAsync(new List<Relationship> { relationship });

            // Act
            var score = await _momentumService.CalculateMomentumAsync(targetNodeId);

            // Assert
            Assert.Equal(0, score);
            _nodeRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Node>(n => n.MomentumScore == 0)), Times.Once);
        }

        [Fact]
        public async Task CalculateMomentumAsync_WithCompletedBelongsToAction_Returns10()
        {
            // Arrange
            var targetNodeId = Guid.NewGuid();
            var sourceNodeId = Guid.NewGuid();

            var targetNode = new Node { Id = targetNodeId, Type = NodeType.Project };
            var completedAction = new Node { Id = sourceNodeId, Type = NodeType.Action, Status = NodeStatus.Completed };
            
            var relationship = new Relationship 
            { 
                SourceNodeId = sourceNodeId, 
                TargetNodeId = targetNodeId, 
                Type = RelationshipType.BelongsTo 
            };

            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(targetNodeId)).ReturnsAsync(targetNode);
            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(sourceNodeId)).ReturnsAsync(completedAction);
            _relRepoMock.Setup(repo => repo.GetByTargetNodeIdAsync(targetNodeId)).ReturnsAsync(new List<Relationship> { relationship });

            // Act
            var score = await _momentumService.CalculateMomentumAsync(targetNodeId);

            // Assert
            Assert.Equal(10, score);
            _nodeRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Node>(n => n.MomentumScore == 10)), Times.Once);
        }

        [Fact]
        public async Task CalculateMomentumAsync_WithCompletedImpactsAction_Returns15()
        {
            // Arrange
            var targetNodeId = Guid.NewGuid();
            var sourceNodeId = Guid.NewGuid();

            var targetNode = new Node { Id = targetNodeId, Type = NodeType.Project };
            var completedAction = new Node { Id = sourceNodeId, Type = NodeType.Action, Status = NodeStatus.Completed };
            
            var relationship = new Relationship 
            { 
                SourceNodeId = sourceNodeId, 
                TargetNodeId = targetNodeId, 
                Type = RelationshipType.Impacts 
            };

            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(targetNodeId)).ReturnsAsync(targetNode);
            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(sourceNodeId)).ReturnsAsync(completedAction);
            _relRepoMock.Setup(repo => repo.GetByTargetNodeIdAsync(targetNodeId)).ReturnsAsync(new List<Relationship> { relationship });

            // Act
            var score = await _momentumService.CalculateMomentumAsync(targetNodeId);

            // Assert
            Assert.Equal(15, score);
            _nodeRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Node>(n => n.MomentumScore == 15)), Times.Once);
        }

        [Fact]
        public async Task CalculateMomentumAsync_WithMultipleActions_ReturnsSummedScore()
        {
            // Arrange
            var targetNodeId = Guid.NewGuid();
            var action1Id = Guid.NewGuid(); // Completed, BelongsTo -> 10
            var action2Id = Guid.NewGuid(); // Completed, Impacts -> 15
            var action3Id = Guid.NewGuid(); // Pending, BelongsTo -> 0

            var targetNode = new Node { Id = targetNodeId, Type = NodeType.Project };
            
            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(targetNodeId)).ReturnsAsync(targetNode);
            
            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(action1Id))
                .ReturnsAsync(new Node { Id = action1Id, Type = NodeType.Action, Status = NodeStatus.Completed });
            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(action2Id))
                .ReturnsAsync(new Node { Id = action2Id, Type = NodeType.Action, Status = NodeStatus.Completed });
            _nodeRepoMock.Setup(repo => repo.GetByIdAsync(action3Id))
                .ReturnsAsync(new Node { Id = action3Id, Type = NodeType.Action, Status = NodeStatus.Pending });

            var relationships = new List<Relationship>
            {
                new Relationship { SourceNodeId = action1Id, TargetNodeId = targetNodeId, Type = RelationshipType.BelongsTo },
                new Relationship { SourceNodeId = action2Id, TargetNodeId = targetNodeId, Type = RelationshipType.Impacts },
                new Relationship { SourceNodeId = action3Id, TargetNodeId = targetNodeId, Type = RelationshipType.BelongsTo }
            };

            _relRepoMock.Setup(repo => repo.GetByTargetNodeIdAsync(targetNodeId)).ReturnsAsync(relationships);

            // Act
            var score = await _momentumService.CalculateMomentumAsync(targetNodeId);

            // Assert
            Assert.Equal(25, score); // 10 + 15 + 0
            _nodeRepoMock.Verify(repo => repo.UpdateAsync(It.Is<Node>(n => n.MomentumScore == 25)), Times.Once);
        }

        [Fact]
        public async Task GetNextBestActionsAsync_SortsByLowestMomentum_And_HighImpacts()
        {
            // Arrange
            var actionHighPriorityId = Guid.NewGuid();
            var actionLowPriorityId = Guid.NewGuid();
            var targetLowMomentumId = Guid.NewGuid();
            var targetHighMomentumId = Guid.NewGuid();

            var actionHighPriority = new Node { Id = actionHighPriorityId, Type = NodeType.Action, Status = NodeStatus.Pending, Name = "High Priority" };
            var actionLowPriority = new Node { Id = actionLowPriorityId, Type = NodeType.Action, Status = NodeStatus.Pending, Name = "Low Priority" };
            
            var targetLowMomentum = new Node { Id = targetLowMomentumId, Type = NodeType.Area, MomentumScore = 0 }; // Low momentum, higher priority
            var targetHighMomentum = new Node { Id = targetHighMomentumId, Type = NodeType.Area, MomentumScore = 100 }; // High momentum, lower priority

            _nodeRepoMock.Setup(repo => repo.GetAllAsync(NodeType.Action, null, NodeStatus.Pending))
                .ReturnsAsync(new List<Node> { actionLowPriority, actionHighPriority });

            _nodeRepoMock.Setup(repo => repo.GetAllAsync(null, null, null))
                .ReturnsAsync(new List<Node> { actionLowPriority, actionHighPriority, targetLowMomentum, targetHighMomentum });

            var relationships = new List<Relationship>
            {
                new Relationship { SourceNodeId = actionHighPriorityId, TargetNodeId = targetLowMomentumId, Type = RelationshipType.Impacts },
                new Relationship { SourceNodeId = actionLowPriorityId, TargetNodeId = targetHighMomentumId, Type = RelationshipType.Impacts }
            };

            _relRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(relationships);

            // Act
            var result = (await _momentumService.GetNextBestActionsAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(actionHighPriorityId, result[0].Id); // High priority first because it impacts a node with 0 momentum (50 - 0 = 50 score)
            Assert.Equal(actionLowPriorityId, result[1].Id); // Low priority second because it impacts a node with 100 momentum (50 - 100 = -50 score)
        }
    }
}
