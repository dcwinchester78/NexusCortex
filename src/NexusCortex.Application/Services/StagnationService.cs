using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NexusCortex.Application.Interfaces;
using NexusCortex.Domain;

namespace NexusCortex.Application.Services
{
    public class StagnationService : IStagnationService
    {
        private readonly INodeRepository _nodeRepository;

        public StagnationService(INodeRepository nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }

        public async Task<IEnumerable<Node>> GetStagnantNodesAsync(int daysThreshold = 7)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);
            return await _nodeRepository.GetStagnantNodesAsync(thresholdDate);
        }
    }
}
