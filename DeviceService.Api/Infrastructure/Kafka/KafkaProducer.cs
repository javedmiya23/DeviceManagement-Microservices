using Confluent.Kafka;
using System.Text.Json;
using DeviceService.Api.Models.Commands;

namespace DeviceService.Api.Infrastructure.Kafka;

public class KafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration config)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task ProduceAsync(string topic, DeviceCommand command)
    {
        var message = new Message<string, string>
        {
            Key = command.CorrelationId,
            Value = JsonSerializer.Serialize(command)
        };

        await _producer.ProduceAsync(topic, message);
    }
}
