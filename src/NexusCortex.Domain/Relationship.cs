using System;

namespace NexusCortex.Domain
{
    public class Relationship
    {
        public Guid Id { get; set; }
        public Guid SourceNodeId { get; set; }
        public Guid TargetNodeId { get; set; }
        public RelationshipType Type { get; set; }
    }
}
