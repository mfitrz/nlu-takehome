namespace NLUTakehome.Repositories;

public interface ICommentsRepository
{
    Task<bool> PropertyExistsAsync(string address);
    Task InsertCommentAsync(string address, string author, string comment);
}
