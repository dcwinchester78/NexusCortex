using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NexusCortex.Domain;

namespace NexusCortex.Application.Services
{
    public interface IMomentumService
    {
        Task<decimal> CalculateMomentumAsync(Guid nodeId);
        Task<IEnumerable<Node>> GetNextBestActionsAsync(int limit = 5);
    }
}
