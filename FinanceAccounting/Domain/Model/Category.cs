namespace KPO_DZ2.Domain.Model;

public enum CategoryType
{
    Income,
    Expense
}

public class Category(Guid id, CategoryType type, string name)
{
    public Guid Id { get; private set; } = id;
    public CategoryType Type { get; private set; } = type;
    public string Name { get; private set; } = name;
    
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Category name cannot be empty");
        
        Name = newName;
    }
}