using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Patterns.FileExport;

public class JsonVisitor : IVisitor
{
    public string Export(IEnumerable<BankAccount> accounts, 
        IEnumerable<Category> categories, 
        IEnumerable<Operation> operations)
    {
        var exportData = new
        {
            Accounts = accounts,
            Categories = categories,
            Operations = operations
        };
            
        return System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }

    public void VisitFile(File file)
    {
        if (file is not JsonFile)
            throw new ArgumentException("File is not json file");
        System.IO.File.WriteAllText(
            AppDomain.CurrentDomain.BaseDirectory + "/exported/" + file.Name, file.Content);
    }
}