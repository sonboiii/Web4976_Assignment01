using Microsoft.AspNetCore.Components;
namespace MEMS.Admin.Components;

public abstract class AdminPage : ComponentBase
{
    [Inject] protected AdminApi Api { get; set; } = null!;
    [Inject] protected ILogger<AdminPage> Logger { get; set; } = null!;
    protected string? Error;
    protected bool Busy;

    protected async Task Run(Func<Task> action)
    {
        if (Busy) return;
        Busy = true;
        Error = null;
        try { await action(); }
        catch (ApplicationException e) { Error = e.Message; }
        catch (Exception e) { Logger.LogError(e, "Administration request failed"); Error = "The election service is unavailable. Try again."; }
        finally { Busy = false; }
    }
}
