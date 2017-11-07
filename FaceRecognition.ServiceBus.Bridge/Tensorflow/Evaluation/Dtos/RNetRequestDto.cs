using FaceRecognition.Common;
using FaceRecognition.ServiceBus.Bridge.Dtos;

namespace FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos
{
    public class RNetRequestDto : BaseRequestDto
    {
        public RNetRequestDto(float[,,,] imageNormalizedValues)
        {
        }
    }
}