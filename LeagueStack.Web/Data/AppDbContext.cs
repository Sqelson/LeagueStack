using Microsoft.EntityFrameworkCore;

namespace LeagueStack.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Fixture> Fixtures => Set<Fixture>();
}