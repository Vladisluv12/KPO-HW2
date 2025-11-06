using KPO_DZ2;
using KPO_DZ2.Domain.Factories;
using KPO_DZ2.Domain.Repositories;
using KPO_DZ2.Domain.Services;

public static class Program
{
    public static void Main()
    {
        DiContainer.InitServices();
        Console.WriteLine("=== Financial Accounting System ===");
        Console.WriteLine("ВШЭ-Банк - Модуль учета финансов\n");

        MainMenu();
    }
    
}