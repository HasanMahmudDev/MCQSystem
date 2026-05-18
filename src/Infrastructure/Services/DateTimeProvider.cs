using MCQSystem.Application.Interfaces;

namespace MCQSystem.Infrastructure.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
