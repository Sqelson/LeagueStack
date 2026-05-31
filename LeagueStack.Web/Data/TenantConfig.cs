namespace LeagueStack.Web.Data;

public class TenantConfig
{
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FriendlyName { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public TenantTheme Theme { get; set; } = new();
}

public class TenantTheme
{
    public string Primary { get; set; } = "#0d6efd";
    public string Secondary { get; set; } = "#6c757d";
    public string BodyBg { get; set; } = "#ffffff";
    public string BodyColor { get; set; } = "#212529";
}
