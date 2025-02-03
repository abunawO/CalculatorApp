using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CalculatorUI.Services;
using CalculatorShared.Models;
using Moq;
using Moq.Protected;
using Xunit;

namespace CalculatorUITests.Services
{
    public class CalculatorClientTests
    {
        /// <summary>
        /// Creates an instance of CalculatorClient with a mock HTTP message handler.
        /// This allows unit testing of HTTP requests without making real API calls.
        /// </summary>
        private CalculatorClient CreateCalculatorClient(HttpMessageHandler handler)
        {
            // Create an HttpClient and set its BaseAddress to the API URL
            var httpClient = new HttpClient(handler) { BaseAddress = new System.Uri("http://localhost:5101/") };

            // Return a new CalculatorClient using the mocked HttpClient
            return new CalculatorClient(httpClient);
        }

        /// <summary>
        /// Unit test to verify that the CalculateAsync method returns the expected result.
        /// This test ensures that when the Calculator API responds with a valid result, the client correctly handles it.
        /// </summary>
        [Fact] // Marks this method as a unit test
        public async Task CalculateAsync_ReturnsExpectedResult()
        {
            // --------------- Arrange ---------------
            // Create a mock HTTP message handler to intercept API calls
            var mockHandler = new Mock<HttpMessageHandler>();

            // Define the expected response from the API
            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK, // Simulate a 200 OK response
                Content = JsonContent.Create(new { Result = 15.0 }) // Simulated response body
            };

            // Set up the mock handler to return the expected response for any HTTP request
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), // Matches any request
                    ItExpr.IsAny<CancellationToken>()   // Matches any cancellation token
                )
                .ReturnsAsync(expectedResponse); // Returns our simulated response

            // Create an instance of CalculatorClient with the mock handler
            var client = CreateCalculatorClient(mockHandler.Object);

            // Define a sample calculation request
            var request = new CalculationRequest { Number1 = 10, Number2 = 5, Operation = "add" };

            // --------------- Act ---------------
            // Call the CalculateAsync method and get the result
            var result = await client.CalculateAsync(request);

            // --------------- Assert ---------------
            // Verify that the result is not null
            Assert.NotNull(result);

            // Verify that the result matches the expected value
            Assert.Equal(15.0, result);
        }
    }
}
