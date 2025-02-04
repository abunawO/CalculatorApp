using System;
using System.Threading.Tasks;
using CalculatorAPI.Data;
using CalculatorAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CalculatorAPI.Services
{
    public class CalculatorService : ICalculatorService
    {
        private readonly CalculatorDbContext _context;
        private readonly ILogger<CalculatorService> _logger;

        public CalculatorService(CalculatorDbContext context, ILogger<CalculatorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<double> CalculateAsync(double number1, double number2, string operation, bool useStoredNumber)
        {
            try
            {
                if (useStoredNumber)
                {
                    var storedNumber = await _context.StoredNumbers.OrderByDescending(s => s.Id).FirstOrDefaultAsync();
                    if (storedNumber != null)
                    {
                        number1 = storedNumber.Number;
                    }
                    else
                    {
                        throw new ArgumentException("No stored number available.");
                    }
                }

                double result = operation.ToLower() switch
                {
                    "add" => number1 + number2,
                    "subtract" => number1 - number2,
                    "multiply" => number1 * number2,
                    "divide" => number2 == 0 ? throw new DivideByZeroException("Cannot divide by zero.") : number1 / number2,
                    _ => throw new ArgumentException("Invalid operation.")
                };

                await StoreNumberAsync(result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CalculateAsync.");
                throw;
            }
        }

        public async Task StoreNumberAsync(double number)
        {
            try
            {
                var existing = await _context.StoredNumbers.OrderByDescending(s => s.Id).FirstOrDefaultAsync();
                if (existing != null)
                {
                    existing.Number = number;
                }
                else
                {
                    await _context.StoredNumbers.AddAsync(new StoredNumber { Number = number });
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in StoreNumberAsync.");
                throw;
            }
        }

        public async Task ResetStoredNumberAsync()
        {
            try
            {
                var storedNumbers = await _context.StoredNumbers.ToListAsync();
                if (storedNumbers.Any())
                {
                    _context.StoredNumbers.RemoveRange(storedNumbers);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ResetStoredNumberAsync.");
                throw;
            }
        }

        public async Task<double?> GetStoredNumberAsync()
        {
            try
            {
                var storedNumber = await _context.StoredNumbers.OrderByDescending(s => s.Id).FirstOrDefaultAsync();
                return storedNumber?.Number;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetStoredNumberAsync.");
                throw;
            }
        }
    }
}
