using System;
using System.Threading;

using Confluent.Kafka;

using Microsoft.Extensions.Logging;

namespace StreamDataProcessor
{
    public class StreamReader : IDisposable
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly ILogger<StreamReader> _logger;

        public StreamReader(ConsumerConfig config, ILogger<StreamReader> logger)
        {
            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _logger = logger;
            _consumer.Subscribe("test-topic");
        }

        public void Dispose()
        {
            _consumer?.Dispose();
        }

        public ConsumeResult<string, string> Read(CancellationToken stoppingToken)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        var cr = _consumer.Consume(stoppingToken);

                        _logger.LogInformation("Consumed message {Msg} at: '{Tpo}'", cr.Message.Value, cr.TopicPartitionOffset);

                        return cr;
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError("Error occured: {@Error}", e);

                        throw;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }
    }
}
