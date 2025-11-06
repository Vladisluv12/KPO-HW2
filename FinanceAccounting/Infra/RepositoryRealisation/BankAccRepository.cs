using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;

namespace KPO_DZ2.Infra.RepositoryRealisation;

public class BankAccRepository : IBankAccRepository
{
    private readonly Dictionary<Guid, BankAccount> _accounts = new();

    public void Add(BankAccount entity) => _accounts[entity.Id] = entity;
    public void Remove(Guid id) => _accounts.Remove(id);
    public void Update(BankAccount entity) => _accounts[entity.Id] = entity;
    public BankAccount? GetById(Guid id) => _accounts.GetValueOrDefault(id);
    public IEnumerable<BankAccount> GetAll() => _accounts.Values;
}