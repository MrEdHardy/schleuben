using AppUI.Components;
using AppUI.Configuration;
using AppUI.Services;
using MudBlazor.Services;
using Shared.Infrastructure.Configuration.OpenApi;
using Shared.Infrastructure.Extensions;

namespace AppUI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add MudBlazor services
        builder.Services.AddMudServices();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // Add json settings
        builder.Services.AddJsonSettings(builder.Configuration.GetSection("JsonSettings"));

        // Add appsettings
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));

        // Add resilience settings
        builder.Services.AddResilientHttpClientFactory(builder.Configuration, "app");

        // Add endpoint provider
        builder.Services.AddSingleton<IEndpointProviderService, AppEndpointProviderService>();

        // Add data service
        builder.Services.AddTransient<MinimalDataService>();

        // Add cancellation token source
        var tokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += async (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            await tokenSource.CancelAsync();
        };

        builder.Services.AddSingleton(tokenSource);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        var endpointProvider = app.Services.GetRequiredService<IEndpointProviderService>();
        await endpointProvider.InitializeEndpoints(tokenSource.Token);

        await app.RunAsync(tokenSource.Token);
    }
}
