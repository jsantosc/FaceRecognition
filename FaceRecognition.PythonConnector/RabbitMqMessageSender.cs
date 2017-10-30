using System.IO;
using System.Threading.Tasks;
using Conditions;
using Newtonsoft.Json;

namespace FaceRecognition.PythonConnector
{
    public class RabbitMqMessageSender : IMessageSender
    {
        private readonly RpcClient _rpcClient;
        private readonly JsonSerializer _serializer;

        public RabbitMqMessageSender(RpcClient rpcClient)
        {
            _rpcClient = rpcClient;
            _serializer = new JsonSerializer();
        }
        public async Task<TResponse> SendRequestAsync<TRequest, TResponse, T>(TRequest request)
            where TRequest : PythonRequestMessage
            where TResponse : PythonResponseMessage<T>
        {
            Condition.Requires(request, nameof(request)).IsNotNull();

            var serializedRequest = await SerializeAsync(request).ConfigureAwait(false);
            var serializedResponse = _rpcClient.Call(serializedRequest);

            return Deserialize<TResponse>(serializedResponse);
        }

        private async Task<byte[]> SerializeAsync<T>(T objectToSerialize)
            where T : class
        {
            using (var mStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(mStream))
            {
                _serializer.Serialize(streamWriter, objectToSerialize);
                await streamWriter.FlushAsync().ConfigureAwait(false);
                mStream.Position = 0;
                return mStream.ToArray();
            }
        }
        private T Deserialize<T>(byte[] bytes)
            where T : class
        {
            using (var mStream = new MemoryStream(bytes))
            using (var streamReader = new StreamReader(mStream))
            {
                var jsonTextReader = new JsonTextReader(streamReader) { CloseInput = false };
                return _serializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}