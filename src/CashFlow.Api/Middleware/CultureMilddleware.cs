using System.Globalization;

namespace CashFlow.Api.Middleware;

public class CultureMilddleware
{
    private readonly RequestDelegate _next;

    public CultureMilddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        var supportedCulture = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            
        var requestedCulture = context.Request.Headers.AcceptLanguage.FirstOrDefault();

        var cultureInfo = new CultureInfo("pt-BR");

        if (string.IsNullOrWhiteSpace(requestedCulture) == false && supportedCulture.Exists(l => l.Name.Equals(requestedCulture)))
        {
            cultureInfo = new CultureInfo(requestedCulture);

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        await _next(context);
    }
}
