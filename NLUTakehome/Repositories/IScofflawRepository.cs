namespace NLUTakehome.Repositories;

public interface IScofflawRepository
{
    Task<List<string>> GetAddressesWithViolationsSinceAsync(DateOnly sinceDate);
}
