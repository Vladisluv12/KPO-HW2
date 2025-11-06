using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Domain.Services;

public interface IOperationFacade
{
    Operation CreateOperation(CategoryType type, Guid bankAccountId, double amount, 
        DateTime date, Guid categoryId, string description = "");
    Operation? GetOperation(Guid id);
    void UpdateOperationDescription(Guid id, string newDescription);
    void DeleteOperation(Guid id);
    IEnumerable<Operation> GetAllOperations();
    IEnumerable<Operation> GetOperationsByPeriod(DateTime startDate, DateTime endDate);
    IEnumerable<Operation> GetOperationsByCategory(Guid categoryId);
}