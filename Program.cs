
using CodeEffects.Demo.Asp;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();
Api.MapEndpoints(app);
app.MapFallbackToFile("index.html");

app.Run();