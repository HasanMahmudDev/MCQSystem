using System.ComponentModel.DataAnnotations;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Admin.Categories;

public sealed class IndexModel(IRepository<Category> categories) : PageModel
{
    public IReadOnlyList<Category> Items { get; private set; } = [];

    [BindProperty]
    public CategoryInput Input { get; set; } = new();

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
            var category = await categories.GetByIdAsync(editId, cancellationToken);
            if (category is not null)
            {
                Input = new CategoryInput
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive
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
            await categories.AddAsync(new Category
            {
                Name = Input.Name.Trim(),
                Slug = Slugify(Input.Name),
                Description = Input.Description,
                IsActive = Input.IsActive
            }, cancellationToken);
        }
        else
        {
            var category = await categories.GetByIdAsync(Input.Id, cancellationToken);
            if (category is not null)
            {
                category.Name = Input.Name.Trim();
                category.Slug = Slugify(Input.Name);
                category.Description = Input.Description;
                category.IsActive = Input.IsActive;
                await categories.UpdateAsync(category, cancellationToken);
            }
        }

        TempData["Toast"] = "Category saved successfully.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id, CancellationToken cancellationToken)
    {
        await categories.DeleteAsync(id, cancellationToken);
        TempData["Toast"] = "Category deleted successfully.";
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        var data = await categories.ListAsync(cancellationToken: cancellationToken);
        if (!string.IsNullOrWhiteSpace(Search))
        {
            data = data
                .Where(x => x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)
                    || (x.Description?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        var ordered = data.OrderBy(x => x.Name).ToList();
        TotalPages = Math.Max(1, (int)Math.Ceiling(ordered.Count / (double)PageSize));
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        Items = ordered.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
    }

    private static string Slugify(string value)
        => string.Join('-', value.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));

    public sealed class CategoryInput
    {
        public string? Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
