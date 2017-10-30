using System.Threading.Tasks;

namespace FaceRecognition.PythonConnector
{
    public interface IMessageSender
    {
        Task<TResponse> SendRequestAsync<TRequest, TResponse, T>(TRequest request)
            where TRequest : PythonRequestMessage
            where TResponse : PythonResponseMessage<T>;
    }
}