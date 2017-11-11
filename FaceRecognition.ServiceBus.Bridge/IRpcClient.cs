using System.Threading.Tasks;

namespace FaceRecognition.ServiceBus.Bridge
{
    public interface IRpcClient<in TSend, TResponse>
    {
        int Timeout { get; }
        string Exchange { get; }
        string RoutingKey { get; }
        TResponse Send(TSend message);
        Task<TResponse> SendAsync(TSend message);
    }
}