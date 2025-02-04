namespace CalculatorShared.Models
{
    public class CalculationRequest
    {
        public double Number1 { get; set; }
        public double Number2 { get; set; }
        public string Operation { get; set; } = "add";
        public bool UseStoredNumber { get; set; } = false;
    }
}
