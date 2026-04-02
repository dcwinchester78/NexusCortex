using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using NexusCortex.Application.Interfaces;
using NexusCortex.Domain;

namespace NexusCortex.Infrastructure.Repositories
{
    public class RelationshipRepository : IRelationshipRepository
    {
        private readonly IDbConnection _db;

        public RelationshipRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task InsertAsync(Relationship relationship)
        {
            const string sql = @"
                INSERT INTO Relationships (Id, SourceNodeId, TargetNodeId, Type)
                VALUES (@Id, @SourceNodeId, @TargetNodeId, @Type)";

            await _db.ExecuteAsync(sql, relationship);
        }

        public async Task<IEnumerable<Relationship>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Relationships";
            return await _db.QueryAsync<Relationship>(sql);
        }

        public async Task<IEnumerable<Relationship>> GetBySourceNodeIdAsync(Guid sourceNodeId)
        {
            const string sql = "SELECT * FROM Relationships WHERE SourceNodeId = @SourceNodeId";
            return await _db.QueryAsync<Relationship>(sql, new { SourceNodeId = sourceNodeId });
        }
    }
}