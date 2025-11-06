using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Services;

namespace KPO_DZ2.Patterns.Command;

public class CreateAccountCommand(IBankAccFacade accountFacade, string name, double initialBalance = 0)
    : ICommand<BankAccount>
{
    private readonly IBankAccFacade _accountFacade = accountFacade;
    private readonly string _name = name;
    private readonly double _initialBalance = initialBalance;

    public BankAccount Execute() => _accountFacade.CreateAccount(_name, _initialBalance);
}
