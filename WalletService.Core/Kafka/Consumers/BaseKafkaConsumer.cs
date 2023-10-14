using System.Net.NetworkInformation;
using Confluent.Kafka;
using Elastic.Apm;
using Elastic.Apm.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalletService.Models.Configuration;
using WalletService.Models.Constants;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Kafka.Consumers;

public abstract class BaseKafkaConsumer
{
    public int ConsumerIndex { get; set; }
    private readonly ConsumerSettings _consumerSettings;
    private Guid InstanceId { get; set; }
    private readonly ApplicationConfiguration _configuration;
    protected readonly ILogger Logger;
    private IConsumer<Ignore, string> _consumer;
    private Thread _thread;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private ConsumeResult<Ignore, string> _consumeResult;
    protected readonly IServiceScopeFactory ServiceScopeFactory;

    private bool Enabled { get; set; }
    private readonly object _locker = new();

    protected BaseKafkaConsumer(
        ConsumerSettings           consumerSettings,
        ApplicationConfiguration   configuration,
        ILogger logger,
        IServiceScopeFactory       serviceScopeFactory
    )
    {
        InstanceId               = Guid.NewGuid();
        Logger                   = logger;
        _consumerSettings        = consumerSettings;
        _configuration           = configuration;
        _cancellationTokenSource = new CancellationTokenSource();
        ServiceScopeFactory      = serviceScopeFactory;
        Connect();
    }

    private void Connect()
    {
        var conf = new ConsumerConfig
        {
            BootstrapServers               = _configuration.ConsumersSetting?.BrokerList,
            FetchMaxBytes                  = 1048576 * 1,
            AutoOffsetReset                = _consumerSettings.ReadLast ? AutoOffsetReset.Latest : AutoOffsetReset.Earliest,
            MaxPartitionFetchBytes         = 1048576 * 1,
            SocketKeepaliveEnable          = true,
            ReconnectBackoffMs             = 2000,
            TopicMetadataRefreshIntervalMs = 10000,
            Acks                           = Acks.All,
            EnableAutoCommit               = true,
            MaxPollIntervalMs              = 60000,
            SessionTimeoutMs               = 45000
        };

        if (_consumerSettings.GroupId.Assigned())
        {
            conf.GroupId = _consumerSettings.GroupId;
        }
        else if (_consumerSettings.UniqueByMac)
        {
            var firstMacAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up &&
                              nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();
            conf.GroupId = $"mac_address_{firstMacAddress}";
        }
        else
        {
            var guid = Guid.NewGuid().ToString();
            conf.GroupId = $"random_{guid}";
        }

        lock (_locker)
        {
            _consumer = new ConsumerBuilder<Ignore, string>(conf)
                .SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
                .Build();
            _consumer.Subscribe(_consumerSettings.Topics);
        }

        lock (_locker)
            _consumer.Subscribe(_consumerSettings.Topics);
        
        if (_consumerSettings.ReadLast)
            _consumer.Assign(_consumer.Assignment.Select(tp => new TopicPartitionOffset(tp, Offset.End)));

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true; // prevent the process from terminating.
            _cancellationTokenSource.Cancel();
        };
    }
    
    public void Consume()
    {
       lock (_locker)
       {
           if (Enabled)
               return;
           
           Enabled = true;
           _thread = new Thread(ConsumeMessages);
           _thread.Start();
       }
    }

    private async void ConsumeMessages()
    {

        while (Enabled && !_cancellationTokenSource.IsCancellationRequested)
        {
            var           consumerName   = GetType().Name;
            var           topic          = string.Join(',', _consumerSettings.Topics);
            ITransaction? apmTransaction = null;
            try
            {
                var guid = Guid.NewGuid();
                Logger.LogInformation($"[BaseKafkaConsumer] {ConsumerIndex} Poll calling {guid}");
                    _consumeResult = _consumer.Consume(_cancellationTokenSource.Token);

                apmTransaction = Agent.Tracer.StartTransaction($"{consumerName} {topic}", ApmTransactionType.KafkaMessage);

                Logger.LogInformation($"[BaseKafkaConsumer] Topic {_consumeResult.Topic} Poll calling {guid.ToString()}");
                await OnMessage(_consumeResult);
                Logger.LogInformation(
                    $"[BaseKafkaConsumer] Topic = {_consumeResult.Topic}, Partition {_consumeResult.Partition.ToString()} {ConsumerIndex} Poll processed {guid.ToString()}",
                    _consumeResult.Topic, _consumeResult.Partition.ToString(), ConsumerIndex.ToString(), guid.ToString());
                _consumer.Commit(_consumeResult);
            }
            catch (ConsumeException ex)
            {
                apmTransaction?.CaptureException(ex);
                OnConsumeException(ex);
            }
            catch (OperationCanceledException ex)
            {
                apmTransaction?.CaptureException(ex);
                Logger.LogError($"[BaseKafkaConsumer] OperationCanceledException");
                _consumer.Close();
                OnOperationCanceledException(ex);
            }
            catch (Exception ex)
            {
                apmTransaction?.CaptureException(ex);
                Logger.LogError($"[BaseKafkaConsumer] OnOtherException");
                OnOtherException(ex);
            }
            finally
            {
                apmTransaction?.End();
            }
        } 
    }

    public void StopConsume()
    {
        _cancellationTokenSource.Cancel();
    }

    protected virtual void OnConsumeException(ConsumeException ex)
    {
        Logger.LogWarning($"[BaseKafkaConsumer] OnConsumerException | ConsumeException");
    }
    
    protected virtual void OnOtherException(Exception ex)
    {
        Logger.LogWarning($"[BaseKafkaConsumer] OnOtherException | Exception");
    }

    private void OnOperationCanceledException(OperationCanceledException ex)
    {
        lock (_locker)
        {
            Enabled = false;
        }
    }



    protected abstract Task<bool> OnMessage(ConsumeResult<Ignore, string> e);
}