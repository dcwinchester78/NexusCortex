using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using NexusCortex.Application.Dtos;
using NexusCortex.Application.Interfaces;
using NexusCortex.Domain;

namespace NexusCortex.Infrastructure.Repositories
{
    public class NodeRepository : INodeRepository
    {
        private readonly IDbConnection _db;

        public NodeRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task InsertAsync(Node node)
        {
            const string sql = @"
                INSERT INTO Nodes (Id, Name, Type, Status, DueDate, MomentumScore, CreatedAt)
                VALUES (@Id, @Name, @Type, @Status, @DueDate, @MomentumScore, @CreatedAt)";

            await _db.ExecuteAsync(sql, node);
        }

        public async Task UpdateAsync(Node node)
        {
            const string sql = @"
                UPDATE Nodes 
                SET Name = @Name, Type = @Type, Status = @Status, DueDate = @DueDate, MomentumScore = @MomentumScore
                WHERE Id = @Id";

            await _db.ExecuteAsync(sql, node);
        }

        public async Task<Node?> GetByIdAsync(Guid id)
        {
            const string sql = "SELECT * FROM Nodes WHERE Id = @Id";
            return await _db.QuerySingleOrDefaultAsync<Node>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Node>> GetAllAsync(NodeType? type = null, Guid? parentId = null, NodeStatus? status = null)
        {
            var sql = "SELECT n.* FROM Nodes n ";
            if (parentId.HasValue)
            {
                sql += "INNER JOIN Relationships r ON n.Id = r.SourceNodeId AND r.Type = 0 AND r.TargetNodeId = @ParentId ";
            }
            sql += "WHERE 1=1 ";

            if (type.HasValue) sql += "AND n.Type = @Type ";
            if (status.HasValue) sql += "AND n.Status = @Status ";

            return await _db.QueryAsync<Node>(sql, new { Type = type, ParentId = parentId, Status = status });
        }

        public async Task<IEnumerable<Node>> GetTodayActionsAsync()
        {
            const string sql = @"
                SELECT * FROM Nodes 
                WHERE Type = 2 
                AND DueDate >= CAST(GETUTCDATE() AS DATE) 
                AND DueDate < CAST(DATEADD(day, 1, GETUTCDATE()) AS DATE)";

            return await _db.QueryAsync<Node>(sql);
        }

        public async Task<IEnumerable<NodeHierarchyFlatDto>> GetHierarchyAsync(Guid rootNodeId)
        {
            const string sql = @"
                WITH NodeCTE AS (
                    SELECT n.Id, n.Name, n.Type, n.CreatedAt, n.Status, n.DueDate, n.MomentumScore, n.LastActivityAt, CAST(NULL AS UNIQUEIDENTIFIER) as ParentId
                    FROM Nodes n
                    WHERE n.Id = @RootNodeId

                    UNION ALL

                    SELECT n.Id, n.Name, n.Type, n.CreatedAt, n.Status, n.DueDate, n.MomentumScore, n.LastActivityAt, r.TargetNodeId as ParentId
                    FROM Nodes n
                    INNER JOIN Relationships r ON n.Id = r.SourceNodeId
                    INNER JOIN NodeCTE c ON r.TargetNodeId = c.Id
                    WHERE r.Type = 0
                )
                SELECT * FROM NodeCTE;";

            return await _db.QueryAsync<NodeHierarchyFlatDto>(sql, new { RootNodeId = rootNodeId });
        }

        public async Task<IEnumerable<Node>> GetImpactedNodesAsync(Guid sourceNodeId)
        {
            const string sql = @"
                SELECT n.*
                FROM Nodes n
                INNER JOIN Relationships r ON n.Id = r.TargetNodeId
                WHERE r.SourceNodeId = @SourceNodeId AND r.Type = 1;";

            return await _db.QueryAsync<Node>(sql, new { SourceNodeId = sourceNodeId });
        }

        public async Task UpdateActivityRecursivelyAsync(Guid nodeId, DateTime activityDate)
        {
            const string sql = @"
                WITH ParentCTE AS (
                    SELECT TargetNodeId as ParentId FROM Relationships WHERE SourceNodeId = @NodeId AND Type = 0
                    UNION ALL
                    SELECT r.TargetNodeId FROM Relationships r INNER JOIN ParentCTE p ON r.SourceNodeId = p.ParentId WHERE r.Type = 0
                )
                UPDATE Nodes 
                SET LastActivityAt = @ActivityDate 
                WHERE Id IN (SELECT ParentId FROM ParentCTE) OR Id = @NodeId;";

            await _db.ExecuteAsync(sql, new { NodeId = nodeId, ActivityDate = activityDate });
        }

        public async Task<IEnumerable<Node>> GetStagnantNodesAsync(DateTime threshold)
        {
            const string sql = "SELECT * FROM Nodes WHERE Type IN (0, 1) AND LastActivityAt < @Threshold";
            return await _db.QueryAsync<Node>(sql, new { Threshold = threshold });
        }
    }
}
