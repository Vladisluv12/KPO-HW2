using KPO_DZ2.Domain.Factories;
using KPO_DZ2.Domain.Model;
using KPO_DZ2.Domain.Repositories;
using KPO_DZ2.Domain.Services;
using KPO_DZ2.Infra.RepositoryRealisation;

namespace KPO_DZ2;

using Microsoft.Extensions.DependencyInjection;

public static class DiContainer
{
    public static ServiceProvider InitServices()
    {
        // Setup DI Container
        var services = new ServiceCollection();

        // Register repositories
        services.AddSingleton<IBankAccRepository, BankAccRepository>();
        services.AddSingleton<ICategoryRepository, CategoryRepository>();
        services.AddSingleton<IOperationRepository, OperationRepository>();

        // Register factory
        services.AddSingleton<IFactory, Factory>();

        // Register facades
        services.AddSingleton<IBankAccFacade, BankAccFacade>();
        services.AddSingleton<ICategoryFacade, CategoryFacade>();
        services.AddSingleton<IOperationFacade, OperationFacade>();
        services.AddSingleton<IAnalyticsFacade, AnalyticsFacade>();

        // Register cached repositories (proxy)
        services.AddSingleton<IRepository<BankAccount>>(provider =>
            new CachedRepository<BankAccount, IBankAccRepository>(provider.GetRequiredService<IBankAccRepository>()));
        services.AddSingleton<ICategoryRepository>(provider =>
            new CachedCategoryRepository(provider.GetRequiredService<CategoryRepository>()));
        services.AddSingleton<IOperationRepository>(provider =>
            new CachedOperationRepository(provider.GetRequiredService<OperationRepository>()));

        return services.BuildServiceProvider();
    }
}