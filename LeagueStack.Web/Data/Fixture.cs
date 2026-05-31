namespace LeagueStack.Web.Data;

public class Fixture
{
    public int Id { get; set; }

    public required string Competition { get; set; }

    public int Round { get; set; }

    public DateTime DateTime { get; set; }

    public required string Venue { get; set; }

    public required string Pitch { get; set; }

    public required string HomeTeam { get; set; }

    public required string AwayTeam { get; set; }

    public decimal HomeTeamScore { get; set; }

    public decimal AwayTeamScore { get; set; }
}