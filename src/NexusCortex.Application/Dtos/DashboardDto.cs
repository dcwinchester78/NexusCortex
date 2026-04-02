using System.Collections.Generic;
using NexusCortex.Domain;

namespace NexusCortex.Application.Dtos
{
    public class DashboardDto
    {
        public IEnumerable<Node> Areas { get; set; } = new List<Node>();
        public IEnumerable<Node> ActiveProjects { get; set; } = new List<Node>();
        public IEnumerable<Node> PendingActions { get; set; } = new List<Node>();
    }
}
