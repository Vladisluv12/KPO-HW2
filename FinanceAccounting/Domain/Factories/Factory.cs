using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Domain.Factories;

public class Factory : IFactory
{
    public BankAccount CreateBankAccount(string name, double initialBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name cannot be empty");
            
        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative");

        return new BankAccount(Guid.NewGuid(), name, initialBalance);
    }

    public Category CreateCategory(CategoryType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty");

        return new Category(Guid.NewGuid(), type, name);
    }

    public Operation CreateOperation(CategoryType type, Guid bankAccountId, double amount, 
        DateTime date, Guid categoryId, string description = "")
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");

        return new Operation(Guid.NewGuid(), type, bankAccountId, amount, date, categoryId, description);
    }
}