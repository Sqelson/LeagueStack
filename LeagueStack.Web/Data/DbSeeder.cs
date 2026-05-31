using Microsoft.EntityFrameworkCore;

namespace LeagueStack.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, string tenantId)
    {
        if (await db.Fixtures.AnyAsync())
            return;

        List<Fixture> fixtures = tenantId switch
        {
            "kent5aside" => SeedKent5aside(),
            "playrounders" => SeedPlayRounders(),
            "simplecricket" => SeedSimpleCricket(),
            _ => []
        };

        db.Fixtures.AddRange(fixtures);
        await db.SaveChangesAsync();
    }

    static List<Fixture> SeedKent5aside()
    {
        var teams = new[]
        {
            "Maidstone FC", "Canterbury City", "Tunbridge Wells", "Ashford United",
            "Dover Athletic", "Folkestone Town", "Gravesend Flyers", "Dartford Dynamos"
        };
        var venues = new[] { "Kent 5s Arena", "Canterbury Sports Centre", "Maidstone Leisure Centre", "Ashford Indoor Pitches" };
        var pitches = new[] { "Pitch 1", "Pitch 2", "Pitch 3", "Pitch 4" };
        var rng = new Random(42);

        var fixtures = new List<Fixture>();

        fixtures.AddRange(GenerateRoundRobin(
            "Spring 2025", teams, venues, pitches,
            new DateTime(2025, 3, 3, 19, 0, 0), 10, 7,
            () => rng.Next(0, 9), rng));

        fixtures.AddRange(GenerateRoundRobin(
            "Summer 2025", teams, venues, pitches,
            new DateTime(2025, 6, 2, 19, 0, 0), 10, 7,
            () => rng.Next(0, 9), rng));

        return fixtures;
    }

    static List<Fixture> SeedPlayRounders()
    {
        var teams = new[]
        {
            "The Sluggers", "Bat & Ball", "The Fielders", "Round Trippers",
            "Base Runners", "The Catchers", "Diamond Dogs", "The Bowlers"
        };
        var venues = new[] { "Battersea Park", "Clapham Common", "Hyde Park", "Regent's Park" };
        var pitches = new[] { "Field A", "Field B" };
        var rng = new Random(123);

        var fixtures = new List<Fixture>();

        fixtures.AddRange(GenerateRoundRobin(
            "Early Spring 2025", teams, venues, pitches,
            new DateTime(2025, 3, 1, 10, 0, 0), 5, 7,
            () => rng.Next(0, 31) / 2m, rng));

        fixtures.AddRange(GenerateRoundRobin(
            "Late Spring 2025", teams, venues, pitches,
            new DateTime(2025, 4, 12, 10, 0, 0), 5, 7,
            () => rng.Next(0, 31) / 2m, rng));

        fixtures.AddRange(GenerateKnockout(
            "Spring Tourno 2025", teams, venues[0], pitches,
            new DateTime(2025, 5, 17, 9, 0, 0),
            () => rng.Next(0, 31) / 2m, rng));

        return fixtures;
    }

    static List<Fixture> SeedSimpleCricket()
    {
        var allTeams = new[]
        {
            "Oakfield CC", "Riverside CC", "Hilltop CC", "Meadow CC",
            "Parkside CC", "Valley CC", "Lakeside CC", "Woodland CC",
            "Heathrow CC", "Greenfield CC", "Stonebridge CC", "Millbrook CC"
        };
        var venues = new[] { "Village Green", "The Oval", "Recreation Ground", "Memorial Ground" };
        var pitches = new[] { "Main Strip", "Practice Strip" };
        var rng = new Random(456);

        var fixtures = new List<Fixture>();

        // Season 2023: 10 teams, 9 rounds
        fixtures.AddRange(GenerateRoundRobin(
            "Season 2023", allTeams[..10], venues, pitches,
            new DateTime(2023, 4, 22, 13, 0, 0), 9, 7,
            () => rng.Next(80, 280), rng));

        // Season 2024: 8 teams, 10 rounds
        fixtures.AddRange(GenerateRoundRobin(
            "Season 2024", allTeams[..8], venues, pitches,
            new DateTime(2024, 4, 20, 13, 0, 0), 10, 7,
            () => rng.Next(80, 280), rng));

        // Season 2025: 12 teams, 11 rounds
        fixtures.AddRange(GenerateRoundRobin(
            "Season 2025", allTeams, venues, pitches,
            new DateTime(2025, 4, 19, 13, 0, 0), 11, 7,
            () => rng.Next(80, 280), rng));

        // 7 one-day knockout competitions
        var oneDayComps = new (string Name, DateTime Date)[]
        {
            ("Summer Cup 2023", new DateTime(2023, 7, 15, 10, 0, 0)),
            ("Charity Shield 2023", new DateTime(2023, 9, 2, 10, 0, 0)),
            ("Summer Cup 2024", new DateTime(2024, 6, 22, 10, 0, 0)),
            ("Charity Shield 2024", new DateTime(2024, 8, 31, 10, 0, 0)),
            ("Presidents Trophy 2024", new DateTime(2024, 9, 14, 10, 0, 0)),
            ("Summer Cup 2025", new DateTime(2025, 6, 21, 10, 0, 0)),
            ("Charity Shield 2025", new DateTime(2025, 8, 30, 10, 0, 0)),
        };

        foreach (var (name, date) in oneDayComps)
        {
            var teams = allTeams.OrderBy(_ => rng.Next()).Take(8).ToArray();
            fixtures.AddRange(GenerateKnockout(
                name, teams, venues[rng.Next(venues.Length)], pitches,
                date, () => rng.Next(80, 280), rng));
        }

        return fixtures;
    }

    static List<Fixture> GenerateRoundRobin(
        string competition, string[] teams, string[] venues, string[] pitches,
        DateTime startDate, int rounds, int daysBetweenRounds,
        Func<decimal> scoreGenerator, Random rng)
    {
        var fixtures = new List<Fixture>();
        int n = teams.Length;

        for (int round = 0; round < rounds; round++)
        {
            var roundDate = startDate.AddDays(round * daysBetweenRounds);
            int r = round % (n - 1);

            var rotated = new int[n];
            rotated[0] = 0;
            for (int i = 1; i < n; i++)
                rotated[i] = 1 + (r + i - 1) % (n - 1);

            for (int i = 0; i < n / 2; i++)
            {
                int home = rotated[i];
                int away = rotated[n - 1 - i];

                if (round >= n - 1)
                    (home, away) = (away, home);

                fixtures.Add(new Fixture
                {
                    Competition = competition,
                    Round = round + 1,
                    DateTime = roundDate.AddMinutes(i * 15),
                    Venue = venues[rng.Next(venues.Length)],
                    Pitch = pitches[rng.Next(pitches.Length)],
                    HomeTeam = teams[home],
                    AwayTeam = teams[away],
                    HomeTeamScore = scoreGenerator(),
                    AwayTeamScore = scoreGenerator()
                });
            }
        }

        return fixtures;
    }

    static List<Fixture> GenerateKnockout(
        string competition, string[] teams, string venue, string[] pitches,
        DateTime matchDay, Func<decimal> scoreGenerator, Random rng)
    {
        var fixtures = new List<Fixture>();
        var remaining = teams.ToList();
        int round = 1;
        var time = matchDay;

        while (remaining.Count > 1)
        {
            var nextRound = new List<string>();

            for (int i = 0; i < remaining.Count; i += 2)
            {
                var homeScore = scoreGenerator();
                var awayScore = scoreGenerator();

                while (homeScore == awayScore)
                    awayScore = scoreGenerator();

                fixtures.Add(new Fixture
                {
                    Competition = competition,
                    Round = round,
                    DateTime = time,
                    Venue = venue,
                    Pitch = pitches[rng.Next(pitches.Length)],
                    HomeTeam = remaining[i],
                    AwayTeam = remaining[i + 1],
                    HomeTeamScore = homeScore,
                    AwayTeamScore = awayScore
                });

                time = time.AddMinutes(45);
                nextRound.Add(homeScore > awayScore ? remaining[i] : remaining[i + 1]);
            }

            remaining = nextRound;
            round++;
        }

        return fixtures;
    }
}
