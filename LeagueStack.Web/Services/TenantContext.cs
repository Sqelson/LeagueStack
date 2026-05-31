using LeagueStack.Web.Data;

namespace LeagueStack.Web.Services;

public class TenantContext
{
    public string? TenantId { get; set; }
    public bool HasTenant => TenantId is not null;
    public string FriendlyName { get; set; } = string.Empty;
    public TenantTheme Theme { get; set; } = new();
}
