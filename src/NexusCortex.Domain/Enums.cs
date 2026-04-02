namespace NexusCortex.Domain
{
    public enum NodeStatus
    {
        Pending = 0,
        Active = 1,
        Completed = 2
    }

    public enum NodeType
    {
        Area = 0,
        Project = 1,
        Action = 2,
        Person = 3,
        Knowledge = 4
    }

    public enum RelationshipType
    {
        BelongsTo = 0,
        Impacts = 1,
        RelatedTo = 2
    }
}
