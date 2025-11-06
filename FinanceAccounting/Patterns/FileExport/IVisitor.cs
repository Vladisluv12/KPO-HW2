using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Patterns.FileExport;

public interface IVisitor
{
    string Export(
        IEnumerable<BankAccount> accounts,
        IEnumerable<Category> categories,
        IEnumerable<Operation> operations);
    void VisitFile(File file);
}