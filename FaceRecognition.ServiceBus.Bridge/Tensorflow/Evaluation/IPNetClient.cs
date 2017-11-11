using FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos;

namespace FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation
{
    public interface IPNetClient : IRpcClient<PNetRequestDto, PNetResponseDto>
    {
    }
}