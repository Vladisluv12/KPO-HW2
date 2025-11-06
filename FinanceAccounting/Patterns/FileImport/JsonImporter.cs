using System.Text.Json;
using KPO_DZ2.Domain.Factories;
using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Services;

namespace KPO_DZ2.Patterns.TemplateMethod;

public class JsonImporter(
    IBankAccFacade accountFacade,
    ICategoryFacade categoryFacade,
    IOperationFacade operationFacade)
    : DataImporter
{
    protected override string ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"JSON file not found: {filePath}");

        return File.ReadAllText(filePath);
    }

    protected override Imported ParseData(string data)
    {
        try
        {

            var importData = JsonSerializer.Deserialize<Imported>(data, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            if (importData == null)
                throw new ArgumentException("Invalid JSON format");

            return new Imported
            {
                Accounts = importData.Accounts
                    .Select(a =>  new BankAccount(a.Id, a.Name, a.Balance))
                    .ToList(),
                Categories = importData.Categories
                    .Select(c => new Category(c.Id, c.Type, c.Name))
                    .ToList(),
                Operations = importData.Operations
                    .Select(o => 
                        new Operation(o.Id, o.Type, o.BankAccountId, o.Amount, o.Date, o.CategoryId, o.Description))
                    .ToList()
            };
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"JSON parsing error: {ex.Message}");
        }
    }

    protected override ImportInfo SaveData(Imported data)
    {
        var info = new ImportInfo();
        
        try
        {
            // Import accounts
            foreach (var account in data.Accounts)
            {
                // Check if account already exists
                try
                {
                    var existingAccount = accountFacade.GetAccount(account.Id);
                    if (existingAccount != null) continue;
                    // Create new account
                    accountFacade.CreateAccount(account.Name, account.Balance);
                    info.ImportedAccounts++;
                }
                catch
                {
                    // If account doesn't exist, create it
                    accountFacade.CreateAccount(account.Name, account.Balance);
                    info.ImportedAccounts++;
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
                    info.ImportedCategories++;
                }
                catch
                {
                    categoryFacade.CreateCategory(category.Type, category.Name);
                    info.ImportedCategories++;
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
                    info.ImportedOperations++;
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
                    info.ImportedOperations++;
                }
            }

            info.Success = true;
        }
        catch (Exception ex)
        {
            info.Success = false;
            info.ErrorMessage = $"Error saving data: {ex.Message}";
        }

        return info;
    }
}