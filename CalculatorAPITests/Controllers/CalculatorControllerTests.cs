using System.Net;
using System.Threading.Tasks;
using CalculatorAPI.Controllers;
using CalculatorAPI.Services;
using CalculatorShared.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CalculatorAPITests.Controllers
{
    public class CalculatorControllerTests
    {
        private readonly CalculatorController _controller;
        private readonly Mock<ICalculatorService> _mockService;

        public CalculatorControllerTests()
        {
            _mockService = new Mock<ICalculatorService>();
            _controller = new CalculatorController(_mockService.Object);
        }

        [Fact]
        public async Task CalculateAsync_ValidRequest_ReturnsOkResult()
        {
            var request = new CalculationRequest { Number1 = 10, Number2 = 2, Operation = "add", UseStoredNumber = false };
            _mockService.Setup(s => s.CalculateAsync(request.Number1, request.Number2, request.Operation, request.UseStoredNumber))
                .ReturnsAsync(12);

            var result = await _controller.CalculateAsync(request) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task CalculateAsync_InvalidOperation_ReturnsBadRequest()
        {
            var request = new CalculationRequest { Number1 = 10, Number2 = 2, Operation = "modulus" };
            _mockService.Setup(s => s.CalculateAsync(request.Number1, request.Number2, request.Operation, false))
                .ThrowsAsync(new ArgumentException("Invalid operation."));

            var result = await _controller.CalculateAsync(request) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task StoreNumber_ValidInput_ReturnsOk()
        {
            var number = 10;
            _mockService.Setup(s => s.StoreNumberAsync(number)).Returns(Task.CompletedTask);

            var result = await _controller.StoreNumber(number) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task GetStoredNumber_NumberExists_ReturnsOk()
        {
            _mockService.Setup(s => s.GetStoredNumberAsync()).ReturnsAsync(15);

            var result = await _controller.GetStoredNumber() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task ResetStoredNumber_CallsServiceAndReturnsOk()
        {
            _mockService.Setup(s => s.ResetStoredNumberAsync()).Returns(Task.CompletedTask);

            var result = await _controller.ResetStoredNumber() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}
