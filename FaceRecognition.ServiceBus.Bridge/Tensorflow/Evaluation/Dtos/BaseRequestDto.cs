using System.Collections.Generic;
using FaceRecognition.ServiceBus.Bridge.Dtos;

namespace FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos
{
    public class BaseRequestDto
    {
        public IEnumerable<LayerValue3DimDto> InputLayerValues { get; set; }
        public IEnumerable<string> OutputLayerNames { get; set; }
    }
}