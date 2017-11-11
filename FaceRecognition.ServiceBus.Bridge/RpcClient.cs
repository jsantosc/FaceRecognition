using System;
using System.Text;
using System.Threading.Tasks;
using FaceRecognition.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace FaceRecognition.ServiceBus.Bridge
{
    public abstract class RpcClient<TSend, TResponse> : IDisposable, IRpcClient<TSend, TResponse>
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public int Timeout { get; }
        public string Exchange { get; protected set; }
        public string RoutingKey { get; protected set; }
        public string ReplyQueueName { get; protected set; }

        // TODO: Refactor to pass IConfiguration<T>
        protected RpcClient(string hostname, int port = 5672, int timeout = 30000)
        {
            Timeout = timeout;
            var factory = new ConnectionFactory() { HostName = hostname, Port = port };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public TResponse Send(TSend message)
        {
            var correlationId = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();

            props.CorrelationId = correlationId;
            props.ReplyTo = ReplyQueueName;

            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, Bootstrapper.Instance.DefaultJsonSettings));
            _channel.BasicPublish(
                exchange: Exchange,
                routingKey: RoutingKey,
                basicProperties: props,
                body: messageBytes);

            var subscription = new Subscription(_channel, ReplyQueueName, true);
            while (subscription.Next(Timeout, out var result))
            {
                var body = result.Body;

                if (result.BasicProperties.CorrelationId == correlationId)
                {
                    //_channel.BasicAck(result.DeliveryTag, false);
                    return JsonConvert.DeserializeObject<TResponse>(Encoding.UTF8.GetString(body));
                }
            }
            throw new TimeoutException();
        }

        public async Task<TResponse> SendAsync(TSend message)
        {
            return await Task.Run(() => Send(message)).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
