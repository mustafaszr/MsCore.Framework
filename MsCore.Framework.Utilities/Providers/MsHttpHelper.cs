using MsCore.Framework.Utilities.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MsCore.Framework.Utilities.Providers
{
    public class MsHttpHelper : IMsHttpHelper
    {
        private readonly HttpClient _httpClient;

        public MsHttpHelper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string url, Dictionary<string, string>? headers = null, string? token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TResponse>(jsonString);
            if (result == null)
            {
                throw new Exception($"Failed to deserialize response. Response content: {jsonString}");
            }
            return result;
        }

        public async Task<TResponse> GetAsync<TResponse>(string url, Dictionary<string, string>? headers = null, string? token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TResponse>(jsonString);
            if (result == null)
            {
                throw new Exception($"Failed to deserialize response. Response content: {jsonString}");
            }
            return result;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null, string? token = null)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TResponse>(jsonString);
            if (result == null)
            {
                throw new Exception($"Failed to deserialize response. Response content: {jsonString}");
            }
            return result;
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string>? headers = null, string? token = null)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = content
            };

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TResponse>(jsonString);
            if (result == null)
            {
                throw new Exception($"Failed to deserialize response. Response content: {jsonString}");
            }
            return result;
        }
    }
}
