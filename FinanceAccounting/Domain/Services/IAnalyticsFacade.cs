namespace KPO_DZ2.Domain.Services;

public interface IAnalyticsFacade
{
    double CalculateBalanceDifference(DateTime startDate, DateTime endDate);
    Dictionary<string, double> GroupOperationsByCategory(DateTime startDate, DateTime endDate);
    Summary GetSummary(DateTime startDate, DateTime endDate);
}