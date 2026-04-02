using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NexusCortex.Application.Dtos;
using NexusCortex.Domain;

namespace NexusCortex.Web.Services
{
    public interface INodeApiClient
    {
        Task<IEnumerable<Node>?> GetNextBestActionsAsync(int limit = 5);
        Task<NodeHierarchyDto?> GetHierarchyAsync(Guid id);
        Task<IEnumerable<Node>?> GetImpactsAsync(Guid id);
    }

    public class NodeApiClient : INodeApiClient
    {
        private readonly HttpClient _httpClient;

        public NodeApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Node>?> GetNextBestActionsAsync(int limit = 5)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Node>>($"nodes/actions/next-best?limit={limit}");
        }

        public async Task<NodeHierarchyDto?> GetHierarchyAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<NodeHierarchyDto>($"nodes/{id}/hierarchy");
        }

        public async Task<IEnumerable<Node>?> GetImpactsAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Node>>($"nodes/{id}/impacts");
        }
    }
}
