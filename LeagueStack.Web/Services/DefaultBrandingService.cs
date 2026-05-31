using LeagueStack.Abstractions;
using Microsoft.AspNetCore.Html;

namespace LeagueStack.Web.Services;

public class DefaultBrandingService : IBrandingService
{
    public Task<IHtmlContent> GetLogo()
    {
        const string Logo = "LeagueStack";
        return Task.FromResult<IHtmlContent>(new HtmlString(Logo));
    }
}