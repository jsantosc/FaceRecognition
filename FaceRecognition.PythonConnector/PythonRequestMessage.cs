namespace FaceRecognition.PythonConnector
{
    public class PythonRequestMessage
    {
        public PythonRequestMessage(string moduleFunctionType, object data)
        {
            ModuleFunctionType = moduleFunctionType;
            Data = data;
        }

        public string ModuleFunctionType { get; set; }
        public object Data { get; set; }
    }
}