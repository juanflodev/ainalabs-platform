using NodusLab.Web.Components;
using Microsoft.EntityFrameworkCore;
using NodusLab.Core.Interfaces;
using NodusLab.Infrastructure;
using NodusLab.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddDbContext<AinaDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"), 
        o => o.UseVector()
    ).UseSnakeCaseNamingConvention());

builder.Services.AddHttpClient<AiEngineClient>(client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("AiEngine:BaseUrl");
    client.BaseAddress = new Uri(baseUrl!);
});

builder.Services.AddAuthentication(options =>
{
    // Todo: pendiente de configuración OIDC/Cookies más adelante
});


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();