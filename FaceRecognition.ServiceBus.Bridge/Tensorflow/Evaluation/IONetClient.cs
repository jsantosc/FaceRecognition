using FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos;

namespace FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation
{
    public interface IONetClient : IRpcClient<ONetRequestDto, ONetResponseDto>
    {
    }
}