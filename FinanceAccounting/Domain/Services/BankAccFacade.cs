using KPO_DZ2.Domain.Factories;
using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;

namespace KPO_DZ2.Domain.Services;

public class BankAccFacade(IFactory factory, IBankAccRepository repository) : IBankAccFacade
{
    public BankAccount CreateAccount(string name, double initialBalance = 0)
    {
        var account = factory.CreateBankAccount(name, initialBalance);
        repository.Add(account);
        return account;
    }

    public BankAccount? GetAccount(Guid id) => repository.GetById(id);

    public void UpdateAccountName(Guid id, string newName)
    {
        var account = repository.GetById(id);
        if (account == null)
            return;
        account.UpdateName(newName);
        repository.Update(account);
    }

    public void DeleteAccount(Guid id) => repository.Remove(id);

    public IEnumerable<BankAccount> GetAllAccounts() => repository.GetAll();
}