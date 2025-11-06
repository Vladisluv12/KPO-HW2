using KPO_DZ2.Domain.Factories;
using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;

namespace KPO_DZ2.Domain.Services;

public class CategoryFacade(IFactory factory, ICategoryRepository repository) : ICategoryFacade
{
    public Category CreateCategory(CategoryType type, string name)
    {
        var category = factory.CreateCategory(type, name);
        repository.Add(category);
        return category;
    }

    public Category? GetCategory(Guid id) => repository.GetById(id);

    public void UpdateCategoryName(Guid id, string newName)
    {
        var category = repository.GetById(id);
        if (category == null)
        {
            return;
        }
        category.UpdateName(newName);
        repository.Update(category);
    }

    public void DeleteCategory(Guid id) => repository.Remove(id);

    public IEnumerable<Category> GetAllCategories() => repository.GetAll();

    public IEnumerable<Category> GetCategoriesByType(CategoryType type) => repository.GetByType(type);
}