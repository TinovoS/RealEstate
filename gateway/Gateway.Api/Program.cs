var builder = WebApplication.CreateBuilder(args);

// CORS: origin-i iz konfiguracije, bilo koja metoda i zaglavlje; bez kolačića jer token ide u zaglavlju.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader()));

// Rute i klasteri postoje samo u sekciji "ReverseProxy" u konfiguraciji.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// CORS pre proxy-ja: preflight odgovara sam gateway, servis ga ne vidi.
app.UseCors();

// Zdravlje samog gateway-a; ostale servise ne proverava.
app.MapGet("/api/gateway/health", () => Results.Ok(new { status = "ok" }));

app.MapReverseProxy();

app.Run();
