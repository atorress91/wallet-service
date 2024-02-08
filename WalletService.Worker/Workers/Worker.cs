using Microsoft.Extensions.Options;
using WalletService.Core.Kafka.Consumers;
using WalletService.Core.Kafka.Topics;
using WalletService.Models.Configuration;

namespace WalletService.Worker.Workers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Task? _executingTask;
    private CancellationTokenSource _cts;
    private readonly List<BaseKafkaConsumer> _consumers = new();
    private readonly ApplicationConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IOptions<ApplicationConfiguration> configuration, IServiceScopeFactory serviceScopeFactory)
    {
        _logger              = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _configuration       = configuration.Value;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("Fortuna Communication Service Worker service started.");

        _cts           = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = ExecuteAsync(_cts.Token);
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask is null)
            return;

        _logger.LogWarning("Wallet Worker service stopping.");
        _cts.Cancel();

        foreach (var item in _consumers)
            item.StopConsume();

        await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogWarning("Wallet Worker service stopped.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerCountSettings = _configuration.ConsumersSetting;

        for (var i = 0; i < consumerCountSettings!.ConsumersCount; i++)
            _consumers.Add(new ProcessModel3Consumer(new ConsumerSettings
            {
                Topics          = new[] { KafkaTopics.ProcessModel3Topic },
                GroupId         = $"{KafkaTopics.ProcessModel3Topic}-group",
                GroupInstanceId = $"{KafkaTopics.ProcessModel3Topic}-group-instance-{i}"
            }, _configuration, _logger, _serviceScopeFactory)
            {
                ConsumerIndex = i
            });

        for (var i = 0; i < consumerCountSettings!.ConsumersCount; i++)
            _consumers.Add(new ProcessModel2Consumer(new ConsumerSettings
            {
                Topics          = new[] { KafkaTopics.ProcessModel2Topic },
                GroupId         = $"{KafkaTopics.ProcessModel2Topic}-group",
                GroupInstanceId = $"{KafkaTopics.ProcessModel2Topic}-group-instance-{i}"
            }, _configuration, _logger, _serviceScopeFactory)
            {
                ConsumerIndex = i
            });
        
        for (var i = 0; i < consumerCountSettings!.ConsumersCount; i++)
            _consumers.Add(new ProcessModel1BConsumer(new ConsumerSettings
            {
                Topics          = new[] { KafkaTopics.ProcessModel1BTopic },
                GroupId         = $"{KafkaTopics.ProcessModel1BTopic}-group",
                GroupInstanceId = $"{KafkaTopics.ProcessModel1BTopic}-group-instance-{i}"
            }, _configuration, _logger, _serviceScopeFactory)
            {
                ConsumerIndex = i
            });

        for (var i = 0; i < consumerCountSettings!.ConsumersCount; i++)
            _consumers.Add(new ProcessModel1AConsumer(new ConsumerSettings
            {
                Topics          = new[] { KafkaTopics.ProcessModel1ATopic },
                GroupId         = $"{KafkaTopics.ProcessModel1ATopic}-group",
                GroupInstanceId = $"{KafkaTopics.ProcessModel1ATopic}-group-instance-{i}"
            }, _configuration, _logger, _serviceScopeFactory)
            {
                ConsumerIndex = i
            });

        for (var i = 0; i < consumerCountSettings.ConsumersProcessPaymentCount; i++)
            _consumers.Add(new ProcessPaymentModel1A1B23Consumer(new ConsumerSettings
            {
                Topics          = new[] { KafkaTopics.ProcessPaymentModelTwoThreeTopic },
                GroupId         = $"{KafkaTopics.ProcessPaymentModelTwoThreeTopic}-group",
                GroupInstanceId = $"{KafkaTopics.ProcessPaymentModelTwoThreeTopic}-group-instance-{i}"
            }, _configuration, _logger, _serviceScopeFactory)
            {
                ConsumerIndex = i
            });

        for (var i = 0; i < consumerCountSettings.ConsumersProcessPaymentCount; i++)
            _consumers.Add(new ProcessModelsFourFiveSixConsumer(new ConsumerSettings
            {
                Topics          = new[] { KafkaTopics.ProcessModelFourFiveSixTopic },
                GroupId         = $"{KafkaTopics.ProcessModelFourFiveSixTopic}-group",
                GroupInstanceId = $"{KafkaTopics.ProcessModelFourFiveSixTopic}-group-instance-{i}"
            }, _configuration, _logger, _serviceScopeFactory)
            {
                ConsumerIndex = i
            });

        foreach (var consumer in _consumers)
            consumer.Consume();

        _logger.LogInformation("Start Consumers Done");
        return Task.CompletedTask;
    }
}