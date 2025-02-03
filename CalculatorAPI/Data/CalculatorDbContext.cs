using Microsoft.EntityFrameworkCore;
using CalculatorAPI.Models;

namespace CalculatorAPI.Data
{
    public class CalculatorDbContext : DbContext
    {
        public CalculatorDbContext(DbContextOptions<CalculatorDbContext> options) : base(options) { }

        public DbSet<StoredNumber> StoredNumbers { get; set; }
    }
}
