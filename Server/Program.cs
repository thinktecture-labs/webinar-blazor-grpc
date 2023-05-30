using ConfTool.Server.Database;
using ConfTool.Server.Services;
using ConfTool.Server.Utils;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddDbContext<ConfToolDbContext>(options => options.UseInMemoryDatabase("ConfTool"));
builder.Services.AddScoped<ContributionsService>();
builder.Services.AddScoped<SpeakersService>();
builder.Services.AddScoped<ConferencesService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DataGenerator.InitializeAsync(services);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseGrpcWeb();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
