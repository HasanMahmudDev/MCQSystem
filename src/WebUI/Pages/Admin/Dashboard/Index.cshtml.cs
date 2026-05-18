using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCQSystem.WebUI.Pages.Admin.Dashboard;

public sealed class IndexModel(IDashboardService dashboardService) : PageModel
{
    public DashboardDto Dashboard { get; private set; } = new(0, 0, 0, 0, 0, [], [], [], []);

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Dashboard = await dashboardService.GetDashboardAsync(cancellationToken);
    }
}
