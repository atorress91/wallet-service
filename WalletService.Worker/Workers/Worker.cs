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
                _consumers.Add(new ProcessModelTwoConsumer(new ConsumerSettings
                {
                    Topics          = new[] { KafkaTopics.ProcessModelTwoTopic },
                    GroupId         = $"{ KafkaTopics.ProcessModelTwoTopic }-group",
                    GroupInstanceId = $"{ KafkaTopics.ProcessModelTwoTopic }-group-instance-{i}"
                }, _configuration, _logger, _serviceScopeFactory)
                {
                    ConsumerIndex = i
                });
            }

            for (var i = 0; i < consumerCountSettings!.ConsumersCount; i++)
            {
                _consumers.Add(new ProcessModelThreeWithInConsumer(new ConsumerSettings
                {
                    Topics            = new[] { KafkaTopics.ProcessModelThreeWithInTopic },
                    GroupId           = $"{ KafkaTopics.ProcessModelThreeWithInTopic }-group",
                    GroupInstanceId   = $"{ KafkaTopics.ProcessModelThreeWithInTopic }-group-instance-{i}"
                }, _configuration, _logger, _serviceScopeFactory)
                {
                    ConsumerIndex = i
                });
            }

            for (var i = 0; i < consumerCountSettings.ConsumersCount; i++)
            {
                _consumers.Add(new ProcessModelThreeWithOutConsumer(new ConsumerSettings
                {
                    Topics          = new[] { KafkaTopics.ProcessModelThreeWithOutTopic },
                    GroupId         = $"{ KafkaTopics.ProcessModelThreeWithOutTopic }-group",
                    GroupInstanceId = $"{ KafkaTopics.ProcessModelThreeWithOutTopic }-group-instance-{i}"
                }, _configuration, _logger, _serviceScopeFactory)
                {
                    ConsumerIndex = i
                });
            }            
            
            for (var i = 0; i < consumerCountSettings.ConsumersProcessPaymentCount; i++)
            {
                _consumers.Add(new ProcessPaymentModel2TwoThreeConsumer(new ConsumerSettings
                {
                    Topics          = new[] { KafkaTopics.ProcessPaymentModelTwoThreeTopic },
                    GroupId         = $"{ KafkaTopics.ProcessPaymentModelTwoThreeTopic }-group",
                    GroupInstanceId = $"{ KafkaTopics.ProcessPaymentModelTwoThreeTopic }-group-instance-{i}"
                }, _configuration, _logger, _serviceScopeFactory)
                {
                    ConsumerIndex = i
                });
            }
            
            for (var i = 0; i < consumerCountSettings.ConsumersProcessPaymentCount; i++)
            {
                _consumers.Add(new ProcessModelsFourFiveSixConsumer(new ConsumerSettings
                {
                    Topics          = new[] { KafkaTopics.ProcessModelFourFiveSixTopic },
                    GroupId         = $"{ KafkaTopics.ProcessModelFourFiveSixTopic }-group",
                    GroupInstanceId = $"{ KafkaTopics.ProcessModelFourFiveSixTopic }-group-instance-{i}"
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