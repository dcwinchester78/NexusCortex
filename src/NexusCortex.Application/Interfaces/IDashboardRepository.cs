using System.Threading.Tasks;
using NexusCortex.Application.Dtos;

namespace NexusCortex.Application.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardDto> GetDashboardAsync();
    }
}