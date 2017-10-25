namespace TensorflowSharpCore.LearnApi.Layers
{
    public class MultidimensionalSoftmax : Layer
    {
        public int Axis { get; protected set; }
        public override string Name { get; }
        public override LayerType Type { get; } = LayerType.MultidimensionalSoftmax;
        public override TFOutput Output { get; protected set; }

        public MultidimensionalSoftmax(string name, int axis)
        {
            Name = name;
            Axis = axis;
        }

        internal void ConstructLayer(TFGraph graph, Layer previousLayer)
        {
            var axis = graph.Const(new TFTensor(Axis));
            var maxAxis = graph.Max(previousLayer.Output, axis, true);
            var targetExp = graph.Exp(graph.Sub(previousLayer.Output, maxAxis));
            var normalize = graph.ReduceSum(targetExp, axis, true);
            Output = graph.Div(targetExp, normalize, Name);
        }
    }
}