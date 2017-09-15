namespace FaceRecognition.Common
{
    public partial class Bootstrapper
    {
        public partial class PythonConfiguration
        {
            public class RabbitMqExchanges
            {
                public string Main { get; } = "facerecognition";
            }
        }
    }
}