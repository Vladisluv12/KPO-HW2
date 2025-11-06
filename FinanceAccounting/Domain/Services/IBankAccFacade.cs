using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Domain.Services;

public interface IBankAccFacade
{
    BankAccount CreateAccount(string name, double initialBalance = 0);
    BankAccount? GetAccount(Guid id);
    void UpdateAccountName(Guid id, string newName);
    void DeleteAccount(Guid id);
    IEnumerable<BankAccount> GetAllAccounts();
}