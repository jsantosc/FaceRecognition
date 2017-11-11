using FaceRecognition.Common;
using FaceRecognition.ServiceBus.Bridge.Dtos;

namespace FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos
{
    public class RNetRequestDto : BaseRequestDto
    {
        public RNetRequestDto(float[,,] normalizedImageValues)
        {
            InputLayerValues = new[] { new LayerValue3DimDto(Bootstrapper.NeuralNetworks.Mtcnn.ONet.InputLayerName, normalizedImageValues) };
            OutputLayerNames = Bootstrapper.NeuralNetworks.Mtcnn.ONet.OutputLayerNames;
        }
    }
}