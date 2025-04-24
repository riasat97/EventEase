using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EventEase;
using EventEase.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<EventState>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddLogging();

var app = builder.Build();

// Initialize session service
var sessionService = app.Services.GetRequiredService<SessionService>();
await sessionService.InitializeAsync();

await app.RunAsync();
