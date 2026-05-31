using LeagueStack.Abstractions;
using Microsoft.AspNetCore.Html;

public class PlayRoundersPlugin : ITenantPlugin
{
    public string TenantId => "playrounders";

    public void ConfigureServices(ITenantServiceBuilder services)
    {
        services.AddScoped<IBrandingService, PlayRoundersBrandingService>();
    }
}

internal class PlayRoundersBrandingService : IBrandingService
{
    public async Task<IHtmlContent> GetLogo()
    {
        var assembly = typeof(PlayRoundersBrandingService).Assembly;
        const string resourceName = "LeagueStack.PlayRounders.Assets.PlayRoundersLogo.svg";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            var available = string.Join(", ", assembly.GetManifestResourceNames());
            throw new InvalidOperationException($"Embedded resource '{resourceName}' not found. Available: {available}");
        }

        using var reader = new StreamReader(stream);
        var svg = await reader.ReadToEndAsync();
        var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svg));
        return new HtmlString($@"<img src=""data:image/svg+xml;base64,{base64}"" alt=""Logo"" height=""32"" />");
    }
}