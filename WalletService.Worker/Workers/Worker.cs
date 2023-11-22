using Microsoft.Extensions.Options;
using WalletService.Core.Kafka.Consumers;
using WalletService.Core.Kafka.Topics;
using WalletService.Models.Configuration;

namespace WalletService.Worker.Workers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker>          _logger;
        private readonly IServiceScopeFactory     _serviceScopeFactory;
        private          Task?                     _executingTask;
        private          CancellationTokenSource  _cts;
        private readonly List<BaseKafkaConsumer>  _consumers = new();
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
            {
                item.StopConsume();
            }

            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogWarning("Wallet Worker service stopped.");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerCountSettings = _configuration.ConsumersSetting;

            for (var i = 0; i < consumerCountSettings!.ConsumersCount; i++)
            {
                _consumers.Add(new ProcessEcoPoolWithInConsumer(new ConsumerSettings
                {
                    Topics            = new[] { KafkaTopics.ProcessEcoPoolWithInTopic },
                    GroupId           = $"{ KafkaTopics.ProcessEcoPoolWithInTopic }-group",
                    GroupInstanceId   = $"{ KafkaTopics.ProcessEcoPoolWithInTopic }-group-instance-{i}"
                }, _configuration, _logger, _serviceScopeFactory)
                {
                    ConsumerIndex = i
                });
            }

            for (var i = 0; i < consumerCountSettings.ConsumersCount; i++)
            {
                _consumers.Add(new ProcessEcoPoolWithOutConsumer(new ConsumerSettings
                {
                    Topics          = new[] { KafkaTopics.ProcessEcoPoolWithOutTopic },
                    GroupId         = $"{ KafkaTopics.ProcessEcoPoolWithOutTopic }-group",
                    GroupInstanceId = $"{ KafkaTopics.ProcessEcoPoolWithOutTopic }-group-instance-{i}"
                }, _configuration, _logger, _serviceScopeFactory)
                {
                    ConsumerIndex = i
                });
            }            
            
            for (var i = 0; i < consumerCountSettings.ConsumersProcessPaymentCount; i++)
            {
                _consumers.Add(new ProcessPaymentEcoPoolConsumer(new ConsumerSettings
                {
                    Topics          = new[] { KafkaTopics.ProcessEcoPoolPaymentTopic },
                    GroupId         = $"{ KafkaTopics.ProcessEcoPoolPaymentTopic }-group",
                    GroupInstanceId = $"{ KafkaTopics.ProcessEcoPoolPaymentTopic }-group-instance-{i}"
                }, _configuration, _logger, _serviceScopeFactory)
                {
                    ConsumerIndex = i
                });
            }
            
            for (var i = 0; i < consumerCountSettings.ConsumersProcessPaymentCount; i++)
            {
                _consumers.Add(new ProcessModelsConsumer(new ConsumerSettings
                {
                    Topics          = new[] { KafkaTopics.ProcessModelFourTopic },
                    GroupId         = $"{ KafkaTopics.ProcessModelFourTopic }-group",
                    GroupInstanceId = $"{ KafkaTopics.ProcessModelFourTopic }-group-instance-{i}"
                }, _configuration, _logger, _serviceScopeFactory)
                {
                    ConsumerIndex = i
                });
            }
            
            foreach (var consumer in _consumers)
            {
                consumer.Consume();
            }

            _logger.LogInformation("Start Consumers Done");
            return Task.CompletedTask;
        }
    }