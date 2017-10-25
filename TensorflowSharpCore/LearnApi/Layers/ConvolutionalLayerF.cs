namespace TensorflowSharpCore.LearnApi.Layers
{
    public class ConvolutionalLayerF : ConvolutionalLayer<float>
    {
        public ConvolutionalLayerF(string name, (int W, int H, int C) kernelSize, (int Width, int Height) stride, PaddingType padding, float[,,,] weights, int grouping = 1, bool addBias = true, float[] biases = null, bool addReLu = true)
            : base(name, kernelSize, stride, padding, weights, grouping, addBias, biases, addReLu)
        {
        }

        public override TFDataType DataType { get; } = TFDataType.Float;
    }
}