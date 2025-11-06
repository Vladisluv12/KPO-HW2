using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Patterns.FileImport;

public class Imported
{
    public List<BankAccount> Accounts { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Operation> Operations { get; set; } = new();
}