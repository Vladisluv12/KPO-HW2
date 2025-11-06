using KPO_DZ2.Domain.Services;

namespace KPO_DZ2.UI;


using Spectre.Console;

public class MainMenu(
    IBankAccFacade bankAccFacade,
    ICategoryFacade categoryFacade,
    IOperationFacade operationFacade,
    IAnalyticsFacade analyticsFacade)
    : IMenu
{
    private readonly BankAccMenu _accountsMenu = new(bankAccFacade);
    private readonly CategoriesMenu _categoriesMenu = new(categoryFacade);
    private readonly OperationsMenu _operationsMenu = new(operationFacade, bankAccFacade, categoryFacade);
    private readonly AnalyticsMenu _analyticsMenu = new(analyticsFacade);
    private readonly ImportMenu _importMenu = new(bankAccFacade, categoryFacade, operationFacade);
    private readonly ExportMenu _exportMenu = new(bankAccFacade, categoryFacade, operationFacade);

    public void Show()
    {
        while (true)
        {
            AnsiConsole.Clear();
            ShowHeader();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Выберите раздел:[/]")
                    .PageSize(10)
                    .AddChoices("Управление счетами", 
                        "Управление категориями", 
                        "Управление операциями", 
                        "Аналитика и отчеты", 
                        "Импорт данных", 
                        "Экспорт данных", 
                        "Выход"));

            switch (choice)
            {
                case "Управление счетами":
                    _accountsMenu.Show();
                    break;
                case "Управление категориями":
                    _categoriesMenu.Show();
                    break;
                case "Управление операциями":
                    _operationsMenu.Show();
                    break;
                case "Аналитика и отчеты":
                    _analyticsMenu.Show();
                    break;
                case "Импорт данных":
                    _importMenu.Show();
                    break;
                case "Экспорт данных":
                    _exportMenu.Show();
                    break;
                case "Выход":
                    if (AnsiConsole.Confirm("[red]Вы уверены, что хотите выйти?[/]"))
                    {
                        AnsiConsole.MarkupLine("[green]До свидания![/]");
                        return;
                    }
                    break;
            }
        }
    }

    private void ShowHeader()
    {
        var figletText = new FigletText("ВШЭ-Банк").Color(Color.Blue);
        AnsiConsole.Write(
            figletText);
        
        AnsiConsole.Write(
            new Markup("[bold green]Модуль учета финансов[/]\n"));

        AnsiConsole.Write(new Rule("[yellow]Главное меню[/]"));
    }
}