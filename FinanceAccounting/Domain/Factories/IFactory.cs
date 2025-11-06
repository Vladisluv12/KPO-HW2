using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Domain.Factories;

public interface IFactory
{
    BankAccount CreateBankAccount(string name, double initialBalance = 0);
    Category CreateCategory(CategoryType type, string name);
    Operation CreateOperation(CategoryType type, Guid bankAccountId, double amount, 
        DateTime date, Guid categoryId, string description = "");
}