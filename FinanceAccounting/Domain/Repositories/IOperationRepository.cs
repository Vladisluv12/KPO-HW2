using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Domain.Repositories;

public interface IOperationRepository : IRepository<Operation>
{
    IEnumerable<Operation> GetByPeriod(DateTime startDate, DateTime endDate);
    IEnumerable<Operation> GetByCategory(Guid categoryId);
}