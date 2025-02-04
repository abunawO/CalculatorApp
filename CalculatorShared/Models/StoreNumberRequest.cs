namespace CalculatorShared.Models
{
    public class StoreNumberRequest
    {
        public double Number { get; set; }
        public string Source { get; set; } = "Custom";
    }
}