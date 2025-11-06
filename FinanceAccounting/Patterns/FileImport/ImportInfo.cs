namespace KPO_DZ2.Patterns.TemplateMethod;

public class ImportInfo
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public int ImportedAccounts { get; set; }
    public int ImportedCategories { get; set; }
    public int ImportedOperations { get; set; }
}