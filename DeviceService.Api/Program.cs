using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DeviceService.Api.Repositories;
using DeviceService.Api.Infrastructure.Kafka;
using DeviceService.Api.Services;
using DeviceService.Api.Caches.Implementations;
using DeviceService.Api.Caches.Interfaces;
using DeviceService.Api.Audit;
using DeviceService.Api.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var jwtKey = builder.Configuration["Jwt:Key"];
var key = Encoding.UTF8.GetBytes(jwtKey!);

builder.Services.AddScoped<DeviceRepository>();
builder.Services.AddScoped<DeviceProcessor>();

builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddHostedService<DeviceCommandConsumer>();


if (builder.Configuration.GetValue<bool>("UseRedis"))
{
    builder.Services.AddSingleton<IDeviceCache, RedisDeviceCache>();
}
else
{
    builder.Services.AddSingleton<IDeviceCache, InMemoryDeviceCache>();
}

builder.Services.AddSingleton<IAuditQueue, AuditQueue>();
builder.Services.AddSingleton<MongoAuditRepository>();
builder.Services.AddHostedService<AuditBackgroundService>();


builder.Services.AddSingleton<MongoLogRepository>();

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

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.Run();
