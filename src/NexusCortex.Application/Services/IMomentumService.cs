using System;
using System.Threading.Tasks;

namespace NexusCortex.Application.Services
{
    public interface IMomentumService
    {
        Task<decimal> CalculateMomentumAsync(Guid nodeId);
    }
}
