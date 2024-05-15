using CommonLib.Data;
using CommonLib.Extensions;
using CommonLib.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.AddOpenTelemetry("DialogsApp");
builder.Services.AddOpenTelemetry("DialogsApp");

// Add services to the container.
builder.Services.AddScoped<IDapperContext, DapperContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddSwaggerService(["v1", "v2"]);

var app = builder.Build();

app.UseMiddleware<JwtMiddleware>();

app.AddSwaggerWebApp(["v1", "v2"]);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
