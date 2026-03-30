using Confluent.Kafka;
using System.Text.Json;

namespace NotificationService.Messaging;

public class DlqPublisher
{
    private readonly IProducer<Null, string> _producer;

    public DlqPublisher(IConfiguration config)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    public async Task PublishAsync(string topic, object message)
    {
        var json = JsonSerializer.Serialize(message);

        await _producer.ProduceAsync(topic,
            new Message<Null, string> { Value = json });
    }
}

