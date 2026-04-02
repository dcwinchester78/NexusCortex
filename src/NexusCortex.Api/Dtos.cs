using System;

namespace NexusCortex.Api.Dtos
{
    public class CreateNodeRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Type { get; set; }
    }

    public class CreateRelationshipRequest
    {
        public Guid SourceNodeId { get; set; }
        public Guid TargetNodeId { get; set; }
        public int Type { get; set; }
    }
}
