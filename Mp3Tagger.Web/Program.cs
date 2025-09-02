using Mp3Tagger.Core;
using Mp3Tagger.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddLogging(z =>
{
    var seqConnectionString = builder.Configuration.GetConnectionString("Seq");
    if (!string.IsNullOrWhiteSpace(seqConnectionString))
    {
        z.AddSeq(seqConnectionString);
    }
});
builder.Services.AddMp3Tagger(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();