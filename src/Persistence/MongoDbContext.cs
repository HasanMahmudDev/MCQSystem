using MCQSystem.Domain.Common;
using MCQSystem.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MCQSystem.Persistence;

public sealed class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<T> Collection<T>() where T : BaseEntity
        => _database.GetCollection<T>(GetCollectionName(typeof(T)));

    public async Task EnsureIndexesAsync(CancellationToken cancellationToken = default)
    {
        var users = _database.GetCollection<ApplicationUser>(GetCollectionName(typeof(ApplicationUser)));
        await users.Indexes.CreateOneAsync(new CreateIndexModel<ApplicationUser>(
            Builders<ApplicationUser>.IndexKeys.Ascending(x => x.NormalizedEmail),
            new CreateIndexOptions { Unique = true, Name = "ux_users_normalized_email" }), cancellationToken: cancellationToken);

        var questions = _database.GetCollection<Question>(GetCollectionName(typeof(Question)));
        await questions.Indexes.CreateOneAsync(new CreateIndexModel<Question>(
            Builders<Question>.IndexKeys.Ascending(x => x.ExamId),
            new CreateIndexOptions { Name = "ix_questions_exam" }), cancellationToken: cancellationToken);

        var results = _database.GetCollection<Result>(GetCollectionName(typeof(Result)));
        await results.Indexes.CreateOneAsync(new CreateIndexModel<Result>(
            Builders<Result>.IndexKeys.Ascending(x => x.UserId).Descending(x => x.SubmittedAtUtc),
            new CreateIndexOptions { Name = "ix_results_user_submitted" }), cancellationToken: cancellationToken);
    }

    private static string GetCollectionName(Type type)
        => type.Name switch
        {
            nameof(ApplicationUser) => "Users",
            nameof(ApplicationRole) => "Roles",
            nameof(Exam) => "Exams",
            nameof(Question) => "Questions",
            nameof(Category) => "Categories",
            nameof(StudentAnswer) => "StudentAnswers",
            nameof(Result) => "Results",
            nameof(SystemSettings) => "Settings",
            nameof(AuditLog) => "AuditLogs",
            _ => $"{type.Name}s"
        };
}
