using LeagueStack.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeagueStack.Web.Data;

namespace LeagueStack.Web.Controllers;

public class StandingsController(AppDbContext dbContext, IPointsConfiguration pointsConfiguration) : Controller
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IPointsConfiguration _pointsConfiguration = pointsConfiguration;

    [Route("Standings")]
    public async Task<IActionResult> Index(string competition)
    {
        var fixtures = await _dbContext
            .Fixtures
            .Where(f => f.Competition == competition)
            .ToListAsync();

        var teamStats = new Dictionary<string, TeamRow>();

        foreach (var fixture in fixtures)
        {
            var home = GetOrAdd(teamStats, fixture.HomeTeam);

            var away = GetOrAdd(teamStats, fixture.AwayTeam);

            home.For += fixture.HomeTeamScore;
            home.Against += fixture.AwayTeamScore;
            away.For += fixture.AwayTeamScore;
            away.Against += fixture.HomeTeamScore;

            if (fixture.HomeTeamScore > fixture.AwayTeamScore)
            {
                home.Won++;
                home.Points += _pointsConfiguration.WinPoints;
                away.Lost++;
                away.Points += _pointsConfiguration.LossPoints;
            }
            else if (fixture.HomeTeamScore < fixture.AwayTeamScore)
            {
                away.Won++;
                away.Points += _pointsConfiguration.WinPoints;
                home.Lost++;
                home.Points += _pointsConfiguration.LossPoints;
            }
            else
            {
                home.Drawn++;
                away.Drawn++;
                home.Points += _pointsConfiguration.DrawPoints;
                away.Points += _pointsConfiguration.DrawPoints;
            }
        }

        var rows = ApplyOrder(teamStats.Values, _pointsConfiguration.StandingsOrder).ToList();

        for (int i = 0; i < rows.Count; i++)
            rows[i].Position = i + 1;

        var viewModel = new StandingsViewModel
        {
            Competition = competition,
            Rows = rows
        };

        return PartialView("_Standings", viewModel);
    }

    static IOrderedEnumerable<TeamRow> ApplyOrder(IEnumerable<TeamRow> rows, StandingsOrderField[] order)
    {
        var ordered = rows.OrderByDescending(r => GetField(r, order[0]));

        for (int i = 1; i < order.Length; i++)
        {
            var field = order[i];
            ordered = ordered.ThenByDescending(r => GetField(r, field));
        }

        return ordered;
    }

    static decimal GetField(TeamRow row, StandingsOrderField field) => field switch
    {
        StandingsOrderField.Points => row.Points,
        StandingsOrderField.Difference => row.Difference,
        StandingsOrderField.For => row.For,
        StandingsOrderField.Against => row.Against,
        StandingsOrderField.Won => row.Won,
        StandingsOrderField.Played => row.Played,
        _ => 0
    };

    static TeamRow GetOrAdd(Dictionary<string, TeamRow> stats, string team)
    {
        if (!stats.TryGetValue(team, out var row))
        {
            row = new TeamRow { Team = team };
            stats[team] = row;
        }

        return row;
    }

    public class StandingsViewModel
    {
        public required string Competition { get; set; }

        public required List<TeamRow> Rows { get; set; }
    }

    public class TeamRow
    {
        public int Position { get; set; }

        public required string Team { get; set; }

        public int Played => Won + Drawn + Lost;

        public int Won { get; set; }

        public int Drawn { get; set; }

        public int Lost { get; set; }

        public decimal For { get; set; }

        public decimal Against { get; set; }

        public decimal Difference => For - Against;

        public int Points { get; set; }
    }
}
