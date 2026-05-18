using System.ComponentModel.DataAnnotations;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Admin.Settings;

public sealed class IndexModel(IRepository<SystemSettings> settings) : PageModel
{
    [BindProperty]
    public SettingsInput Input { get; set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostSaveAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existing = (await settings.ListAsync(cancellationToken: cancellationToken)).FirstOrDefault();
        if (existing is null)
        {
            await settings.AddAsync(ToEntity(new SystemSettings()), cancellationToken);
        }
        else
        {
            await settings.UpdateAsync(ToEntity(existing), cancellationToken);
        }

        TempData["Toast"] = "Settings saved successfully.";
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        var current = (await settings.ListAsync(cancellationToken: cancellationToken)).FirstOrDefault() ?? new SystemSettings();
        Input = new SettingsInput
        {
            SiteName = current.SiteName,
            SupportEmail = current.SupportEmail,
            AllowRegistration = current.AllowRegistration,
            DefaultPassPercentage = current.DefaultPassPercentage,
            EnableNegativeMarking = current.EnableNegativeMarking
        };
    }

    private SystemSettings ToEntity(SystemSettings entity)
    {
        entity.SiteName = Input.SiteName.Trim();
        entity.SupportEmail = Input.SupportEmail.Trim();
        entity.AllowRegistration = Input.AllowRegistration;
        entity.DefaultPassPercentage = Input.DefaultPassPercentage;
        entity.EnableNegativeMarking = Input.EnableNegativeMarking;
        return entity;
    }

    public sealed class SettingsInput
    {
        [Required]
        [Display(Name = "Site name")]
        public string SiteName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Support email")]
        public string SupportEmail { get; set; } = string.Empty;

        [Display(Name = "Allow registration")]
        public bool AllowRegistration { get; set; }

        [Range(0, 100)]
        [Display(Name = "Default pass percentage")]
        public decimal DefaultPassPercentage { get; set; }

        [Display(Name = "Enable negative marking")]
        public bool EnableNegativeMarking { get; set; }
    }
}
