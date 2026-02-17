using UserService.Api.Infrastructure.Kafka;
using UserService.Api.Repositories;
using UserService.Api.Services;
using StackExchange.Redis;
using UserService.Api.Caches.Implementations;
using UserService.Api.Caches.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UserService.Api.Middleware;
using UserService.Api.Audit;

using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);




builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddHostedService<UserCommandConsumer>();
builder.Services.AddControllers();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserProcessor>();
builder.Services.AddSingleton<IAuditQueue, AuditQueue>();
builder.Services.AddSingleton<MongoAuditRepository>();
builder.Services.AddHostedService<AuditBackgroundService>();


var useRedis = builder.Configuration.GetValue<bool>("UseRedis");

if (useRedis)
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(
            builder.Configuration["Redis:ConnectionString"]!));

    builder.Services.AddSingleton<IUserCache, RedisUserCache>();
}
else
{
    builder.Services.AddSingleton<IUserCache, InMemoryUserCache>();
}

var jwtKey = builder.Configuration["Jwt:Key"];
var key = Encoding.UTF8.GetBytes(jwtKey!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();
app.MapControllers();
app.Run();
