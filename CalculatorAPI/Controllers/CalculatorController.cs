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

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateAsync([FromBody] CalculationRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Operation))
                return BadRequest(new { Message = "Invalid input data." });

            try
            {
                double result = await _calculatorService.CalculateAsync(request.Number1, request.Number2, request.Operation, request.UseStoredNumber);
                return Ok(new { Result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpPost("store-number")]
        public async Task<IActionResult> StoreNumber([FromBody] double number)
        {
            if (double.IsNaN(number) || double.IsInfinity(number))
            {
                return BadRequest(new { Message = "Invalid number provided." });
            }

            try
            {
                await _calculatorService.StoreNumberAsync(number);
                return Ok(new { Message = "Number stored successfully", StoredNumber = number });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to store the number.", Details = ex.Message });
            }
        }

        [HttpGet("stored-number")]
        public async Task<IActionResult> GetStoredNumber()
        {
            try
            {
                var storedNumber = await _calculatorService.GetStoredNumberAsync();
                if (storedNumber == null)
                {
                    return NotFound(new { Message = "No stored number found." });
                }

                return Ok(new { StoredNumber = storedNumber });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to retrieve stored number.", Details = ex.Message });
            }
        }

        [HttpDelete("reset-stored-number")]
        public async Task<IActionResult> ResetStoredNumber()
        {
            try
            {
                await _calculatorService.ResetStoredNumberAsync();
                return Ok(new { Message = "Stored number reset successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to reset stored number.", Details = ex.Message });
            }
        }
    }
}
