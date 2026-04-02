using System.Threading.Tasks;
using NexusCortex.Application.Dtos;
using NexusCortex.Application.Interfaces;

namespace NexusCortex.Application.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            return await _dashboardRepository.GetDashboardAsync();
        }
    }
}
