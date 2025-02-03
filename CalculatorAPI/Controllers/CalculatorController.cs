using Microsoft.AspNetCore.Mvc;
using CalculatorAPI.Services;
using CalculatorShared.Models;

namespace CalculatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculatorController : ControllerBase
    {
        private readonly ICalculatorService _calculatorService;

        public CalculatorController(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        // [HttpPost("calculate")]
        // public IActionResult Calculate([FromBody] CalculationRequest request)
        // {
        //     if (request == null || string.IsNullOrWhiteSpace(request.Operation))
        //         return BadRequest(new { Message = "Invalid input data." });

        //     try
        //     {
        //         // Prevent division by zero
        //         if (request.Operation == "divide" && request.Number2 == 0)
        //         {
        //             return BadRequest(new { Message = "Error: Division by zero is not allowed." });
        //         }

        //         double result = _calculatorService.Calculate(request.Number1, request.Number2, request.Operation);
        //         return Ok(new { Result = result });
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return BadRequest(new { Message = ex.Message });
        //     }
        // }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateAsync([FromBody] CalculationRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Operation))
                return BadRequest("Invalid input data.");

            try
            {
                double result = await _calculatorService.CalculateAsync(request.Number1, request.Number2, request.Operation, request.UseStoredNumber);
                return Ok(new { Result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("store-number")]
        public async Task<IActionResult> StoreNumber([FromBody] double number)
        {
            await _calculatorService.StoreNumberAsync(number);
            return Ok(new { Message = "Number stored successfully", StoredNumber = number });
        }

        [HttpGet("stored-number")]
        public async Task<IActionResult> GetStoredNumber()
        {
            var storedNumber = await _calculatorService.GetStoredNumberAsync();
            return Ok(new { StoredNumber = storedNumber });
        }

        [HttpDelete("reset-stored-number")]
        public async Task<IActionResult> ResetStoredNumber()
        {
            await _calculatorService.ResetStoredNumberAsync();
            return Ok(new { Message = "Stored number reset successfully" });
        }
    }
}
