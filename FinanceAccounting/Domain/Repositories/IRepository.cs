namespace KPO_DZ2.Domain.Repositories;

public interface IRepository<T>
{
    void Add(T entity);
    void Remove(Guid id);
    void Update(T entity);
    T? GetById(Guid id);
    IEnumerable<T> GetAll();
}