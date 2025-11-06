using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Services;
using KPO_DZ2.Patterns.Command;
using KPO_DZ2.Patterns.Decorator;
using Spectre.Console;

namespace KPO_DZ2.UI;

public class BankAccMenu(IBankAccFacade accountFacade) : IMenu
{
    public void Show()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Управление счетами[/]"));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите действие:")
                    .PageSize(10)
                    .AddChoices(
                        "Создать счет",
                        "Просмотреть все счета",
                        "Изменить название счета", 
                        "Удалить счет",
                        "Назад"
                    ));

            switch (choice)
            {
                case "Создать счет":
                    CreateAccount();
                    break;
                case "Просмотреть все счета":
                    ShowAllAccounts();
                    break;
                case "Изменить название счета":
                    UpdateAccountName();
                    break;
                case "Удалить счет":
                    DeleteAccount();
                    break;
                case "Назад":
                    return;
            }
        }
    }
    private void CreateAccount()
    {
        try
        {
            var name = AnsiConsole.Ask<string>("Введите название счета:");
            
            var balance = AnsiConsole.Prompt(
                new TextPrompt<double>("Введите начальный баланс:")
                    .ValidationErrorMessage("Неверный формат баланса")
                    .Validate(balance => balance >= 0 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error("Баланс не может быть отрицательным")));

            AnsiConsole.Status()
                .Start("Создание счета...", _ =>
                {
                    var command = new TimedCommandDecorator<BankAccount>(
                        new CreateAccountCommand(accountFacade, name, balance));
                    
                    var account = command.Execute();
                    
                    AnsiConsole.MarkupLine($"[green]Счет создан: {account.Name}, Баланс: {account.Balance} руб.[/]");
                });

            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private void ShowAllAccounts()
    {
        var accounts = accountFacade.GetAllAccounts().ToList();
        
        if (!accounts.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Счета не найдены[/]");
            WaitForContinue();
            return;
        }

        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.AddColumn(new TableColumn("[bold]ID[/]").Width(36));
        table.AddColumn(new TableColumn("[bold]Название[/]"));
        table.AddColumn(new TableColumn("[bold]Баланс[/]").RightAligned());

        foreach (var account in accounts)
        {
            var balanceColor = account.Balance >= 0 ? "green" : "red";
            table.AddRow(
                $"[grey]{account.Id}[/]",
                account.Name,
                $"[{balanceColor}]{account.Balance} руб.[/]");
        }

        AnsiConsole.Write(table);
        
        var totalBalance = accounts.Sum(a => a.Balance);
        var totalColor = totalBalance >= 0 ? "green" : "red";
        AnsiConsole.MarkupLine($"\n[bold]Общий баланс: [{totalColor}]{totalBalance} руб.[/][/]");
        
        WaitForContinue();
    }

    private void UpdateAccountName()
    {
        try
        {
            var accounts = accountFacade.GetAllAccounts().ToList();
            if (!accounts.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Нет доступных счетов[/]");
                WaitForContinue();
                return;
            }

            var accountChoice = AnsiConsole.Prompt(
                new SelectionPrompt<BankAccount>()
                    .Title("Выберите счет:")
                    .PageSize(10)
                    .UseConverter(acc => $"{acc.Name} ({acc.Balance} руб.)")
                    .AddChoices(accounts));

            var newName = AnsiConsole.Ask<string>("Введите новое название:");

            accountFacade.UpdateAccountName(accountChoice.Id, newName);
            AnsiConsole.MarkupLine("[green]Название счета обновлено[/]");
            
            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private void DeleteAccount()
    {
        try
        {
            var accounts = accountFacade.GetAllAccounts().ToList();
            if (!accounts.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Нет доступных счетов[/]");
                WaitForContinue();
                return;
            }

            var accountChoice = AnsiConsole.Prompt(
                new SelectionPrompt<BankAccount>()
                    .Title("Выберите счет для удаления:")
                    .PageSize(10)
                    .UseConverter(acc => $"{acc.Name} ({acc.Balance} руб.)")
                    .AddChoices(accounts));

            if (AnsiConsole.Confirm($"Вы уверены, что хотите удалить счет '{accountChoice.Name}'?"))
            {
                accountFacade.DeleteAccount(accountChoice.Id);
                AnsiConsole.MarkupLine("[green]Счет удален[/]");
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