using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using UserService.Api.Models.Commands;
using UserService.Api.Services;

namespace UserService.Api.Infrastructure.Kafka;

public class UserCommandConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserCommandConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public UserCommandConsumer(
        IConfiguration configuration,
        ILogger<UserCommandConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "user-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe("user-commands");

        _logger.LogInformation("UserCommandConsumer started...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(TimeSpan.FromSeconds(1));

                if (result == null)
                    continue;

                var command = JsonSerializer.Deserialize<UserCommand>(result.Message.Value);

                if (command == null)
                    continue;

                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<UserProcessor>();

                await ProcessCommandAsync(processor, command);

                consumer.Commit(result);

                _logger.LogInformation(
                    "Processed command {CommandType} with CorrelationId {CorrelationId}",
                    command.CommandType,
                    command.CorrelationId);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Kafka message");
            }
        }

        consumer.Close();
    }

    private async Task ProcessCommandAsync(UserProcessor processor, UserCommand command)
    {
        switch (command.CommandType)
        {
            case "CreateUser":
                await processor.HandleCreateAsync(command.Payload);
                break;

            case "UpdateUser":
                await processor.HandleUpdateAsync(command.Payload);
                break;

            case "DeleteUser":
                await processor.HandleDeleteAsync(command.Payload);
                break;

            default:
                break;
        }
    }
}
