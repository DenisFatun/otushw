using CommonLib.Data;
using CommonLib.Extensions;
using CommonLib.Handlers;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddOpenTelemetry("CountsApp");
builder.Services.AddOpenTelemetry("CountsApp");

builder.Services.AddScoped<IDapperContext, DapperContext>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices();
builder.Services.AddRepositories();

builder.Services.AddSwaggerService(["v1", "v2"]);

builder.Services.AddSingleton(config =>
{
    var configurationOptions = new ConfigurationOptions
    {
        EndPoints = { builder.Configuration["Redis:Host"] },
        Password = builder.Configuration["Redis:Password"]
    };

    IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(configurationOptions);
    var redisDb = Convert.ToInt32(builder.Configuration["Redis:Db"]);
    return multiplexer.GetDatabase(redisDb);
});

var app = builder.Build();

app.UseMiddleware<JwtMiddleware>();

app.AddSwaggerWebApp(["v1", "v2"]);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
