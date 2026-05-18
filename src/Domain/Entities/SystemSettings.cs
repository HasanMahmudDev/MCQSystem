using MCQSystem.Domain.Common;

namespace MCQSystem.Domain.Entities;

public sealed class SystemSettings : BaseEntity
{
    public string SiteName { get; set; } = "MCQ Examination System";

    public string SupportEmail { get; set; } = "support@mcq.local";

    public bool AllowRegistration { get; set; } = true;

    public decimal DefaultPassPercentage { get; set; } = 50;

    public bool EnableNegativeMarking { get; set; } = true;
}
