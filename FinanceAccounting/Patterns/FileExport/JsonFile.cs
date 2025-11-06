using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Patterns.FileExport;

public class JsonFile(
    string name, 
    IEnumerable<BankAccount> accounts, 
    IEnumerable<Category> categories, 
    IEnumerable<Operation> operations) : File(name)
{
    private IEnumerable<BankAccount> Accounts { get; } = accounts;
    private IEnumerable<Category> Categories { get; } = categories;
    private IEnumerable<Operation> Operations { get; } = operations;
    
    public override void Accept(IVisitor visitor)
    {
        if (visitor is not JsonVisitor)
        {
            throw new ArgumentException("Can accept only JsonVisitor");
        }
        Content = visitor.Export(Accounts, Categories, Operations);
        visitor.VisitFile(this);
    }
}