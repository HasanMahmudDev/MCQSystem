using MCQSystem.Application.DTOs;
using MCQSystem.Application.Interfaces;
using MCQSystem.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MCQSystem.WebUI.Pages.Shared.Components.Notifications;

public sealed class NotificationsViewComponent(IDashboardService dashboardService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!User.Identity?.IsAuthenticated ?? true || !User.IsInRole(AppRoles.Admin))
        {
            return View(Array.Empty<ActivityDto>());
        }

        var dashboard = await dashboardService.GetDashboardAsync(HttpContext.RequestAborted);
        return View(dashboard.RecentActivities.Take(3).ToList());
    }
}
