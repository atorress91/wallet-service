using Hangfire.Dashboard;

namespace WalletService.Ioc;

public class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}