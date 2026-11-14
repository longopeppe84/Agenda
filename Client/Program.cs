using Agenda2.Client;
using Agenda2.Client.Services; // <-- Assicurati che ci sia questo using
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// --- AGGIUNGI QUESTA RIGA ---
builder.Services.AddScoped<CartService>();
// -----------------------------

await builder.Build().RunAsync();