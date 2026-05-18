using System.ComponentModel.DataAnnotations;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Admin.Exams;

public sealed class IndexModel(IRepository<Exam> exams, IRepository<Category> categories) : PageModel
{
    public IReadOnlyList<Exam> Items { get; private set; } = [];

    public IReadOnlyList<Category> Categories { get; private set; } = [];

    [BindProperty]
    public ExamInput Input { get; set; } = new();

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
            var exam = await exams.GetByIdAsync(editId, cancellationToken);
            if (exam is not null)
            {
                Input = new ExamInput
                {
                    Id = exam.Id,
                    Title = exam.Title,
                    Description = exam.Description,
                    CategoryId = exam.CategoryId,
                    DurationMinutes = exam.DurationMinutes,
                    TotalMarks = exam.TotalMarks,
                    PassMarks = exam.PassMarks,
                    NumberOfQuestions = exam.NumberOfQuestions,
                    IsPublished = exam.IsPublished,
                    RandomizeQuestions = exam.RandomizeQuestions,
                    StartsAtUtc = exam.StartsAtUtc,
                    EndsAtUtc = exam.EndsAtUtc
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
            await exams.AddAsync(ToEntity(new Exam()), cancellationToken);
        }
        else
        {
            var exam = await exams.GetByIdAsync(Input.Id, cancellationToken);
            if (exam is not null)
            {
                await exams.UpdateAsync(ToEntity(exam), cancellationToken);
            }
        }

        TempData["Toast"] = "Exam saved successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id, CancellationToken cancellationToken)
    {
        await exams.DeleteAsync(id, cancellationToken);
        TempData["Toast"] = "Exam deleted successfully.";
        return RedirectToPage();
    }

    public string CategoryName(string categoryId)
        => Categories.FirstOrDefault(x => x.Id == categoryId)?.Name ?? "Uncategorized";

    private Exam ToEntity(Exam exam)
    {
        exam.Title = Input.Title.Trim();
        exam.Description = Input.Description;
        exam.CategoryId = Input.CategoryId;
        exam.DurationMinutes = Input.DurationMinutes;
        exam.TotalMarks = Input.TotalMarks;
        exam.PassMarks = Input.PassMarks;
        exam.NumberOfQuestions = Input.NumberOfQuestions;
        exam.IsPublished = Input.IsPublished;
        exam.RandomizeQuestions = Input.RandomizeQuestions;
        exam.StartsAtUtc = Input.StartsAtUtc;
        exam.EndsAtUtc = Input.EndsAtUtc;
        return exam;
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Categories = (await categories.ListAsync(x => x.IsActive, cancellationToken)).OrderBy(x => x.Name).ToList();
        var data = await exams.ListAsync(cancellationToken: cancellationToken);
        if (!string.IsNullOrWhiteSpace(Search))
        {
            data = data
                .Where(x => x.Title.Contains(Search, StringComparison.OrdinalIgnoreCase)
                    || (x.Description?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        var ordered = data.OrderByDescending(x => x.CreatedAtUtc).ToList();
        TotalPages = Math.Max(1, (int)Math.Ceiling(ordered.Count / (double)PageSize));
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        Items = ordered.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
        if (string.IsNullOrWhiteSpace(Input.CategoryId) && Categories.Count > 0)
        {
            Input.CategoryId = Categories[0].Id;
        }
    }

    public sealed class ExamInput
    {
        public string? Id { get; set; }

        [Required]
        [MaxLength(160)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string CategoryId { get; set; } = string.Empty;

        [Range(1, 300)]
        [Display(Name = "Duration minutes")]
        public int DurationMinutes { get; set; } = 30;

        [Range(0, 10000)]
        [Display(Name = "Total marks")]
        public decimal TotalMarks { get; set; } = 10;

        [Range(0, 10000)]
        [Display(Name = "Pass marks")]
        public decimal PassMarks { get; set; } = 5;

        [Range(1, 500)]
        [Display(Name = "Questions to show")]
        public int NumberOfQuestions { get; set; } = 10;

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Randomize questions")]
        public bool RandomizeQuestions { get; set; } = true;

        [Display(Name = "Starts at UTC")]
        public DateTime? StartsAtUtc { get; set; }

        [Display(Name = "Ends at UTC")]
        public DateTime? EndsAtUtc { get; set; }
    }
}
