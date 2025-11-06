using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;

namespace KPO_DZ2.Domain.Services;

public class AnalyticsFacade(
    Guid bankAccId, 
    IOperationRepository operationRepository, 
    ICategoryRepository categoryRepository) : IAnalyticsFacade
{
    public double CalculateBalanceDifference(DateTime startDate, DateTime endDate)
    {
        var operations = operationRepository
            .GetByPeriod(startDate, endDate)
            .Where(o => o.BankAccountId == bankAccId)
            .ToList();
        var income = operations
            .Where(o => o.Type == CategoryType.Income)
            .Sum(o => o.Amount);
        var expenses = operations
            .Where(o => o.Type == CategoryType.Expense)
            .Sum(o => o.Amount);
        return income - expenses;
    }

    public Dictionary<string, double> GroupOperationsByCategory(DateTime startDate, DateTime endDate)
    {
        var operations = operationRepository
            .GetByPeriod(startDate, endDate)
            .Where(o => o.BankAccountId == bankAccId);
        var categories = categoryRepository
            .GetAll()
            .ToDictionary(c => c.Id, c => c.Name);
        
        return operations
            .GroupBy(o => o.CategoryId)
            .ToDictionary(
                g => categories.GetValueOrDefault(g.Key, "Unknown"),
                g => g.Sum(o => o.Type == CategoryType.Income ? o.Amount : -o.Amount)
            );
    }

    public Summary GetSummary(DateTime startDate, DateTime endDate)
    {
        var operations = operationRepository
            .GetByPeriod(startDate, endDate)
            .Where(o => o.BankAccountId == bankAccId)
            .ToList();
        
        return new Summary
        {
            TotalIncome = operations.Where(o => o.Type == CategoryType.Income).Sum(o => o.Amount),
            TotalExpenses = operations.Where(o => o.Type == CategoryType.Expense).Sum(o => o.Amount),
            BalanceDifference = CalculateBalanceDifference(startDate, endDate),
            CategoryBreakdown = GroupOperationsByCategory(startDate, endDate)
        };
    }
}