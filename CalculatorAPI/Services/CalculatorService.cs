using System;
using System.Threading.Tasks;
using CalculatorAPI.Data;
using CalculatorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CalculatorAPI.Services
{
    public class CalculatorService : ICalculatorService
    {
        private readonly CalculatorDbContext _context;

        public CalculatorService(CalculatorDbContext context)
        {
            _context = context;
        }

        public double Calculate(double number1, double number2, string operation)
        {
            return operation.ToLower() switch
            {
                "add" => number1 + number2,
                "subtract" => number1 - number2,
                "multiply" => number1 * number2,
                "divide" => number2 == 0 ? throw new ArgumentException("Cannot divide by zero.") : number1 / number2,
                _ => throw new ArgumentException("Invalid operation.")
            };
        }

        public async Task<double> CalculateAsync(double number1, double number2, string operation, bool useStoredNumber)
        {
            if (useStoredNumber)
            {
                var storedNumber = await _context.StoredNumbers.FirstOrDefaultAsync();
                number1 = storedNumber?.Number ?? 0; // Use stored number if available
            }

            double result = operation.ToLower() switch
            {
                "add" => number1 + number2,
                "subtract" => number1 - number2,
                "multiply" => number1 * number2,
                "divide" => number2 == 0 ? throw new ArgumentException("Cannot divide by zero.") : number1 / number2,
                _ => throw new ArgumentException("Invalid operation.")
            };

            await StoreNumberAsync(result); // Store the result for reuse
            return result;
        }

        public async Task StoreNumberAsync(double number)
        {
            var existing = await _context.StoredNumbers
                .OrderByDescending(s => s.Id) // Ensures the most recent record is selected
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.Number = number;
            }
            else
            {
                _context.StoredNumbers.Add(new StoredNumber { Number = number });
            }

            await _context.SaveChangesAsync();
        }

        public async Task ResetStoredNumberAsync()
        {
            var storedNumber = await _context.StoredNumbers.FirstOrDefaultAsync();
            if (storedNumber != null)
            {
                _context.StoredNumbers.Remove(storedNumber);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<double?> GetStoredNumberAsync()
        {
            var storedNumber = await _context.StoredNumbers.OrderByDescending(s => s.Id).FirstOrDefaultAsync();
            return storedNumber?.Number;
        }
    }
}
