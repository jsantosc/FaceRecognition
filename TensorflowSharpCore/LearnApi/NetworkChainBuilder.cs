using TensorflowSharpCore.LearnApi.Layers;

namespace TensorflowSharpCore.LearnApi
{
    public class NetworkChainBuilder
    {
        private readonly Network _network;
        private Layer _lastLayer;

        public NetworkChainBuilder(Network network, Layer lastLayer)
        {
            _network = network;
            _lastLayer = lastLayer;
        }

        public NetworkChainBuilder ContinueWithConv2D(string name, (int W, int H, int C) kernelSize, (int Width, int Height) stride,
            PaddingType padding, float[,,,] weights, int grouping = 1, bool addBias = true, float[] biases = null, bool addReLu = true)
        {
            var layer = new ConvolutionalLayerF(name, kernelSize, stride, padding, weights, grouping, addBias, biases, addReLu);

            layer.ConstructLayer(_network.Graph, _lastLayer);
            _lastLayer = layer;
            _network.AddLayer(_lastLayer);
            return this;
        }

        public NetworkChainBuilder ContinueWithPRelu(string name, float[] alphas)
        {
            var layer = new PReluLayer(name, alphas);

            layer.ConstructLayer(_network.Graph, _lastLayer);
            _lastLayer = layer;
            _network.AddLayer(_lastLayer);
            return this;
        }
        public NetworkChainBuilder ContinueWithMultidimensionalSoftMax(string name, int axis)
        {
            var layer = new MultidimensionalSoftmax(name, axis);

            layer.ConstructLayer(_network.Graph, _lastLayer);
            _lastLayer = layer;
            _network.AddLayer(_lastLayer);
            return this;
        }

    }
}