namespace TensorflowSharpCore.LearnApi.Layers
{
    public class ConvolutionalLayerD : ConvolutionalLayer<double>
    {
        public ConvolutionalLayerD(string name, (int W, int H, int C) kernelSize, (int Width, int Height) stride, PaddingType padding, double[,,,] weights, int grouping = 1, bool addBias = true, double[] biases = null, bool addReLu = true)
            : base(name, kernelSize, stride, padding, weights, grouping, addBias, biases, addReLu)
        {
        }

        public override TFDataType DataType { get; } = TFDataType.Double;
    }
}