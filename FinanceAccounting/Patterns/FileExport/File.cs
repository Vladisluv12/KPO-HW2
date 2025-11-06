namespace KPO_DZ2.Patterns.FileExport;

public abstract class File(string name)
{
    public string Name { get; set; } = name;
    public string? Content { get; set; }

    public abstract void Accept(IVisitor visitor);
}