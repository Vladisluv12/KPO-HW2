using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Services;

namespace KPO_DZ2.Patterns.Command;

public class CreateOperationCommand(
    IOperationFacade operationFacade,
    CategoryType type,
    Guid accountId,
    double amount,
    DateTime date,
    Guid categoryId,
    string description = "")
    : ICommand<Operation>
{
    public Operation Execute() => operationFacade.CreateOperation(type, accountId, amount, date, categoryId, description);
}