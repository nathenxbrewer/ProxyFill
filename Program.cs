using AutoMapper;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProxyFill;
using MudBlazor.Services;
using ProxyFill.Domain.Services;
using ProxyFill.Mapper;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddHttpClient();
var mapperConfiguration = new MapperConfiguration(configuration =>
{
    configuration.AddProfile(new ProxyCardProfile());
});

var mapper = mapperConfiguration.CreateMapper();
var PokemonAPIService = new PokemonAPIService();
builder.Services.AddSingleton(mapper);
builder.Services.AddSingleton(PokemonAPIService);
await builder.Build().RunAsync();