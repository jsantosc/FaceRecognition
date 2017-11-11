using FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos;

namespace FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation
{
    public interface IRNetClient : IRpcClient<RNetRequestDto, RNetResponseDto>
    {
    }
}