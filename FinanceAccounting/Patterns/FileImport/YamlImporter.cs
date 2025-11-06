using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Services;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace KPO_DZ2.Patterns.FileImport;

public class YamlImporter(
    IBankAccFacade accountFacade,
    ICategoryFacade categoryFacade,
    IOperationFacade operationFacade)
    : DataImporter
{
    protected override string ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"YAML file not found: {filePath}");

        return File.ReadAllText(filePath);
    }

    protected override Imported ParseData(string data)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            var yamlData = deserializer.Deserialize<Imported>(data);
            
            if (yamlData == null)
                throw new ArgumentException("Invalid YAML format");

            return new Imported
            {
                Accounts = yamlData.Accounts
                    .Select(a => new BankAccount(a.Id, a.Name, a.Balance))
                    .ToList(),
                Categories = yamlData.Categories
                    .Select(c => new Category(c.Id, c.Type, c.Name))
                    .ToList(),
                Operations = yamlData.Operations
                    .Select(o => 
                        new Operation(o.Id, o.Type, o.BankAccountId, o.Amount, o.Date, o.CategoryId, o.Description))
                    .ToList() 
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"YAML parsing error: {ex.Message}");
        }
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
            result.ErrorMessage = $"Error saving YAML data: {ex.Message}";
        }

        return result;
    }
}