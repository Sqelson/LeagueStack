using Microsoft.AspNetCore.Html;

namespace LeagueStack.Abstractions;

public interface IBrandingService
{
    Task<IHtmlContent> GetLogo();
}
