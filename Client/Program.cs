using Fluxor;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Thinktecture.Blazor.GrpcWeb.DevTools;
using ConfTool.Client;
using ConfTool.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("ConfTool.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ConfTool.ServerAPI"));

builder.Services.AddScoped(services =>
{
    var channel = GrpcChannel.ForAddress(builder.HostEnvironment.BaseAddress, 
        new GrpcChannelOptions 
        { 
            HttpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()) 
        });
        
    return channel;
});
builder.Services.AddGrpcService<IConferencesService>();
builder.Services.AddGrpcService<IContributionService>();
builder.Services.AddGrpcService<ISpeakersService>();
builder.Services.EnableGrpcWebDevTools();

builder.Services.AddFluxor(configure =>
{
    configure.ScanAssemblies(typeof(Program).Assembly);
    configure.UseReduxDevTools();
});

await builder.Build().RunAsync();
