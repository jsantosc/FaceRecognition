namespace FaceRecognition.ServiceBus.Bridge.Dtos
{
    public class LayerValue3DimDto
    {
        public string Name { get; set; }
        public float[,,] Value { get; set; }

        public LayerValue3DimDto(string name, float[,,] value)
        {
            Name = name;
            Value = value;
        }
    }
}