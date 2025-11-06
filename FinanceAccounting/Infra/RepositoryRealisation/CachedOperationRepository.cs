using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;

namespace KPO_DZ2.Infra.RepositoryRealisation;

public class CachedOperationRepository(IOperationRepository decoratedRepository)
    : CachedRepository<Operation, IOperationRepository>(decoratedRepository), IOperationRepository
{
    private readonly IOperationRepository _decoratedOperationRepository = decoratedRepository;

    public IEnumerable<Operation> GetByPeriod(DateTime startDate, DateTime endDate)
        => _decoratedOperationRepository.GetByPeriod(startDate, endDate);

    public IEnumerable<Operation> GetByCategory(Guid categoryId)
        => _decoratedOperationRepository.GetByCategory(categoryId);
}
