namespace CalculatorAPI.Services
{
    public interface ICalculatorService
    {
        Task<double> CalculateAsync(double number1, double number2, string operation, bool useStoredNumber);
        Task StoreNumberAsync(double number);
        Task ResetStoredNumberAsync();
        Task<double?> GetStoredNumberAsync();
    }
}