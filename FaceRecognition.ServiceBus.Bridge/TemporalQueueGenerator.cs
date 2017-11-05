using System;
using System.Collections.Concurrent;
using RabbitMQ.Client;

namespace FaceRecognition.ServiceBus.Bridge
{
    internal sealed class TemporalQueueGenerator : IDisposable
    {
        private ConcurrentDictionary<string, (IConnection Connection, IModel Channel, ConcurrentBag<string> TemporalQueues)> _temporalQueues =
            new ConcurrentDictionary<string, (IConnection Connection, IModel Channel, ConcurrentBag<string> TemporalQueues)>();
        private static Lazy<TemporalQueueGenerator> _instance = new Lazy<TemporalQueueGenerator>(() => new TemporalQueueGenerator());

        public static TemporalQueueGenerator Instance => _instance.Value;

        private TemporalQueueGenerator()
        {
        }

        internal string GetTemporalQueue(string hostname, int port)
        {
            var key = GenerateKey(hostname, port);

            var queues = _temporalQueues.GetOrAdd(key, newKey =>
            {
                var factory = new ConnectionFactory() { HostName = hostname, Port = port };
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();

                return (connection, channel, new ConcurrentBag<string>());
            });
            var queueName = queues.Channel.QueueDeclare(exclusive: false, autoDelete: false).QueueName;
            queues.TemporalQueues.Add(queueName);
            return queueName;
        }

        private string GenerateKey(string hostname, int port) => $"{hostname}:{port}";

        public void Dispose()
        {
            foreach (var item in _temporalQueues)
            {
                try
                {
                    foreach (var queue in item.Value.TemporalQueues)
                    {
                        item.Value.Channel.QueueDelete(queue);
                    }
                    item.Value.Channel?.Dispose();
                    item.Value.Connection?.Close();
                    item.Value.Connection?.Dispose();
                }
                catch
                {
                    // Skip errors during dispose workflow
                }
            }
        }
    }
}