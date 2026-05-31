using LeagueStack.Abstractions;
using Microsoft.EntityFrameworkCore;
using LeagueStack.Web.Data;
using LeagueStack.Web.Middleware;
using LeagueStack.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var plugins = TenantPluginLoader.DiscoverPlugins();

var tenants = builder.Configuration
    .GetSection("Multitenancy:Tenants")
    .Get<List<TenantConfig>>() ?? [];

builder.Services.AddSingleton<IReadOnlyList<TenantConfig>>(tenants);

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var tenantContext = serviceProvider.GetRequiredService<TenantContext>();

    var config = serviceProvider.GetRequiredService<IConfiguration>();

    var template = config.GetConnectionString("Default")!;

    var tenantId = tenantContext.HasTenant
        ? tenantContext.TenantId!
        : tenants.First().TenantId;

    var connString = BuildConnectionString(template, tenantId, tenants);

    options.UseSqlServer(connString);
});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<TenantContext>();

var registry = new TenantServiceRegistry();

registry.RegisterDefault<IPointsConfiguration, DefaultPointsConfiguration>();
registry.RegisterDefault<IBrandingService, DefaultBrandingService>();

foreach (var plugin in plugins)
    plugin.ConfigureServices(new TenantServiceBuilder(plugin.TenantId, registry));

builder.Services.AddSingleton(registry);

foreach (var serviceType in registry.DefaultImplementations.Keys)
{
    var capturedServiceType = serviceType;

    var defaultImplType = registry.DefaultImplementations[capturedServiceType];

    builder.Services.AddScoped(capturedServiceType, implementationFactory: sp =>
    {
        var tenantContext = sp.GetRequiredService<TenantContext>();

        if (tenantContext.HasTenant)
        {
            var implType = registry.GetImplementationType(tenantContext.TenantId!, capturedServiceType);

            if (implType != null)
                return ActivatorUtilities.CreateInstance(sp, implType);
        }

        return ActivatorUtilities.CreateInstance(sp, defaultImplType);
    });
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var template = config.GetConnectionString("Default")!;

    foreach (var tenant in tenants)
    {
        var connString = BuildConnectionString(template, tenant.TenantId, tenants);

        await MigrateDatabase(connString);
        await SeedDatabase(connString, tenant.TenantId);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<TenantResolutionMiddleware>();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static string BuildConnectionString(string template, string tenantId, IReadOnlyList<TenantConfig> tenants)
{
    var tenant = tenants.FirstOrDefault(t => t.TenantId == tenantId);

    var database = tenant?.Database ?? tenantId;

    return string.Format(template, database);
}

static async Task SeedDatabase(string connectionString, string tenantId)
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(connectionString)
        .Options;

    await using var dbContext = new AppDbContext(options);
    await DbSeeder.SeedAsync(dbContext, tenantId);
}

static async Task MigrateDatabase(string connectionString)
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(connectionString)
        .Options;

    await using var dbContext = new AppDbContext(options);
    await dbContext.Database.MigrateAsync();
}