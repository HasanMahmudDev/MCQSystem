using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MCQSystem.Persistence;

public static class MongoDbStartup
{
    public static async Task EnsureAvailableAsync(
        IOptions<MongoDbSettings> settings,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var connectionString = settings.Value.ConnectionString;
        var maxAttempts = 10;
        var delay = TimeSpan.FromSeconds(3);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var client = new MongoClient(connectionString);
                await client.GetDatabase(settings.Value.DatabaseName)
                    .RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
                logger.LogInformation("MongoDB is available at {ConnectionString}", connectionString);
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(
                    ex,
                    "MongoDB not ready (attempt {Attempt}/{MaxAttempts}). Retrying in {DelaySeconds}s...",
                    attempt,
                    maxAttempts,
                    delay.TotalSeconds);
                await Task.Delay(delay, cancellationToken);
            }
        }

        throw new InvalidOperationException(
            $"""
            MongoDB is not reachable at '{connectionString}'.

            Start MongoDB locally, then run the app again:

              1. Install MongoDB Community Server (Windows):
                 https://www.mongodb.com/try/download/community

              2. Or use Docker (if installed):
                 docker compose up -d

              3. Or run the helper script from the repo root:
                 .\scripts\Start-MongoDB.ps1

              4. For cloud MongoDB Atlas, set MongoDb:ConnectionString in:
                 src/WebUI/appsettings.Development.json

            Original error: connection refused on localhost:27017 (nothing is listening).
            """);
    }
}
