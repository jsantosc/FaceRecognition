namespace FaceRecognition.Common
{
    public partial class Bootstrapper
    {
        public partial class PythonConfiguration
        {
            public class RabbitMqQueues
            {
                public string NeuralNetworks { get; } = "neuralnetworks";
            }
        }
    }
}