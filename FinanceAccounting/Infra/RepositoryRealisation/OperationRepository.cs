using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;

namespace KPO_DZ2.Infra.RepositoryRealisation;

public class OperationRepository : IOperationRepository
{
    private readonly Dictionary<Guid, Operation> _operations = new();

    public void Add(Operation entity) => _operations[entity.Id] = entity;
    public void Remove(Guid id) => _operations.Remove(id);
    public void Update(Operation entity) => _operations[entity.Id] = entity;
    public Operation? GetById(Guid id) => _operations.GetValueOrDefault(id);
    public IEnumerable<Operation> GetAll() => _operations.Values;
        
    public IEnumerable<Operation> GetByPeriod(DateTime startDate, DateTime endDate)
        => _operations.Values.Where(o => o.Date >= startDate && o.Date <= endDate);
            
    public IEnumerable<Operation> GetByCategory(Guid categoryId)
        => _operations.Values.Where(o => o.CategoryId == categoryId);
}