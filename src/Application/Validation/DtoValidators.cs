using FluentValidation;
using MCQSystem.Application.DTOs;

namespace MCQSystem.Application.Validation;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public sealed class ExamDtoValidator : AbstractValidator<ExamDto>
{
    public ExamDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(160);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.DurationMinutes).InclusiveBetween(1, 300);
        RuleFor(x => x.NumberOfQuestions).InclusiveBetween(1, 500);
        RuleFor(x => x.PassMarks).GreaterThanOrEqualTo(0);
    }
}

public sealed class QuestionDtoValidator : AbstractValidator<QuestionDto>
{
    public QuestionDtoValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.ExamId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Options).Must(x => x.Count == 4).WithMessage("Each question must have exactly four options.");
        RuleForEach(x => x.Options).ChildRules(option =>
        {
            option.RuleFor(x => x.Key).NotEmpty().MaximumLength(1);
            option.RuleFor(x => x.Text).NotEmpty().MaximumLength(500);
        });
        RuleFor(x => x.CorrectOptionKey).NotEmpty().Must(x => new[] { "A", "B", "C", "D" }.Contains(x.ToUpperInvariant()));
        RuleFor(x => x.Marks).GreaterThan(0);
        RuleFor(x => x.NegativeMarks).GreaterThanOrEqualTo(0);
    }
}
