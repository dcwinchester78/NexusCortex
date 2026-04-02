using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
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
    }
}