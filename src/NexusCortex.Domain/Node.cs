using System;

namespace NexusCortex.Domain
{
    public class Node
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public NodeType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public NodeStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal MomentumScore { get; set; }
        public DateTime LastActivityAt { get; set; }
    }
}
