namespace KPO_DZ2.Domain.Model;

public class Operation(
    Guid id,
    CategoryType type,
    Guid bankAccountId,
    double amount,
    DateTime date,
    Guid categoryId,
    string description = "")
{
    public Guid Id { get; private set; } = id;
    public CategoryType Type { get; private set; } = type;
    public Guid BankAccountId { get; private set; } = bankAccountId;
    public double Amount { get; private set; } = amount;
    public DateTime Date { get; private set; } = date;
    public Guid CategoryId { get; private set; } = categoryId;
    public string Description { get; private set; } = description;

    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be empty.");
        }
        Description = description;
    }
}