using LeagueStack.Abstractions;

namespace LeagueStack.SimpleCricket;

public class SimpleCricketPlugin : ITenantPlugin
{
    public string TenantId => "simplecricket";

    public void ConfigureServices(ITenantServiceBuilder services)
    {
    }
}