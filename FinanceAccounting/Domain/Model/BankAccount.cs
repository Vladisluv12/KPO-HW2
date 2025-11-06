namespace KPO_DZ2.Domain.Model;

public class BankAccount(Guid id, string name, double initialBalance = 0)
{
    public Guid Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public double Balance { get; private set; } = initialBalance;

    public void UpdateBalance(double amount)
    {
        Balance += amount;
    }
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Account name cannot be empty");
            
        Name = newName;
    }
}