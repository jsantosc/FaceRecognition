using FaceRecognition.Common;
using FaceRecognition.ServiceBus.Bridge.Dtos;

namespace FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos
{
    public class ONetRequestDto : BaseRequestDto
    {
        public ONetRequestDto(float[,,] normalizedImageValues)
        {
            InputLayerValues = new[] { new LayerValue3DimDto(Bootstrapper.NeuralNetworks.Mtcnn.ONet.InputLayerName, normalizedImageValues) };
            OutputLayerNames = Bootstrapper.NeuralNetworks.Mtcnn.ONet.OutputLayerNames;
        }
    }
}