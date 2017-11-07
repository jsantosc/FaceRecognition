namespace FaceRecognition.Common
{
    public partial class Bootstrapper
    {
        public static class NeuralNetworks
        {
            public static class Mtcnn
            {
                public static class ONet
                {
                    public const string InputLayerName = "data";
                    public static string[] OutputLayerNames { get; } = { "prob1", "conv4-2" };
                    public static string[] AllLayerNames { get; } = { "data", "pool1", "conv2", "conv3", "prob1", "conv4-1", "conv4-2" };
                }
            }
        }
    }
}