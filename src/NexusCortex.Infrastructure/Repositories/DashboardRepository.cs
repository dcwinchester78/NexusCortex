using System.Data;
using System.Threading.Tasks;
using Dapper;
using NexusCortex.Application.Dtos;
using NexusCortex.Application.Interfaces;
using NexusCortex.Domain;

namespace NexusCortex.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IDbConnection _db;

        public DashboardRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            const string sql = @"
                SELECT * FROM Nodes WHERE Type = 0;
                SELECT * FROM Nodes WHERE Type = 1 AND Status = 1;
                SELECT * FROM Nodes WHERE Type = 2 AND Status = 0;
            ";

            using var multi = await _db.QueryMultipleAsync(sql);
            
            var dashboard = new DashboardDto
            {
                Areas = await multi.ReadAsync<Node>(),
                ActiveProjects = await multi.ReadAsync<Node>(),
                PendingActions = await multi.ReadAsync<Node>()
            };

            return dashboard;
        }
    }
}
