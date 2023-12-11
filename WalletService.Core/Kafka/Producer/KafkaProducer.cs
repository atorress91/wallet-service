using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WalletService.Models.Configuration;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Kafka.Producer;

public class KafkaProducer
{
    private readonly ProducerConfig _producerConfig;
    public KafkaProducer(IServiceCollection serviceCollection)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var settings = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>().Value;
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = settings.ConsumersSetting!.BrokerList,
            BatchNumMessages = 16384 * 1,
            LingerMs         = 1,
            CompressionType  = CompressionType.Gzip,
            Acks             = Acks.Leader,
            MessageMaxBytes  = 20000000
        };
    }

    private async Task<DeliveryResult<Null, string>> ProduceAsync<T>(string topic, T source)
    {
        var       value    = source as string ?? source.ToJsonString();
        using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
        return await producer.ProduceAsync(topic, new Message<Null, string>
        {
            Value = value
        });
    }

    public async Task<DeliveryResult<Null, string>> ProduceAsync(string topic, string source)
    {
        return await ProduceAsync<string>(topic, source);
    }

    public async Task<DeliveryResult<string, string>> ProduceAsync(string topic, string source, string key)
    {
        return await ProduceWithKeyAsync(topic, source, key);
    }

    public async Task<DeliveryResult<string, string>> ProduceWithKeyAsync<T>(string topic, T source, string key)
    {
        try
        {
            var       value    = source as string ?? source.ToJsonString();
            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
            return await producer.ProduceAsync(topic, new Message<string, string>
            {
                Value = value,
                Key   = key
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}