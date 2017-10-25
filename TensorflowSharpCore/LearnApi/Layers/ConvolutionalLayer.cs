using System;

namespace TensorflowSharpCore.LearnApi.Layers
{
    public abstract partial class ConvolutionalLayer<T> : Layer
    {
        private TFOutput _kernelWeights;
        private TFOutput _convolutionalOutput;
        private TFOutput _convolutionalOutputBiased;
        private TFOutput _biasWeights;

        public override string Name { get; }
        public override LayerType Type { get; } = LayerType.Convolution2D;
        public override TFOutput Output { get; protected set; }
        public (int W, int H, int C) KernelSize { get; protected set; }
        public int Grouping { get; protected set; }
        public bool AddBias { get; protected set; }
        public T[] Biases { get; protected set; }
        public bool AddReLu { get; protected set; }
        public (int Width, int Height) Stride { get; protected set; }
        public PaddingType Padding { get; protected set; }
        public T[,,,] Weights { get; protected set; }
        public abstract TFDataType DataType { get; }

        /// <summary>
        /// Builds a new convolutional layer with the defined parameters
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="grouping">Group factor between batch channels. Batch dimmesion of the input and output must be divisible by this number.</param>
        public ConvolutionalLayer(string name, (int W, int H, int C) kernelSize, (int Width, int Height) stride, PaddingType padding, T[,,,] weights, int grouping = 1, bool addBias = true, T[] biases = null, bool addReLu = true)
        {
            Name = name;
            KernelSize = kernelSize;
            Grouping = grouping;
            AddBias = addBias;
            Biases = biases;
            AddReLu = addReLu;
            Stride = stride;
            Padding = padding;
            Weights = weights;
        }

        internal void ConstructLayer(TFGraph graph, Layer previousLayer)
        {
            var shape = graph.GetShape(previousLayer.Output);
            var channelInput = shape[shape.Length - 1];

            if (channelInput % Grouping != 0 || KernelSize.C % Grouping != 0)
            {
                throw new Exception();
            }
            if (AddBias)
            {
                if (Biases == null)
                {
                    throw new ArgumentNullException(nameof(Biases));
                }
                if (Biases.Length != KernelSize.C)
                {
                    throw new ArgumentException($"{nameof(Biases)} must have a length of {KernelSize.C} instead of {Biases.Length}", nameof(Biases));
                }
            }
            if (Weights.Rank != 4)
            {
                throw new ArgumentException($"{nameof(Weights)} must have rank 4 instead of {Weights.Rank}", nameof(Weights));
            }
            if (Weights.GetLength(0) != KernelSize.H
                || Weights.GetLength(1) != KernelSize.W
                || (channelInput != TfConstants.NullDimension && Weights.GetLength(2) != channelInput)
                || Weights.GetLength(3) != KernelSize.C)
            {
                throw new ArgumentException(
                    $"Invalid {nameof(Weights)} size, ({Weights.GetLength(0)},{Weights.GetLength(1)},{Weights.GetLength(2)},{Weights.GetLength(3)}) instead of ({KernelSize.H},{KernelSize.W},{channelInput},{KernelSize.C})",
                    nameof(Weights));
            }
            using (var scope = graph.WithScope(Name))
            {
                _kernelWeights = graph.Const(new TFTensor(Weights), DataType);
                Output = _convolutionalOutput = graph.Conv2D(previousLayer.Output, _kernelWeights, new long[] { 1, Stride.Width, Stride.Height, 1 }, Padding.ToString().ToUpper());
                if (AddBias)
                {
                    _biasWeights = graph.Const(new TFTensor(Biases), DataType, "Bias");
                    Output = _convolutionalOutputBiased = graph.BiasAdd(Output, _biasWeights);
                }
                if (AddReLu)
                {
                    Output = graph.Relu(Output, Name);
                }
            }
        }
    }
}