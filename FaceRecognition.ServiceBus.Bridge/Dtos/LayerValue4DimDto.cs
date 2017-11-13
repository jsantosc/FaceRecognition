using FaceRecognition.Common.JSonConverter;
using FaceRecognition.Common.NoLoh.SpanImplementation;
using Newtonsoft.Json;

namespace FaceRecognition.ServiceBus.Bridge.Dtos
{
    public class LayerValue4DimDto
    {
        public string Name { get; set; }
        [JsonConverter(typeof(NoLohArray4DConverter))]
        public NoLohArray4D<float> Value { get; set; }
    }
}