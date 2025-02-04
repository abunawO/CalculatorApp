using System.Net;
using System.Net.Http.Json;
using CalculatorShared.Models;
using Microsoft.Extensions.Logging;

namespace CalculatorUI.Services
{
    public class CalculatorClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CalculatorClient> _logger;

        public CalculatorClient(HttpClient httpClient, ILogger<CalculatorClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri("http://localhost:5101/");
        }

        // Perform a calculation
        public async Task<double?> CalculateAsync(CalculationRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/calculator/calculate", request);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CalculationResponse>();
                    return result?.Result;
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                    _logger.LogWarning($"Calculation failed: {error?.Message}");
                    return null; // Return null instead of a string
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while performing calculation.");
                return null;
            }
        }

        // Store a number
        public async Task<bool> StoreNumberAsync(double number)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/calculator/store-number", new { Number = number });
                if (response.IsSuccessStatusCode)
                    return true;

                if (response.Content.Headers.ContentLength > 0)
                {
                    var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                    _logger.LogWarning($"Failed to store number: {error?.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while storing number.");
            }
            return false;
        }


        // Reset the stored number
        public async Task<bool> ResetStoredNumberAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync("api/calculator/reset-stored-number");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                _logger.LogWarning($"Failed to reset stored number: {error?.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting stored number.");
            }
            return false;
        }

        // Retrieve stored number
        public async Task<double?> GetStoredNumberAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/calculator/stored-number");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<StoredNumberResponse>();
                    return result?.StoredNumber;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("No stored number found.");
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                    _logger.LogWarning($"Failed to get stored number: {error?.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving stored number.");
            }
            return null;
        }
    }

    // Response models
    public class CalculationResponse
    {
        public double Result { get; set; }
    }

    public class StoredNumberResponse
    {
        public double? StoredNumber { get; set; }
    }

    public class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
