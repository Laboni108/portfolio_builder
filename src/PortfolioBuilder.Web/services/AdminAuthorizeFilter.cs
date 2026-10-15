using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PortfolioBuilder.Web.Services;

public class AdminAuthorizeFilter : IAsyncPageFilter
{
    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        => Task.CompletedTask;

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        if (context.ActionDescriptor.ViewEnginePath == "/Admin/Login")
        {
            await next();
            return;
        }

        var http = context.HttpContext;
        var auth = http.RequestServices.GetRequiredService<AdminAuthService>();

        if (!await auth.TryRestoreFromCookieAsync(http))
        {
            string returnUrl = http.Request.Path + http.Request.QueryString;
            context.Result = new RedirectResult("/Admin/Login?returnUrl=" + Uri.EscapeDataString(returnUrl));
            return;
        }

        await next();
    }
}