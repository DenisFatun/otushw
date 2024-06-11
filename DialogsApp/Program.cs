using CommonLib.Data;
using CommonLib.Extensions;
using CommonLib.Handlers;
using DialogsApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry("DialogsApp");

// Add services to the container.
builder.Services.AddScoped<IDapperContext, DapperContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddServices();
builder.Services.AddRepositories();

builder.Services.AddSwaggerService(["v1", "v2"]);

var countsUrl = new Uri(builder.Configuration["CountsUrl"]);
builder.Services.AddHttpClient(CountsService.HttpClientName, httpClient => httpClient.BaseAddress = countsUrl);

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();

app.UseMiddleware<JwtMiddleware>();

app.AddSwaggerWebApp(["v1", "v2"]);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
