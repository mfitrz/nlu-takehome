namespace NLUTakehome.Repositories;

using Npgsql;

public class CommentsRepository(NpgsqlDataSource db, ILogger<CommentsRepository> logger) : ICommentsRepository
{
    public async Task<bool> PropertyExistsAsync(string address)
    {
        try
        {
            await using var conn = await db.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(
                "SELECT 1 FROM violations WHERE address = @address LIMIT 1", conn);

            cmd.Parameters.AddWithValue("address", address);

            return await cmd.ExecuteScalarAsync() != null;
        }
        catch (NpgsqlException ex)
        {
            logger.LogError(ex, "Database error checking existence for address {Address}", address);
            throw;
        }
    }

    public async Task InsertCommentAsync(string address, string author, string comment)
    {
        try
        {
            await using var conn = await db.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
                INSERT INTO comments (address, author, comment_text, created_at)
                VALUES (@address, @author, @comment_text, NOW())", conn);

            cmd.Parameters.AddWithValue("address", address);
            cmd.Parameters.AddWithValue("author", author);
            cmd.Parameters.AddWithValue("comment_text", comment);

            await cmd.ExecuteNonQueryAsync();
        }
        catch (NpgsqlException ex)
        {
            logger.LogError(ex, "Database error inserting comment for address {Address}", address);
            throw;
        }
    }
}
