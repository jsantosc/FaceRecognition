namespace FaceRecognition.ServiceBus.Bridge
{
    public static class RabbitMqQueues //TODO: Move to Bootstraper
    {
        public static class Tensorflow
        {
            public static class Evaluation
            {
                public const string ONet = "evaluate.onet";
                public const string PNet = "evaluate.pnet";
                public const string RNet = "evaluate.rnet";
            }
        }
    }
}