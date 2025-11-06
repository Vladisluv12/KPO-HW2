using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;

namespace KPO_DZ2.Infra.RepositoryRealisation;

public class CategoryRepository : ICategoryRepository
{
    private readonly Dictionary<Guid, Category> _categories = new();

    public void Add(Category entity) => _categories[entity.Id] = entity;
    public void Remove(Guid id) => _categories.Remove(id);
    public void Update(Category entity) => _categories[entity.Id] = entity;
    public Category? GetById(Guid id) => _categories.GetValueOrDefault(id);
    public IEnumerable<Category> GetAll() => _categories.Values;
    public IEnumerable<Category> GetByType(CategoryType type) => _categories.Values.Where(c => c.Type == type);
}