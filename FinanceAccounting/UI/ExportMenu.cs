using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Services;
using KPO_DZ2.Patterns.FileExport;
using Spectre.Console;

namespace KPO_DZ2.UI;

public class ExportMenu(
    IBankAccFacade accountFacade,
    ICategoryFacade categoryFacade,
    IOperationFacade operationFacade) : IMenu
{
    public void Show()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Экспорт данных[/]"));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите формат:")
                    .PageSize(10)
                    .AddChoices(
                        "Экспорт в JSON",
                        "Экспорт в CSV - пока нет, tbd",
                        "Экспорт в YAML - пока нет, tbd",
                        "Назад"
                    ));

            switch (choice)
            {
                case "Экспорт в JSON":
                    ExportToJson();
                    break;
                case "Назад":
                    return;
            }
        }
    }

    private void ExportToJson()
    {
        try
        {
            var accounts = accountFacade.GetAllAccounts();
            var categories = categoryFacade.GetAllCategories();
            var operations = operationFacade.GetAllOperations();

            JsonVisitor jsonExporter = new JsonVisitor();
            var bankAccs = accounts as BankAccount[] ?? accounts.ToArray();
            var bankCategories = categories as Category[] ?? categories.ToArray();
            var operations1 = operations as Operation[] ?? operations.ToArray();
            var file = new JsonFile("exported_data.json", bankAccs, bankCategories, operations1);
            file.Accept(jsonExporter);
            AnsiConsole.MarkupLine("[green]Данные экспортированы в JSON формат в папку 'exported'[/]");

            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private static void WaitForContinue()
    {
        AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу для продолжения...[/]");
        Console.ReadKey();
    }
}