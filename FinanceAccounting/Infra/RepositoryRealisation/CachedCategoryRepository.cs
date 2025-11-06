using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;

namespace KPO_DZ2.Infra.RepositoryRealisation;

public class CachedCategoryRepository(ICategoryRepository decoratedRepository)
    : CachedRepository<Category, ICategoryRepository>(decoratedRepository), ICategoryRepository
{
    private readonly ICategoryRepository _decoratedCategoryRepository = decoratedRepository;

    public IEnumerable<Category> GetByType(CategoryType type)
        => _decoratedCategoryRepository.GetByType(type);
}