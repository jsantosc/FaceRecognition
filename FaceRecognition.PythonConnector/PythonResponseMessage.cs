namespace FaceRecognition.PythonConnector
{
    public class PythonResponseMessage<T>
    {
        public bool IsSuccess { get; set; }
        public string OutputMessage { get; set; }
        public T Data { get; set; }
    }
}