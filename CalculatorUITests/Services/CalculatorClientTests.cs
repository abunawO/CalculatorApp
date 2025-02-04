using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CalculatorUI.Services;
using CalculatorShared.Models;
using Moq;
using Moq.Protected;
using Xunit;
using Microsoft.Extensions.Logging;

namespace CalculatorUITests.Services
{
    public class CalculatorClientTests
    {
        private readonly CalculatorClient _client;
        private readonly Mock<HttpMessageHandler> _mockHttp;
        private readonly HttpClient _httpClient;

        public CalculatorClientTests()
        {
            _mockHttp = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttp.Object) { BaseAddress = new Uri("http://localhost:5101/") };
            var loggerMock = new Mock<ILogger<CalculatorClient>>();

            _client = new CalculatorClient(_httpClient, loggerMock.Object);
        }

        [Fact]
        public async Task CalculateAsync_ValidRequest_ReturnsResult()
        {
            var request = new CalculationRequest { Number1 = 5, Number2 = 3, Operation = "add" };
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new CalculationResponse { Result = 8 })
            };

            _mockHttp.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _client.CalculateAsync(request);

            Assert.NotNull(result);
            Assert.Equal(8, result);
        }

        [Fact]
        public async Task StoreNumberAsync_ValidInput_ReturnsTrue()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            _mockHttp.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _client.StoreNumberAsync(10);

            Assert.True(result);
        }

        [Fact]
        public async Task GetStoredNumberAsync_NumberExists_ReturnsNumber()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new StoredNumberResponse { StoredNumber = 20 })
            };

            _mockHttp.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _client.GetStoredNumberAsync();

            Assert.NotNull(result);
            Assert.Equal(20, result);
        }
    }
}
