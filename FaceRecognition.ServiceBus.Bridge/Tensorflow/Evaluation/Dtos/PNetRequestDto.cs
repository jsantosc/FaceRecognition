using FaceRecognition.Common;
using FaceRecognition.ServiceBus.Bridge.Dtos;

namespace FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos
{
    public class PNetRequestDto : BaseRequestDto
    {
        public PNetRequestDto(float[,,] normalizedImageValues, LoadWeightsMode loadWeightsMode, string weightsFilePath)
        {
            InputLayerValues = new[] { new LayerValue3DimDto(Bootstrapper.NeuralNetworks.Mtcnn.ONet.InputLayerName, normalizedImageValues) };
            OutputLayerNames = Bootstrapper.NeuralNetworks.Mtcnn.ONet.OutputLayerNames;
            LoadWeightsMode = loadWeightsMode;
            WeightsFilePath = weightsFilePath;
        }
    }
}