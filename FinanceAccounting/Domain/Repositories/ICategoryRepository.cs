using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Domain.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    IEnumerable<Category> GetByType(CategoryType type);
}