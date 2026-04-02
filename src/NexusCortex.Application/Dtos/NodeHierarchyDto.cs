using System;
using System.Collections.Generic;
using NexusCortex.Domain;

namespace NexusCortex.Application.Dtos
{
    public class NodeHierarchyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public NodeType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public NodeStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal MomentumScore { get; set; }
        public List<NodeHierarchyDto> Children { get; set; } = new();
    }
}
