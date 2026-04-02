using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NexusCortex.Domain;

namespace NexusCortex.Application.Services
{
    public interface IStagnationService
    {
        Task<IEnumerable<Node>> GetStagnantNodesAsync(int daysThreshold = 7);
    }
}
