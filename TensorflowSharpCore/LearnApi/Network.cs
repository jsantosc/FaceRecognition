using System;
using System.Collections.Generic;
using System.Linq;
using TensorflowSharpCore.LearnApi.Layers;

namespace TensorflowSharpCore.LearnApi
{
    public class Network : IDisposable
    {

        private readonly List<IDisposable> _disposeObjects = new List<IDisposable>();
        private readonly IDictionary<string, Layer> _layers = new Dictionary<string, Layer>(StringComparer.OrdinalIgnoreCase);
        private readonly TFGraph _graph;

        public bool HasInputLayer => _layers.Any(l => l.Value.Type == LayerType.Input);
        internal TFGraph Graph => _graph;
        public PrecisionType Precision { get; }

        public Network(PrecisionType precision = PrecisionType.Float)
        {
            Precision = precision;
            _graph = new TFGraph();
            _disposeObjects.Add(_graph);
        }

        public NetworkChainBuilder AddInput(string name, int height, int width, int channels, int batch)
        {
            var inputLayer = new InputLayer(name, height, width, channels, batch);

            inputLayer.ConstructLayer(_graph);
            AddLayer(inputLayer);
            return new NetworkChainBuilder(this, inputLayer);
        }

        public NetworkChainBuilder FromLayer(string name)
        {
            var layer = _layers[name];

            return new NetworkChainBuilder(this, layer);
        }

        internal void AddLayer(Layer layer)
        {
            _layers.Add(layer.Name, layer);
        }
        public void Dispose()
        {
            foreach (var disposeObject in _disposeObjects)
            {
                disposeObject?.Dispose();
            }
        }

        public void Run(dynamic inputParameters, Action<TFSession> customRunning)
        {
            using (var session = new TFSession(Graph))
            {
                var runner = session.GetRunner();

                void SetPlaceholderDouble(Layer inputLayer, object value)
                {
                    runner.AddInput(inputLayer.Output, new TFTensor((double[,,,])value));
                }
                void SetPlaceholderFloat(Layer inputLayer, object value)
                {
                    runner.AddInput(inputLayer.Output, new TFTensor((float[,,,])value));
                }

                Action<Layer, object> setPlaceholder;
                switch (Precision)
                {
                    case PrecisionType.Float:
                        setPlaceholder = SetPlaceholderFloat;
                        break;
                    case PrecisionType.Double:
                        setPlaceholder = SetPlaceholderDouble;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (inputParameters is IDictionary<string, object> parameters)
                {
                    foreach (var item in parameters)
                    {
                        var input = _layers[item.Key];

                        setPlaceholder(input, item.Value);
                    }
                }
                runner.Run();
            }
        }
    }
}