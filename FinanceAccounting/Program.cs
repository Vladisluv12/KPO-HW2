using KPO_DZ2;
using KPO_DZ2.Domain.Factories;
using KPO_DZ2.Domain.Repositories;
using KPO_DZ2.Domain.Services;
using KPO_DZ2.UI;
using Microsoft.Extensions.DependencyInjection;

public static class Program
{
    public static void Main()
    {
        var services = DiContainer.InitServices();
        
        var menu = new MainMenu(services.GetRequiredService<IBankAccFacade>(),
            services.GetRequiredService<ICategoryFacade>(),
            services.GetRequiredService<IOperationFacade>(),
            services.GetRequiredService<IAnalyticsFacade>());
        menu.Show();
    }
    
}