using Case.Ui3.Components;
using Case.Ui3.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


#region Services

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<ConversationStateService>();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient("AzureAppClient", client =>
{
    //client.BaseAddress = new Uri("https://20250517-chatbot.azurewebsites.net");
    var apiSettingsBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    client.BaseAddress = new Uri(apiSettingsBaseUrl);
});

// Add this to the existing services in Program.cs
builder.Services.AddScoped<ConversationStateService>();
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
