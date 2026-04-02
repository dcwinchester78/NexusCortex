using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NexusCortex.Application.Dtos;
using NexusCortex.Domain;

namespace NexusCortex.Web.Services
{
    public interface IDashboardApiClient
    {
        Task<DashboardDto?> GetDashboardAsync();
        Task<IEnumerable<Node>?> GetStagnantNodesAsync(int days = 7);
    }

    public class DashboardApiClient : IDashboardApiClient
    {
        private readonly HttpClient _httpClient;

        public DashboardApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardDto?> GetDashboardAsync()
        {
            return await _httpClient.GetFromJsonAsync<DashboardDto>("dashboard");
        }

        public async Task<IEnumerable<Node>?> GetStagnantNodesAsync(int days = 7)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Node>>($"dashboard/stagnant?days={days}");
        }
    }
}
