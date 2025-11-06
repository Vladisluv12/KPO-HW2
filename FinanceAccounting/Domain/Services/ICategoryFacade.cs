using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Domain.Services;

public interface ICategoryFacade
{
    Category CreateCategory(CategoryType type, string name);
    Category? GetCategory(Guid id);
    void UpdateCategoryName(Guid id, string newName);
    void DeleteCategory(Guid id);
    IEnumerable<Category> GetAllCategories();
    IEnumerable<Category> GetCategoriesByType(CategoryType type);
}