using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;

namespace KPO_DZ2.Infra.RepositoryRealisation;

public class CachedRepository<T, TRepository> : IRepository<T> where T : class where TRepository : IRepository<T>
{
    private readonly TRepository _decoratedRepository;
    private readonly Dictionary<Guid, T> _cache = new();
    
    private static Guid GetEntityId(T entity)
    {
        return entity switch
        {
            BankAccount account => account.Id,
            Category category => category.Id,
            Operation operation => operation.Id,
            _ => throw new InvalidOperationException("Unsupported entity type")
        };
    }

    public CachedRepository(TRepository decoratedRepository)
    {
        _decoratedRepository = decoratedRepository;
        foreach (var entity in _decoratedRepository.GetAll())
        {
            var id = GetEntityId(entity);
            _cache[id] = entity;
        }
    }

    public void Add(T entity)
    {
        _decoratedRepository.Add(entity);
        var id = GetEntityId(entity);
        _cache[id] = entity;
    }

    public void Remove(Guid id)
    {
        _decoratedRepository.Remove(id);
        _cache.Remove(id);
    }

    public void Update(T entity)
    {
        _decoratedRepository.Update(entity);
        var id = GetEntityId(entity);
        _cache[id] = entity;
    }

    public T? GetById(Guid id)
    {
        if (_cache.TryGetValue(id, out var entity))
            return entity;
        entity = _decoratedRepository.GetById(id);
        if (entity == null)
            return null;
        _cache[id] = entity;
        return entity;
    }
    public IEnumerable<T> GetAll() => _cache.Values;
}