using System;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace FaceRecognition.PythonConnector
{
    public class RpcClient : SimpleRpcClient
    {
        public RpcClient(IModel model, string exchange, string exchangeType, string routingKey)
            : base(model, exchange, exchangeType, routingKey)
        {
            TimeoutMilliseconds = 30000;
        }

        public sealed override void OnTimedOut()
        {
            throw new TimeoutException();
        }

        public sealed override void OnDisconnected()
        {
            throw new Exception("Disconnected while waiting for a response");
        }
    }
}