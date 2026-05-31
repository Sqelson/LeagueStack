using Microsoft.AspNetCore.Mvc;
using LeagueStack.Web.Data;

namespace LeagueStack.Web.Controllers;

public class HomeController(IReadOnlyList<TenantConfig> tenants) : Controller
{
    private readonly IReadOnlyList<TenantConfig> _tenants = tenants;

    public IActionResult Index()
    {
        return View(_tenants);
    }
}
