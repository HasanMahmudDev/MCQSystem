using System.ComponentModel.DataAnnotations;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using MCQSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Admin.Questions;

public sealed class IndexModel(
    IRepository<Question> questions,
    IRepository<Exam> exams,
    IRepository<Category> categories) : PageModel
{
    public IReadOnlyList<Question> Items { get; private set; } = [];

    public IReadOnlyList<Exam> Exams { get; private set; } = [];

    public IReadOnlyList<Category> Categories { get; private set; } = [];

    [BindProperty]
    public QuestionInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; private set; }

    public int PageSize { get; } = 10;

    public async Task OnGetAsync(string? editId, CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(editId))
        {
            var question = await questions.GetByIdAsync(editId, cancellationToken);
            if (question is not null)
            {
                Input = new QuestionInput
                {
                    Id = question.Id,
                    ExamId = question.ExamId,
                    CategoryId = question.CategoryId,
                    Text = question.Text,
                    OptionA = OptionText(question, "A"),
                    OptionB = OptionText(question, "B"),
                    OptionC = OptionText(question, "C"),
                    OptionD = OptionText(question, "D"),
                    CorrectOptionKey = question.CorrectOptionKey,
                    Difficulty = question.Difficulty,
                    Marks = question.Marks,
                    NegativeMarks = question.NegativeMarks,
                    Explanation = question.Explanation,
                    IsActive = question.IsActive
                };
            }
        }
    }

    public async Task<IActionResult> OnPostSaveAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Input.Id))
        {
            await questions.AddAsync(ToEntity(new Question()), cancellationToken);
        }
        else
        {
            var question = await questions.GetByIdAsync(Input.Id, cancellationToken);
            if (question is not null)
            {
                await questions.UpdateAsync(ToEntity(question), cancellationToken);
            }
        }

        TempData["Toast"] = "Question saved successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id, CancellationToken cancellationToken)
    {
        await questions.DeleteAsync(id, cancellationToken);
        TempData["Toast"] = "Question deleted successfully.";
        return RedirectToPage();
    }

    public string ExamTitle(string examId) => Exams.FirstOrDefault(x => x.Id == examId)?.Title ?? "No exam";

    public string CategoryName(string categoryId) => Categories.FirstOrDefault(x => x.Id == categoryId)?.Name ?? "Uncategorized";

    private Question ToEntity(Question question)
    {
        question.ExamId = Input.ExamId;
        question.CategoryId = Input.CategoryId;
        question.Text = Input.Text.Trim();
        question.Options =
        [
            new QuestionOption { Key = "A", Text = Input.OptionA.Trim() },
            new QuestionOption { Key = "B", Text = Input.OptionB.Trim() },
            new QuestionOption { Key = "C", Text = Input.OptionC.Trim() },
            new QuestionOption { Key = "D", Text = Input.OptionD.Trim() }
        ];
        question.CorrectOptionKey = Input.CorrectOptionKey;
        question.Difficulty = Input.Difficulty;
        question.Marks = Input.Marks;
        question.NegativeMarks = Input.NegativeMarks;
        question.Explanation = Input.Explanation;
        question.IsActive = Input.IsActive;
        return question;
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Exams = (await exams.ListAsync(cancellationToken: cancellationToken)).OrderBy(x => x.Title).ToList();
        Categories = (await categories.ListAsync(x => x.IsActive, cancellationToken)).OrderBy(x => x.Name).ToList();
        var data = await questions.ListAsync(cancellationToken: cancellationToken);
        if (!string.IsNullOrWhiteSpace(Search))
        {
            data = data
                .Where(x => x.Text.Contains(Search, StringComparison.OrdinalIgnoreCase)
                    || x.Options.Any(o => o.Text.Contains(Search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        var ordered = data.OrderByDescending(x => x.CreatedAtUtc).ToList();
        TotalPages = Math.Max(1, (int)Math.Ceiling(ordered.Count / (double)PageSize));
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        Items = ordered.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
        if (string.IsNullOrWhiteSpace(Input.ExamId) && Exams.Count > 0)
        {
            Input.ExamId = Exams[0].Id;
        }
        if (string.IsNullOrWhiteSpace(Input.CategoryId) && Categories.Count > 0)
        {
            Input.CategoryId = Categories[0].Id;
        }
    }

    private static string OptionText(Question question, string key)
        => question.Options.FirstOrDefault(x => x.Key == key)?.Text ?? string.Empty;

    public sealed class QuestionInput
    {
        public string? Id { get; set; }

        [Required]
        [Display(Name = "Exam")]
        public string ExamId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public string CategoryId { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Option A")]
        public string OptionA { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Option B")]
        public string OptionB { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Option C")]
        public string OptionC { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Option D")]
        public string OptionD { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Correct answer")]
        public string CorrectOptionKey { get; set; } = "A";

        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;

        [Range(0.25, 1000)]
        public decimal Marks { get; set; } = 1;

        [Range(0, 1000)]
        [Display(Name = "Negative marks")]
        public decimal NegativeMarks { get; set; } = 0.25m;

        public string? Explanation { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
