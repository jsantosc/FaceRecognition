namespace TensorflowSharpCore.LearnApi.Layers
{
    public class InputLayer : Layer
    {
        public int Dimensions { get; protected set; }
        public override string Name { get; }
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public int C { get; protected set; }
        public int Batch { get; protected set; }
        public override TFOutput Output { get; protected set; }

        public override LayerType Type { get; } = LayerType.Input;

        //public InputLayer(string name, int length)
        //{
        //    Name = name;
        //    X = length;
        //}

        //public InputLayer(string name, int height, int width)
        //{
        //    Name = name;
        //    X = height;
        //    Y = width;
        //}

        public InputLayer(string name, int height, int width, int channels, int batch)
        {
            Name = name;
            X = height;
            Y = width;
            C = channels;
            Batch = batch;
        }

        internal void ConstructLayer(TFGraph graph)
        {
            Output = graph.Placeholder(TFDataType.Float, new TFShape(Batch, Y, X, C));
        }
    }
}