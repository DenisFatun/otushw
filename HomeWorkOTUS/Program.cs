using Dapper;
using HomeWorkOTUS.Data;
using HomeWorkOTUS.Extensions;
using HomeWorkOTUS.Handlers;
using HomeWorkOTUS.Services.RabbitMq;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<IDapperSlaveContext, DapperSlaveContext>();

builder.Services.AddServices();
builder.Services.AddRepositories();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=> {
    c.DescribeAllParametersInCamelCase();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new List<string>()
                        }
                    });
});
builder.Services.AddHttpClient();

builder.Services.AddTransient<AddPostConsumer>();
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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.UseMiddleware<JwtMiddleware>();

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
            )

            TABLESPACE pg_default;

            ALTER TABLE clients OWNER to postgres;
            
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
                CONSTRAINT fk_client FOREIGN KEY(client_id) REFERENCES clients(id)
            );
        "
    );
}

app.Run();
