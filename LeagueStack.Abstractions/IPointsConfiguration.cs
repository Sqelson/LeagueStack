namespace LeagueStack.Abstractions;

public enum StandingsOrderField
{
    Points,
    Difference,
    For,
    Against,
    Won,
    Played
}

public interface IPointsConfiguration
{
    int WinPoints { get; }

    int DrawPoints { get; }

    int LossPoints { get; }

    StandingsOrderField[] StandingsOrder { get; }
}
