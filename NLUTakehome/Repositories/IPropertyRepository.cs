namespace NLUTakehome.Repositories;

using NLUTakehome.Models.Db;

public interface IPropertyRepository
{
    Task<(List<ViolationRow> Violations, bool IsScofflaw)> GetViolationsAsync(string address);
}
