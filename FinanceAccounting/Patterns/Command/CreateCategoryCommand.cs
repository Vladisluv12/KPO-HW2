using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Services;

namespace KPO_DZ2.Patterns.Command;

public class CreateCategoryCommand(ICategoryFacade categoryFacade, CategoryType type, string name)
    : ICommand<Category>
{
    public Category Execute() => categoryFacade.CreateCategory(type, name);
}
