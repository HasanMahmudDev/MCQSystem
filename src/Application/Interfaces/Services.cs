using MCQSystem.Application.Common;
using MCQSystem.Application.DTOs;
using MCQSystem.Domain.Entities;

namespace MCQSystem.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthenticatedUserDto>> SignInAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<ServiceResult<AuthenticatedUserDto>> RegisterStudentAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}

public interface IExamService
{
    Task<IReadOnlyList<StudentExamSummaryDto>> GetPublishedExamsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<ExamAttemptDto>> StartExamAsync(string examId, string userId, CancellationToken cancellationToken = default);

    Task<ServiceResult<ResultDto>> SubmitExamAsync(string userId, SubmitExamRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ResultDto>> GetHistoryAsync(string userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ResultDto>> GetLeaderboardAsync(string examId, CancellationToken cancellationToken = default);
}

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}

public interface IPasswordHashService
{
    string HashPassword(ApplicationUser user, string password);

    bool VerifyPassword(ApplicationUser user, string password);
}

public interface IDatabaseSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
}
