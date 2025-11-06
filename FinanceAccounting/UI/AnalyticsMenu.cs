using KPO_DZ2.Domain.Services;
using Spectre.Console;

namespace KPO_DZ2.UI;

public class AnalyticsMenu(IAnalyticsFacade analyticsFacade) : IMenu
{
    public void Show()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Аналитика и отчеты[/]"));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите отчет:")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Разница доходов и расходов за период",
                        "Группировка операций по категориям", 
                        "Полный финансовый отчет",
                        "Назад"
                    }));

            switch (choice)
            {
                case "Разница доходов и расходов за период":
                    ShowBalanceDifference();
                    break;
                case "Группировка операций по категориям":
                    ShowCategoryBreakdown();
                    break;
                case "Полный финансовый отчет":
                    ShowFinancialSummary();
                    break;
                case "Назад":
                    return;
            }
        }
    }

    private void ShowBalanceDifference()
    {
        try
        {
            var period = GetPeriodFromUser();
            if (!period.HasValue) return;

            var (startDate, endDate) = period.Value;

            AnsiConsole.Status()
                .Start("Расчет баланса...", _ =>
                {
                    var difference = analyticsFacade.CalculateBalanceDifference(startDate, endDate);
                    
                    var panel = new Panel($"[bold]{difference} руб.[/]")
                    {
                        Header = new PanelHeader("Разница доходов и расходов"),
                        Border = BoxBorder.Rounded
                    };

                    panel.BorderColor(difference >= 0 ? Color.Green : Color.Red);

                    AnsiConsole.Write(panel);
                    AnsiConsole.MarkupLine($"[grey]Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}[/]");
                });

            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private void ShowCategoryBreakdown()
    {
        try
        {
            var period = GetPeriodFromUser();
            if (!period.HasValue) return;

            var (startDate, endDate) = period.Value;

            AnsiConsole.Status()
                .Start("Анализ категорий...", _ =>
                {
                    var breakdown = analyticsFacade.GroupOperationsByCategory(startDate, endDate);
                    
                    if (!breakdown.Any())
                    {
                        AnsiConsole.MarkupLine("[yellow]Нет данных за указанный период[/]");
                        return;
                    }

                    var table = new Table();
                    table.Border(TableBorder.Rounded);
                    table.Title(new TableTitle($"Группировка по категориям"));
                    
                    table.AddColumn(new TableColumn("[bold]Категория[/]"));
                    table.AddColumn(new TableColumn("[bold]Сумма[/]").RightAligned());

                    foreach (var item in breakdown.OrderByDescending(x => Math.Abs(x.Value)))
                    {
                        var amountColor = item.Value >= 0 ? "green" : "red";
                        var amountSign = item.Value >= 0 ? "+" : "";
                        
                        table.AddRow(
                            item.Key,
                            $"[{amountColor}]{amountSign}{item.Value} руб.[/]");
                    }

                    AnsiConsole.Write(table);
                    AnsiConsole.MarkupLine($"[grey]Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}[/]");
                });

            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private void ShowFinancialSummary()
    {
        try
        {
            var period = GetPeriodFromUser();
            if (!period.HasValue) return;

            var (startDate, endDate) = period.Value;

            AnsiConsole.Status()
                .Start("Формирование отчета...", _ =>
                {
                    var summary = analyticsFacade.GetSummary(startDate, endDate);
                    
                    var grid = new Grid();
                    grid.AddColumn();
                    grid.AddColumn();
                    
                    grid.AddRow(
                        new Panel($"[bold green]{summary.TotalIncome} руб.[/]")
                        {
                            Header = new PanelHeader("Общие доходы"),
                            Border = BoxBorder.Rounded,
                            BorderStyle = new Style(Color.Green)
                        },
                        new Panel($"[bold red]{summary.TotalExpenses} руб.[/]")
                        {
                            Header = new PanelHeader("Общие расходы"),
                            Border = BoxBorder.Rounded,
                            BorderStyle = new Style(Color.Red)
                        });
                    
                    var balanceColor = summary.BalanceDifference >= 0 ? Color.Green : Color.Red;
                    
                    grid.AddRow(
                        new Panel($"[bold {balanceColor.ToMarkup()}]{summary.BalanceDifference} руб.[/]")
                        {
                            Header = new PanelHeader("Баланс"),
                            Border = BoxBorder.Rounded,
                            BorderStyle = new Style(balanceColor)
                        },
                        new Panel($"[bold]{summary.CategoryBreakdown.Count} категорий[/]")
                        {
                            Header = new PanelHeader("Статистика"),
                            Border = BoxBorder.Rounded
                        });

                    AnsiConsole.Write(grid);
                    AnsiConsole.MarkupLine($"[grey]Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}[/]");

                    if (summary.CategoryBreakdown.Any())
                    {
                        AnsiConsole.MarkupLine("\n[bold]Детали по категориям:[/]");
                        foreach (var item in summary.CategoryBreakdown.OrderByDescending(x => Math.Abs(x.Value)))
                        {
                            var amountColor = item.Value >= 0 ? "green" : "red";
                            var amountSign = item.Value >= 0 ? "+" : "";
                            AnsiConsole.MarkupLine($"  {item.Key}: [{amountColor}]{amountSign}{item.Value} руб.[/]");
                        }
                    }
                });

            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private static (DateTime startDate, DateTime endDate)? GetPeriodFromUser()
    {
        try
        {
            var startDate = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Введите начальную дату (dd.MM.yyyy):")
                    .ValidationErrorMessage("Неверный формат даты"));

            var endDate = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Введите конечную дату (dd.MM.yyyy):")
                    .ValidationErrorMessage("Неверный формат даты"));

            return (startDate, endDate);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static void WaitForContinue()
    {
        AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу для продолжения...[/]");
        Console.ReadKey();
    }
}