using LeagueStack.Abstractions;

namespace LeagueStack.Kent5aside;

public class Kent5asidePlugin : ITenantPlugin
{
    public string TenantId => "kent5aside";

    public void ConfigureServices(ITenantServiceBuilder services)
    {
        services.AddScoped<IPointsConfiguration, Kent5asideScoringConfiguration>();
    }
}

internal class Kent5asideScoringConfiguration : IPointsConfiguration
{
    public int WinPoints => 3;

    public int DrawPoints => 0;

    public int LossPoints => -1;

    public StandingsOrderField[] StandingsOrder { get; } =
    [
        StandingsOrderField.Points,
        StandingsOrderField.Won
    ];
}