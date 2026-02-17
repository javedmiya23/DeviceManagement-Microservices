using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using DeviceService.Api.Models.Commands;
using DeviceService.Api.Services;

namespace DeviceService.Api.Infrastructure.Kafka;

public class DeviceCommandConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeviceCommandConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DeviceCommandConsumer(
        IConfiguration configuration,
        ILogger<DeviceCommandConsumer> logger,
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
            GroupId = "device-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe("device-commands");

        _logger.LogInformation("DeviceCommandConsumer started...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(TimeSpan.FromSeconds(1));

                if (result == null)
                    continue;

                var command = JsonSerializer.Deserialize<DeviceCommand>(result.Message.Value);

                if (command == null)
                {
                    _logger.LogWarning("Invalid message received");
                    continue;
                }

                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<DeviceProcessor>();

                await ProcessCommandAsync(processor, command);

                consumer.Commit(result);

                _logger.LogInformation(
                    "Processed {CommandType} with CorrelationId {CorrelationId}",
                    command.CommandType,
                    command.CorrelationId);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DeviceCommandConsumer");
            }
        }

        consumer.Close();
    }

    private async Task ProcessCommandAsync(DeviceProcessor processor, DeviceCommand command)
    {
        switch (command.CommandType)
        {
            case "CreateDevice":
                await processor.HandleCreateAsync(command.Payload);
                break;

            case "UpdateDevice":
                await processor.HandleUpdateAsync(command.Payload);
                break;

            case "DeleteDevice":
                await processor.HandleDeleteAsync(command.Payload);
                break;

            default:
                _logger.LogWarning("Unknown command type: {CommandType}", command.CommandType);
                break;
        }
    }
}
