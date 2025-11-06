using KPO_DZ2.Domain.Model;

namespace KPO_DZ2.Patterns.TemplateMethod;

public abstract class DataImporter
{
    public ImportInfo Import(string filePath)
    {
        try
        {
            var data = ReadFile(filePath);
            var parsedData = ParseData(data);
            ValidateData(parsedData);
            return SaveData(parsedData);
        }
        catch (Exception ex)
        {
            return new ImportInfo { Success = false, ErrorMessage = ex.Message };
        }
    }

    protected abstract string ReadFile(string filePath);
    protected abstract Imported ParseData(string data);
    protected virtual void ValidateData(Imported data)
    {
        if (data == null)
            throw new ArgumentException("Invalid data format");
    }
    
    protected abstract ImportInfo SaveData(Imported data);
}