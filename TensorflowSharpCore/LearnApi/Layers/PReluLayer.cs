using System;

namespace TensorflowSharpCore.LearnApi.Layers
{
    public class PReluLayer : Layer
    {
        private TFOutput _alphaWeights;

        public override string Name { get; }
        public float[] Alphas { get; protected set; }
        public override LayerType Type { get; } = LayerType.PReLu;
        public override TFOutput Output { get; protected set; }

        public PReluLayer(string name, float[] alphas)
        {
            Name = name;
            Alphas = alphas;
        }

        public void ConstructLayer(TFGraph graph, Layer previousLayer)
        {
            var shape = graph.GetShape(previousLayer.Output);
            var inputSize = shape[shape.Length - 1];

            if (Alphas.Length != inputSize)
            {
                throw new ArgumentException($"The {nameof(Alphas)} must have a size of {inputSize} instead of {Alphas.Length}", nameof(Alphas));
            }
            using (var scope = graph.WithScope(Name))
            {
                _alphaWeights = graph.Const(new TFTensor(Alphas), TFDataType.Float);
                Output = graph.Add(graph.Relu(previousLayer.Output), graph.Mul(_alphaWeights, graph.Neg(graph.Relu(graph.Neg(previousLayer.Output)))));
            }
        }
    }
}