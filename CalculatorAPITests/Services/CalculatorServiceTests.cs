using System.Threading.Tasks;
using CalculatorAPI.Services;
using CalculatorAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CalculatorAPITests.Services
{
    public class CalculatorServiceTests
    {
        private readonly CalculatorService _calculatorService;
        private readonly CalculatorDbContext _dbContext;
        private readonly Mock<ILogger<CalculatorService>> _mockLogger;

        public CalculatorServiceTests()
        {
            var options = new DbContextOptionsBuilder<CalculatorDbContext>()
                .UseInMemoryDatabase(databaseName: "TestCalculatorDB")
                .Options;

            _dbContext = new CalculatorDbContext(options);
            _mockLogger = new Mock<ILogger<CalculatorService>>(); // Mocking ILogger

            _calculatorService = new CalculatorService(_dbContext, _mockLogger.Object);
        }

        [Theory]
        [InlineData(3, 2, "add", 5)]
        [InlineData(5, 3, "subtract", 2)]
        [InlineData(4, 2, "multiply", 8)]
        [InlineData(10, 2, "divide", 5)]
        public async Task Calculate_ValidOperations_ReturnsCorrectResult(double num1, double num2, string operation, double expected)
        {
            double result = await _calculatorService.CalculateAsync(num1, num2, operation, false);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Calculate_DivideByZero_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<DivideByZeroException>(() => _calculatorService.CalculateAsync(5, 0, "divide", false));
        }

        [Fact]
        public async Task Calculate_InvalidOperation_ThrowsException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _calculatorService.CalculateAsync(5, 2, "modulus", false));
        }

        [Fact]
        public async Task StoreNumberAsync_SavesNumberSuccessfully()
        {
            await _calculatorService.StoreNumberAsync(20);
            var storedNumber = await _calculatorService.GetStoredNumberAsync();
            Assert.Equal(20, storedNumber);
        }

        [Fact]
        public async Task ResetStoredNumberAsync_ClearsStoredNumber()
        {
            await _calculatorService.StoreNumberAsync(15);
            await _calculatorService.ResetStoredNumberAsync();
            var storedNumber = await _calculatorService.GetStoredNumberAsync();
            Assert.Null(storedNumber);
        }
    }
}

