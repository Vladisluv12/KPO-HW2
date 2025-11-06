using KPO_DZ2.Domain.Factories;
using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;

namespace KPO_DZ2.Domain.Services;

public class OperationFacade(
    IFactory factory,
    IOperationRepository repository,
    IBankAccRepository accountRepository) : IOperationFacade
{
    public Operation CreateOperation(CategoryType type, Guid bankAccountId, double amount, 
        DateTime date, Guid categoryId, string description = "")
    {
        var operation = factory.CreateOperation(type, bankAccountId, amount, date, categoryId, description);
            
        // Update account balance
        var account = accountRepository.GetById(bankAccountId);
        if (account == null)
        {
            throw new ArgumentException("Account not found");
        }
        account.UpdateBalance(type == CategoryType.Income ? amount : -amount);
        accountRepository.Update(account);
            
        repository.Add(operation);
        return operation;
    }

    public Operation? GetOperation(Guid id) => repository.GetById(id);

    public void UpdateOperationDescription(Guid id, string newDescription)
    {
        var operation = repository.GetById(id);
        if (operation == null) 
            return;
        operation.UpdateDescription(newDescription);
        repository.Update(operation);
    }

    public void DeleteOperation(Guid id) => repository.Remove(id);

    public IEnumerable<Operation> GetAllOperations() => repository.GetAll();

    public IEnumerable<Operation> GetOperationsByPeriod(DateTime startDate, DateTime endDate)
        => repository.GetByPeriod(startDate, endDate);

    public IEnumerable<Operation> GetOperationsByCategory(Guid categoryId)
        => repository.GetByCategory(categoryId);
}