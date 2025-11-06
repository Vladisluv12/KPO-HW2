using System.Globalization;
using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Services;

namespace KPO_DZ2.Patterns.FileImport;


public class CsvImporter(IBankAccFacade accountFacade, ICategoryFacade categoryFacade, IOperationFacade operationFacade)
    : DataImporter
{
    protected override string ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"CSV file not found: {filePath}");

        return File.ReadAllText(filePath);
    }

    protected override Imported ParseData(string data)
    {
        var financialData = new Imported();
        var lines = data.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

        if (lines.Length == 0)
            throw new ArgumentException("CSV file is empty");

        var currentSection = "";
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            // Detect section headers
            if (trimmedLine.Equals("Accounts", StringComparison.OrdinalIgnoreCase))
            {
                currentSection = "Accounts";
                continue;
            }
            else if (trimmedLine.Equals("Categories", StringComparison.OrdinalIgnoreCase))
            {
                currentSection = "Categories";
                continue;
            }
            else if (trimmedLine.Equals("Operations", StringComparison.OrdinalIgnoreCase))
            {
                currentSection = "Operations";
                continue;
            }
            else if (trimmedLine.StartsWith("Id,", StringComparison.OrdinalIgnoreCase))
            {
                // Skip header rows
                continue;
            }

            // Parse data based on current section
            switch (currentSection)
            {
                case "Accounts":
                    ParseAccountLine(trimmedLine, financialData);
                    break;
                case "Categories":
                    ParseCategoryLine(trimmedLine, financialData);
                    break;
                case "Operations":
                    ParseOperationLine(trimmedLine, financialData);
                    break;
            }
        }

        return financialData;
    }

    private static void ParseAccountLine(string line, Imported financialData)
    {
        var parts = ParseCsvLine(line);
        if (parts.Length < 3) return;

        if (!Guid.TryParse(parts[0], out var id) ||
            !double.TryParse(parts[2], NumberStyles.Currency, CultureInfo.InvariantCulture, out var balance))
            return;
        var account = new BankAccount(id, parts[1], balance);
        financialData.Accounts.Add(account);
    }

    private static void ParseCategoryLine(string line, Imported financialData)
    {
        var parts = ParseCsvLine(line);
        if (parts.Length < 3) return;

        if (!Guid.TryParse(parts[0], out var id) ||
            !Enum.TryParse<CategoryType>(parts[1], true, out var type)) return;
        var category = new Category(id, type, parts[2]);
        financialData.Categories.Add(category);
    }

    private static void ParseOperationLine(string line, Imported financialData)
    {
        var parts = ParseCsvLine(line);
        if (parts.Length < 6) return;

        if (!Guid.TryParse(parts[0], out var id) ||
            !Enum.TryParse<CategoryType>(parts[1], true, out var type) ||
            !Guid.TryParse(parts[2], out var accountId) ||
            !double.TryParse(parts[3], NumberStyles.Currency, CultureInfo.InvariantCulture, 
                out var amount) ||
            !DateTime.TryParse(parts[4], out var date) ||
            !Guid.TryParse(parts[5], out var categoryId)) return;
        var description = parts.Length > 6 ? parts[6] : "";
        var operation = new Operation(id, type, accountId, amount, date, categoryId, description);
        financialData.Operations.Add(operation);
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var inQuotes = false;
        var currentField = "";

        foreach (var c in line)
        {
            switch (c)
            {
                case '"':
                    inQuotes = !inQuotes;
                    break;
                case ',' when !inQuotes:
                    result.Add(currentField);
                    currentField = "";
                    break;
                default:
                    currentField += c;
                    break;
            }
        }

        result.Add(currentField);
        return result.ToArray();
    }

    protected override ImportInfo SaveData(Imported data)
    {
        var result = new ImportInfo();
        
        try
        {
            // Import accounts
            foreach (var account in data.Accounts)
            {
                try
                {
                    var existingAccount = accountFacade.GetAccount(account.Id);
                    if (existingAccount != null) continue;
                    accountFacade.CreateAccount(account.Name, account.Balance);
                    result.ImportedAccounts++;
                }
                catch
                {
                    accountFacade.CreateAccount(account.Name, account.Balance);
                    result.ImportedAccounts++;
                }
            }

            // Import categories
            foreach (var category in data.Categories)
            {
                try
                {
                    var existingCategory = categoryFacade.GetCategory(category.Id);
                    if (existingCategory != null) continue;
                    categoryFacade.CreateCategory(category.Type, category.Name);
                    result.ImportedCategories++;
                }
                catch
                {
                    categoryFacade.CreateCategory(category.Type, category.Name);
                    result.ImportedCategories++;
                }
            }

            // Import operations
            foreach (var operation in data.Operations)
            {
                try
                {
                    var existingOperation = operationFacade.GetOperation(operation.Id);
                    if (existingOperation != null) continue;
                    operationFacade.CreateOperation(
                        operation.Type,
                        operation.BankAccountId,
                        operation.Amount,
                        operation.Date,
                        operation.CategoryId,
                        operation.Description);
                    result.ImportedOperations++;
                }
                catch
                {
                    operationFacade.CreateOperation(
                        operation.Type,
                        operation.BankAccountId,
                        operation.Amount,
                        operation.Date,
                        operation.CategoryId,
                        operation.Description);
                    result.ImportedOperations++;
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Error saving CSV data: {ex.Message}";
        }

        return result;
    }
}