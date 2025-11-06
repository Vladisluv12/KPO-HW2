using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Services;
using KPO_DZ2.Patterns.Command;
using KPO_DZ2.Patterns.Decorator;
using Spectre.Console;

namespace KPO_DZ2.UI;

public class OperationsMenu(
    IOperationFacade operationFacade,
    IBankAccFacade accountFacade,
    ICategoryFacade categoryFacade)
{
    public void Show()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Управление операциями[/]"));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите действие:")
                    .PageSize(10)
                    .AddChoices(
                        "Создать операцию дохода",
                        "Создать операцию расхода",
                        "Просмотреть все операции", 
                        "Просмотреть операции за период",
                        "Удалить операцию",
                        "Назад"
                    ));

            switch (choice)
            {
                case "Создать операцию дохода":
                    CreateOperation(CategoryType.Income);
                    break;
                case "Создать операцию расхода":
                    CreateOperation(CategoryType.Expense);
                    break;
                case "Просмотреть все операции":
                    ShowAllOperations();
                    break;
                case "Просмотреть операции за период":
                    ShowOperationsByPeriod();
                    break;
                case "Удалить операцию":
                    DeleteOperation();
                    break;
                case "Назад":
                    return;
            }
        }
    }

    private void CreateOperation(CategoryType type)
    {
        try
        {
            var accounts = accountFacade.GetAllAccounts().ToList();
            if (!accounts.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Нет доступных счетов. Сначала создайте счет.[/]");
                WaitForContinue();
                return;
            }

            var accountChoice = AnsiConsole.Prompt(
                new SelectionPrompt<BankAccount>()
                    .Title("Выберите счет:")
                    .PageSize(10)
                    .UseConverter(acc => $"{acc.Name} ({acc.Balance} руб.)")
                    .AddChoices(accounts));

            var categories = categoryFacade.GetCategoriesByType(type).ToList();
            if (categories.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]Нет категорий для типа {type}. Сначала создайте категорию.[/]");
                WaitForContinue();
                return;
            }

            var categoryChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Category>()
                    .Title("Выберите категорию:")
                    .PageSize(10)
                    .UseConverter(cat => cat.Name)
                    .AddChoices(categories));

            var amount = AnsiConsole.Prompt(
                new TextPrompt<double>("Введите сумму:")
                    .ValidationErrorMessage("Неверный формат суммы")
                    .Validate(amount => amount > 0 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error("Сумма должна быть положительной")));

            var description = AnsiConsole.Ask<string>("Введите описание (опционально):", "");

            AnsiConsole.Status()
                .Start("Создание операции...", ctx =>
                {
                    var command = new TimedCommandDecorator<Operation>(
                        new CreateOperationCommand(operationFacade, type, accountChoice.Id, amount, 
                            DateTime.Now, categoryChoice.Id, description));
                    
                    var operation = command.Execute();
                    
                    var operationType = type == CategoryType.Income ? "дохода" : "расхода";
                    var amountColor = type == CategoryType.Income ? "green" : "red";
                    
                    AnsiConsole.MarkupLine($"[green]Операция {operationType} создана: " +
                                           $"[{amountColor}]{operation.Amount} руб.[/] - {operation.Description}[/]");
                });

            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private void ShowAllOperations()
    {
        var operations = operationFacade.GetAllOperations()
            .OrderByDescending(o => o.Date)
            .ToList();
        
        if (!operations.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Операции не найдены[/]");
            WaitForContinue();
            return;
        }

        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.AddColumn(new TableColumn("[bold]Дата[/]").Width(12));
        table.AddColumn(new TableColumn("[bold]Тип[/]").Width(10));
        table.AddColumn(new TableColumn("[bold]Сумма[/]").Width(15).RightAligned());
        table.AddColumn(new TableColumn("[bold]Описание[/]"));

        foreach (var operation in operations)
        {
            var typeColor = operation.Type == CategoryType.Income ? "green" : "red";
            var typeName = operation.Type == CategoryType.Income ? "Доход" : "Расход";
            var amountSign = operation.Type == CategoryType.Income ? "+" : "-";
            
            table.AddRow(
                $"[grey]{operation.Date:dd.MM.yyyy}[/]",
                $"[{typeColor}]{typeName}[/]",
                $"[{typeColor}]{amountSign}{operation.Amount} руб.[/]",
                operation.Description);
        }

        AnsiConsole.Write(table);
        
        var totalIncome = operations.Where(o => o.Type == CategoryType.Income).Sum(o => o.Amount);
        var totalExpense = operations.Where(o => o.Type == CategoryType.Expense).Sum(o => o.Amount);
        var balance = totalIncome - totalExpense;
        
        AnsiConsole.MarkupLine($"\n[bold]Итого:[/] [green]Доходы: {totalIncome} руб.[/] | " +
                               $"[red]Расходы: {totalExpense} руб.[/] | [blue]Баланс: {balance} руб.[/]");
        
        WaitForContinue();
    }

    private void ShowOperationsByPeriod()
    {
        try
        {
            var startDate = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Введите начальную дату (dd.MM.yyyy):")
                    .ValidationErrorMessage("Неверный формат даты"));

            var endDate = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Введите конечную дату (dd.MM.yyyy):")
                    .ValidationErrorMessage("Неверный формат даты"));

            var operations = operationFacade.GetOperationsByPeriod(startDate, endDate)
                .OrderByDescending(o => o.Date)
                .ToList();

            if (!operations.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Операции за указанный период не найдены[/]");
                WaitForContinue();
                return;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.Title(new TableTitle($"Операции за период {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}"));
            
            table.AddColumn(new TableColumn("[bold]Дата[/]").Width(12));
            table.AddColumn(new TableColumn("[bold]Тип[/]").Width(10));
            table.AddColumn(new TableColumn("[bold]Сумма[/]").Width(15).RightAligned());
            table.AddColumn(new TableColumn("[bold]Описание[/]"));

            foreach (var operation in operations)
            {
                var typeColor = operation.Type == CategoryType.Income ? "green" : "red";
                var typeName = operation.Type == CategoryType.Income ? "Доход" : "Расход";
                var amountSign = operation.Type == CategoryType.Income ? "+" : "-";
                
                table.AddRow(
                    $"[grey]{operation.Date:dd.MM.yyyy}[/]",
                    $"[{typeColor}]{typeName}[/]",
                    $"[{typeColor}]{amountSign}{operation.Amount} руб.[/]",
                    operation.Description);
            }

            AnsiConsole.Write(table);
            
            var periodIncome = operations.Where(o => o.Type == CategoryType.Income).Sum(o => o.Amount);
            var periodExpense = operations.Where(o => o.Type == CategoryType.Expense).Sum(o => o.Amount);
            var periodBalance = periodIncome - periodExpense;
            
            AnsiConsole.MarkupLine($"\n[bold]За период:[/] [green]Доходы: {periodIncome} руб.[/] | [red]Расходы: {periodExpense} руб.[/] | [blue]Баланс: {periodBalance} руб.[/]");
            
            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private void DeleteOperation()
    {
        try
        {
            var operations = operationFacade.GetAllOperations()
                .OrderByDescending(o => o.Date)
                .ToList();

            if (!operations.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Нет доступных операций[/]");
                WaitForContinue();
                return;
            }

            var operationChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Operation>()
                    .Title("Выберите операцию для удаления:")
                    .PageSize(10)
                    .UseConverter(op => $"{op.Date:dd.MM.yyyy} - {op.Type} - {op.Amount} руб. - {op.Description}")
                    .AddChoices(operations));

            if (AnsiConsole.Confirm($"Вы уверены, что хотите удалить операцию от {operationChoice.Date:dd.MM.yyyy} на сумму {operationChoice.Amount} руб.?"))
            {
                operationFacade.DeleteOperation(operationChoice.Id);
                AnsiConsole.MarkupLine("[green]Операция удалена[/]");
            }
            
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