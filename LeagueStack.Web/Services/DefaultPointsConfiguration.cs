using LeagueStack.Abstractions;

namespace LeagueStack.Web.Services;

public class DefaultPointsConfiguration : IPointsConfiguration
{
    public int WinPoints => 3;

    public int DrawPoints => 1;

    public int LossPoints => 0;

    public StandingsOrderField[] StandingsOrder =>
    [
        StandingsOrderField.Points,
        StandingsOrderField.Difference
    ];
}
