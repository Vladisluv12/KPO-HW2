using KPO_DZ2.Domain.Services;
using KPO_DZ2.Patterns.FileImport;
using Spectre.Console;

namespace KPO_DZ2.UI;

public class ImportMenu(
    IBankAccFacade accountFacade,
    ICategoryFacade categoryFacade,
    IOperationFacade operationFacade) : IMenu
{
    public void Show()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Импорт данных[/]"));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите формат:")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Импорт из JSON",
                        "Импорт из CSV",
                        "Импорт из YAML",
                        "Назад"
                    }));

            switch (choice)
            {
                case "Импорт из JSON":
                    ImportFromJson();
                    break;
                case "Импорт из CSV":
                    ImportFromCsv();
                    break;
                case "Импорт из YAML":
                    ImportFromYaml();
                    break;
                case "Назад":
                    return;
            }
        }
    }

    private void ImportFromJson()
    {
        try
        {
            var filePath = AnsiConsole.Ask<string>("Введите путь к JSON файлу:");

            var importer = new JsonImporter(accountFacade, categoryFacade, operationFacade);
            var result = importer.Import(filePath);
            
            AnsiConsole.MarkupLine(result.Success 
                ? $"[green]Импорт успешен: {result.ImportedAccounts} счетов, {result.ImportedCategories} категорий, {result.ImportedOperations} операций[/]"
                : $"[red]Ошибка импорта: {result.ErrorMessage}[/]");

            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private void ImportFromCsv()
    {
        try
        {
            var filePath = AnsiConsole.Ask<string>("Введите путь к CSV файлу:");

            var importer = new CsvImporter(accountFacade, categoryFacade, operationFacade);
            var result = importer.Import(filePath);
            
            AnsiConsole.MarkupLine(result.Success 
                ? $"[green]Импорт успешен: {result.ImportedAccounts} счетов, {result.ImportedCategories} категорий, {result.ImportedOperations} операций[/]"
                : $"[red]Ошибка импорта: {result.ErrorMessage}[/]");

            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private void ImportFromYaml()
    {
        try
        {
            var filePath = AnsiConsole.Ask<string>("Введите путь к YAML файлу:");

            var importer = new YamlImporter(accountFacade, categoryFacade, operationFacade);
            var result = importer.Import(filePath);
            
            AnsiConsole.MarkupLine(result.Success 
                ? $"[green]Импорт успешен: {result.ImportedAccounts} счетов, {result.ImportedCategories} категорий, {result.ImportedOperations} операций[/]"
                : $"[red]Ошибка импорта: {result.ErrorMessage}[/]");

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