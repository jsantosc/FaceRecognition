namespace FaceRecognition.ServiceBus.Bridge.Dtos
{
    public class LayerValue4DimDto
    {
        public string Name { get; set; }
        public float[,,,] Value { get; set; }
    }
}