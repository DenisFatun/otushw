using CommonLib.Data;
using CommonLib.Extensions;
using Dapper;
using HomeWorkOTUS.Services.RabbitMq;
using HomeWorkOTUS.Services.SignalR;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {

                var accessToken = context.Request.Query["access_token"];

                // если запрос направлен хабу
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/post/feed/posted"))
                {
                    // получаем токен из строки запроса
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<IDapperContext, DapperContext>();
//builder.Services.AddScoped<IDapperSlaveContext, DapperSlaveContext>();

builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddSwaggerService(["v1", "v2"]);

builder.Services.AddHttpClient();

builder.Services.AddTransient<AddPostConsumer>();
builder.Services.AddTransient<AddedPostConsumer>();
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.UsingRabbitMq((context, cfg) =>
    {
        var uri = new Uri(builder.Configuration["RabbitMqUri"]);
        cfg.Host(uri);        
        cfg.ReceiveEndpoint("add_post", e =>
        {
            e.Consumer<AddPostConsumer>(context);
        });
        cfg.ReceiveEndpoint(Guid.NewGuid().ToString(), e =>
        {
            e.Durable = false;
            e.AutoDelete = true;
            e.Consumer<AddedPostConsumer>(context);
        });
    });    
});

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

//var tarantoolUrl = new Uri(builder.Configuration["TarantoolUrl"]);
//builder.Services.AddHttpClient(PostsRepoTarantool.HttpClientName, httpClient => httpClient.BaseAddress = tarantoolUrl);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.AddDefaultWebApp();

app.AddSwaggerWebApp(["v1", "v2"]);

using (var scope = app.Services.CreateScope())
{
    var _db = scope.ServiceProvider.GetRequiredService<IDapperContext>();
    await _db.Connection.ExecuteAsync(
        @"CREATE TABLE IF NOT EXISTS clients
            (
                id uuid NOT NULL,
                name character varying(50) NOT NULL,
                ser_name character varying(50) NOT NULL,
                age integer NOT NULL,
                is_male boolean NOT NULL,
                interests character varying(300),
                city character varying(50),
                creationdate timestamp NOT NULL,
                password character varying(512) NOT NULL,
                CONSTRAINT pl_clients PRIMARY KEY (id)
            );
            
            CREATE INDEX IF NOT EXISTS btree_clients_name_sername_ind ON public.clients USING btree (ser_name text_pattern_ops, name text_pattern_ops);

            CREATE TABLE IF NOT EXISTS clients_friends
            (
                id INT GENERATED ALWAYS AS IDENTITY,
                client_id uuid NOT NULL,
                friend_id uuid NOT NULL,
                CONSTRAINT fk_client FOREIGN KEY(client_id) REFERENCES clients(id),
                CONSTRAINT fk_friend FOREIGN KEY(friend_id) REFERENCES clients(id),
                UNIQUE (client_id, friend_id)
            );
    
            CREATE TABLE IF NOT EXISTS posts
            (
                id INT GENERATED ALWAYS AS IDENTITY,
                client_id uuid NOT NULL,
                created_at TIMESTAMP DEFAULT NOW(),
                post_text text,
                CONSTRAINT pk_posts PRIMARY KEY (id),
                CONSTRAINT fk_client FOREIGN KEY(client_id) REFERENCES clients(id)
            );

            CREATE TABLE IF NOT EXISTS dialogs
            (
                id INT GENERATED ALWAYS AS IDENTITY,
                to_client_id uuid NOT NULL,
                from_client_id uuid NOT NULL,
                created_at TIMESTAMP DEFAULT NOW(),
                message text,
                CONSTRAINT pk_dialogs PRIMARY KEY (id, to_client_id)                
            );
        "
    );
}

app.MapHub<FeedPostedHub>("/post/feed/posted");
app.UseHttpsRedirection();

app.Run();
