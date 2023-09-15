namespace RaspiApi;

public static class RestEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app, IServiceCollection services)
    {
        IHostedService backgroundService = services.BuildServiceProvider().GetRequiredService<IHostedService>();
        
        app.MapGet("/clock/start", () =>
        {
            if (backgroundService is ClockBackgroundService bgService)
            {
                bgService.StartAsync(default).GetAwaiter().GetResult();
            }
            return Results.Ok();
        });
        
        app.MapGet("/clock/stop", () =>
        {
            if (backgroundService is ClockBackgroundService bgService)
            {
                bgService.Stop();
            }
        });
    }
}