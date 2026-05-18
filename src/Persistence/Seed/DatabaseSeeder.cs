using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using MCQSystem.Domain.Enums;
using MCQSystem.Shared;

namespace MCQSystem.Persistence.Seed;

public sealed class DatabaseSeeder(
    MongoDbContext context,
    IRepository<ApplicationRole> roles,
    IRepository<ApplicationUser> users,
    IRepository<Category> categories,
    IRepository<Exam> exams,
    IRepository<Question> questions,
    IRepository<SystemSettings> settings,
    IPasswordHashService passwordHashService) : IDatabaseSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await context.EnsureIndexesAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        await SeedUsersAsync(cancellationToken);
        await SeedSettingsAsync(cancellationToken);
        await SeedExamDataAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        foreach (var roleName in AppRoles.All)
        {
            var normalized = roleName.ToUpperInvariant();
            if (await roles.CountAsync(x => x.NormalizedName == normalized, cancellationToken) == 0)
            {
                await roles.AddAsync(new ApplicationRole
                {
                    Name = roleName,
                    NormalizedName = normalized
                }, cancellationToken);
            }
        }
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        await EnsureUserAsync("Admin Hasan", "admin@mcq.local", "Admin@12345", [AppRoles.Admin], cancellationToken);
        await EnsureUserAsync("Demo Student", "student@mcq.local", "Student@12345", [AppRoles.Student], cancellationToken);
    }

    private async Task EnsureUserAsync(string fullName, string email, string password, List<string> userRoles, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.ToUpperInvariant();
        if (await users.CountAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken) > 0)
        {
            return;
        }

        var user = new ApplicationUser
        {
            FullName = fullName,
            Email = email,
            NormalizedEmail = normalizedEmail,
            Roles = userRoles,
            IsActive = true
        };
        user.PasswordHash = passwordHashService.HashPassword(user, password);

        await users.AddAsync(user, cancellationToken);
    }

    private async Task SeedSettingsAsync(CancellationToken cancellationToken)
    {
        if (await settings.CountAsync(cancellationToken: cancellationToken) > 0)
        {
            return;
        }

        await settings.AddAsync(new SystemSettings
        {
            SiteName = "MCQ Examination System",
            SupportEmail = "support@mcq.local",
            AllowRegistration = true,
            DefaultPassPercentage = 50,
            EnableNegativeMarking = true
        }, cancellationToken);
    }

    private async Task SeedExamDataAsync(CancellationToken cancellationToken)
    {
        if (await categories.CountAsync(cancellationToken: cancellationToken) > 0)
        {
            return;
        }

        var category = await categories.AddAsync(new Category
        {
            Name = "ASP.NET Core",
            Slug = "aspnet-core",
            Description = "Razor Pages, dependency injection, middleware, and clean architecture."
        }, cancellationToken);

        var exam = await exams.AddAsync(new Exam
        {
            Title = "ASP.NET Core Fundamentals",
            Description = "A starter MCQ exam covering Razor Pages and application architecture basics.",
            CategoryId = category.Id,
            DurationMinutes = 15,
            NumberOfQuestions = 5,
            PassMarks = 3,
            IsPublished = true,
            RandomizeQuestions = true
        }, cancellationToken);

        var seedQuestions = new List<Question>
        {
            CreateQuestion(exam.Id, category.Id, "Which file configures the ASP.NET Core request pipeline?", "Program.cs", "appsettings.json", "_ViewImports.cshtml", "launchSettings.json", "A", "Program.cs configures services and middleware in modern ASP.NET Core."),
            CreateQuestion(exam.Id, category.Id, "Which pattern separates Domain, Application, Infrastructure, and UI concerns?", "Repository Pattern", "Clean Architecture", "Singleton Pattern", "Factory Method", "B", "Clean Architecture keeps business rules independent from UI and infrastructure."),
            CreateQuestion(exam.Id, category.Id, "Which MongoDB collection stores exam submissions in this system?", "Questions", "StudentAnswers", "Settings", "Roles", "B", "StudentAnswers stores one record per submitted question."),
            CreateQuestion(exam.Id, category.Id, "What does Razor Pages use for page handlers?", "OnGet and OnPost methods", "Controllers only", "Stored procedures", "SignalR hubs only", "A", "Razor Pages handlers are named OnGet, OnPost, and related variants."),
            CreateQuestion(exam.Id, category.Id, "Which service lifetime is normally used for repositories per web request?", "Singleton", "Scoped", "Transient static", "Hosted", "B", "Scoped services are created once per request.")
        };

        foreach (var question in seedQuestions)
        {
            await questions.AddAsync(question, cancellationToken);
        }

        exam.QuestionIds = seedQuestions.Select(x => x.Id).ToList();
        exam.TotalMarks = seedQuestions.Sum(x => x.Marks);
        await exams.UpdateAsync(exam, cancellationToken);
    }

    private static Question CreateQuestion(
        string examId,
        string categoryId,
        string text,
        string optionA,
        string optionB,
        string optionC,
        string optionD,
        string correct,
        string explanation)
        => new()
        {
            ExamId = examId,
            CategoryId = categoryId,
            Text = text,
            Options =
            [
                new QuestionOption { Key = "A", Text = optionA },
                new QuestionOption { Key = "B", Text = optionB },
                new QuestionOption { Key = "C", Text = optionC },
                new QuestionOption { Key = "D", Text = optionD }
            ],
            CorrectOptionKey = correct,
            Difficulty = DifficultyLevel.Medium,
            Marks = 1,
            NegativeMarks = 0.25m,
            Explanation = explanation,
            IsActive = true
        };
}
