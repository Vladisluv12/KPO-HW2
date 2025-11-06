using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Services;
using KPO_DZ2.Patterns.Command;
using KPO_DZ2.Patterns.Decorator;
using Spectre.Console;

namespace KPO_DZ2.UI;

public class CategoriesMenu
{
    private readonly ICategoryFacade _categoryFacade;

    public CategoriesMenu(ICategoryFacade categoryFacade)
    {
        _categoryFacade = categoryFacade;
    }

    public void Show()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Управление категориями[/]"));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите действие:")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Создать категорию дохода",
                        "Создать категорию расхода", 
                        "Просмотреть все категории",
                        "Изменить название категории",
                        "Удалить категорию",
                        "Назад"
                    }));

            switch (choice)
            {
                case "Создать категорию дохода":
                    CreateCategory(CategoryType.Income);
                    break;
                case "Создать категорию расхода":
                    CreateCategory(CategoryType.Expense);
                    break;
                case "Просмотреть все категории":
                    ShowAllCategories();
                    break;
                case "Изменить название категории":
                    UpdateCategoryName();
                    break;
                case "Удалить категорию":
                    DeleteCategory();
                    break;
                case "Назад":
                    return;
            }
        }
    }

    private void CreateCategory(CategoryType type)
    {
        try
        {
            var name = AnsiConsole.Ask<string>("Введите название категории:");
            var typeName = type == CategoryType.Income ? "дохода" : "расхода";

            AnsiConsole.Status()
                .Start("Создание категории...", _ =>
                {
                    var command = new TimedCommandDecorator<Category>(
                        new CreateCategoryCommand(_categoryFacade, type, name));
                    
                    var category = command.Execute();
                    
                    AnsiConsole.MarkupLine($"[green]Категория {typeName} создана: {category.Name}[/]");
                });

            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private void ShowAllCategories()
    {
        var categories = _categoryFacade.GetAllCategories().ToList();
        
        if (!categories.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Категории не найдены[/]");
            WaitForContinue();
            return;
        }

        var table = new Table();
        table.Border(TableBorder.Rounded);
        table.AddColumn(new TableColumn("[bold]ID[/]").Width(36));
        table.AddColumn(new TableColumn("[bold]Название[/]"));
        table.AddColumn(new TableColumn("[bold]Тип[/]"));

        foreach (var category in categories)
        {
            var typeColor = category.Type == CategoryType.Income ? "green" : "red";
            var typeName = category.Type == CategoryType.Income ? "Доход" : "Расход";
            
            table.AddRow(
                $"[grey]{category.Id}[/]",
                category.Name,
                $"[{typeColor}]{typeName}[/]");
        }

        AnsiConsole.Write(table);
        
        var incomeCount = categories.Count(c => c.Type == CategoryType.Income);
        var expenseCount = categories.Count(c => c.Type == CategoryType.Expense);
        
        AnsiConsole.MarkupLine($"\n[bold]Статистика:[/] [green]Доходы: {incomeCount}[/] | [red]Расходы: {expenseCount}[/]");
        
        WaitForContinue();
    }

    private void UpdateCategoryName()
    {
        try
        {
            var categories = _categoryFacade.GetAllCategories().ToList();
            if (!categories.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Нет доступных категорий[/]");
                WaitForContinue();
                return;
            }

            var categoryChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Category>()
                    .Title("Выберите категорию:")
                    .PageSize(10)
                    .UseConverter(cat => $"{cat.Name} ({cat.Type})")
                    .AddChoices(categories));

            var newName = AnsiConsole.Ask<string>("Введите новое название:");

            _categoryFacade.UpdateCategoryName(categoryChoice.Id, newName);
            AnsiConsole.MarkupLine("[green]Название категории обновлено[/]");
            
            WaitForContinue();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            WaitForContinue();
        }
    }

    private void DeleteCategory()
    {
        try
        {
            var categories = _categoryFacade.GetAllCategories().ToList();
            if (!categories.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Нет доступных категорий[/]");
                WaitForContinue();
                return;
            }

            var categoryChoice = AnsiConsole.Prompt(
                new SelectionPrompt<Category>()
                    .Title("Выберите категорию для удаления:")
                    .PageSize(10)
                    .UseConverter(cat => $"{cat.Name} ({cat.Type})")
                    .AddChoices(categories));

            if (AnsiConsole.Confirm($"Вы уверены, что хотите удалить категорию '{categoryChoice.Name}'?"))
            {
                _categoryFacade.DeleteCategory(categoryChoice.Id);
                AnsiConsole.MarkupLine("[green]Категория удалена[/]");
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