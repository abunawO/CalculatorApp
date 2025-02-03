using System.Net.Http.Json;
using CalculatorShared.Models;

namespace CalculatorUI.Services
{
    public class CalculatorClient
    {
        private readonly HttpClient _httpClient;

        public CalculatorClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5101/"); // Ensure correct API base URL
        }

        public async Task<double?> CalculateAsync(CalculationRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/calculator/calculate", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CalculationResponse>();
                return result?.Result;
            }
            return null;
        }

        // Store a number
        public async Task StoreNumberAsync(double number)
        {
            var response = await _httpClient.PostAsJsonAsync("api/calculator/store-number", number);
            response.EnsureSuccessStatusCode();
        }

        // Reset the stored number
        public async Task ResetStoredNumberAsync()
        {
            var response = await _httpClient.DeleteAsync("api/calculator/reset-stored-number");
            response.EnsureSuccessStatusCode();
        }

        public async Task<double?> GetStoredNumberAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<double?>("api/calculator/stored-number");
            return response;
        }
    }

    public class CalculationResponse
    {
        public double Result { get; set; }
    }
}
