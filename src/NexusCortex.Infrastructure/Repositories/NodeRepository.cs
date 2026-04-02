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
                INSERT INTO Nodes (Id, Name, Type, CreatedAt)
                VALUES (@Id, @Name, @Type, @CreatedAt)";

            await _db.ExecuteAsync(sql, node);
        }

        public async Task<Node?> GetByIdAsync(Guid id)
        {
            const string sql = "SELECT * FROM Nodes WHERE Id = @Id";
            return await _db.QuerySingleOrDefaultAsync<Node>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Node>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Nodes";
            return await _db.QueryAsync<Node>(sql);
        }

        public async Task<IEnumerable<NodeHierarchyFlatDto>> GetHierarchyAsync(Guid rootNodeId)
        {
            const string sql = @"
                WITH NodeCTE AS (
                    SELECT n.Id, n.Name, n.Type, n.CreatedAt, CAST(NULL AS UNIQUEIDENTIFIER) as ParentId
                    FROM Nodes n
                    WHERE n.Id = @RootNodeId

                    UNION ALL

                    SELECT n.Id, n.Name, n.Type, n.CreatedAt, r.TargetNodeId as ParentId
                    FROM Nodes n
                    INNER JOIN Relationships r ON n.Id = r.SourceNodeId
                    INNER JOIN NodeCTE c ON r.TargetNodeId = c.Id
                    WHERE r.Type = 0
                )
                SELECT * FROM NodeCTE;";

            return await _db.QueryAsync<NodeHierarchyFlatDto>(sql, new { RootNodeId = rootNodeId });
        }
    }
}
