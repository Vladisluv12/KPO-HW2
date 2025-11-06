namespace KPO_DZ2.Domain.Services;

public class Summary
{
        public double TotalIncome { get; set; }
        public double TotalExpenses { get; set; }
        public double BalanceDifference { get; set; }
        public Dictionary<string, double> CategoryBreakdown { get; set; } = new();
}