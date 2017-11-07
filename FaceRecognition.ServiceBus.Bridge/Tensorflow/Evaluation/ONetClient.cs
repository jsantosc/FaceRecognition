using FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos;

namespace FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation
{
    public class ONetClient : RpcClient<ONetRequestDto, ONetResponseDto>
    {

        private static string _replyQueueName;

        public ONetClient(string hostname, int port = 5672, int timeout = 30000)
            : base(hostname, port, timeout)
        {
            if (string.IsNullOrWhiteSpace(_replyQueueName))
            {
                _replyQueueName = TemporalQueueGenerator.Instance.GetTemporalQueue(hostname, port);
            }
            ReplyQueueName = _replyQueueName;
            Exchange = RabbitMqExchanges.Tensorflow;
            RoutingKey = RabbitMqQueues.Tensorflow.Evaluation.ONet;
        }
    }
}