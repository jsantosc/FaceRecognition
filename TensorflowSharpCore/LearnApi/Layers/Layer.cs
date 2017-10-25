namespace TensorflowSharpCore.LearnApi.Layers
{
    public abstract class Layer
    {
        public abstract string Name { get; }
        public abstract LayerType Type { get; }
        public abstract TFOutput Output { get; protected set; }
    }
}