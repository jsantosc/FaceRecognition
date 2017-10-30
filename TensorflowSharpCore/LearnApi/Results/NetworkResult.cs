using System;

namespace TensorflowSharpCore.LearnApi.Results
{
    public abstract class NetworkResult<T>
    {
        private T[] _oneDimensionArray;
        private T[,] _twoDimensionArray;
        private T[,,] _threeDimensionArray;
        private T[,,,] _fourDimensionArray;

        public int Dimensions { get; }
        public string NetworkLayerName { get; }
        public int XLength { get; private set; }
        public int YLength { get; private set; }
        public int ZLength { get; private set; }
        public int WLength { get; private set; }

        public virtual T this[int x]
        {
            get
            {
                if (Dimensions != 1)
                {
                    throw new NotSupportedException($"Result must have dimension 1. Actual dimension is {Dimensions}");
                }
                ValidateRanges(x, 0, 0, 0);
                return _oneDimensionArray[x];
            }
        }
        public virtual T this[int x, int y]
        {
            get
            {
                if (Dimensions != 2)
                {
                    throw new NotSupportedException($"Result must have dimension 2. Actual dimension is {Dimensions}");
                }
                ValidateRanges(x, y, 0, 0);
                return _twoDimensionArray[x, y];
            }
        }
        public virtual T this[int x, int y, int z]
        {
            get
            {
                if (Dimensions != 3)
                {
                    throw new NotSupportedException($"Result must have dimension 3. Actual dimension is {Dimensions}");
                }
                ValidateRanges(x, y, z, 0);
                return _threeDimensionArray[x, y, z];
            }
        }
        public virtual T this[int x, int y, int z, int w]
        {
            get
            {
                if (Dimensions != 4)
                {
                    throw new NotSupportedException($"Result must have dimension 3. Actual dimension is {Dimensions}");
                }
                ValidateRanges(x, y, z, w);
                return _fourDimensionArray[x, y, z, w];
            }
        }



        public NetworkResult(int dimensions, object value, string networkLayerName)
        {
            Dimensions = dimensions;
            NetworkLayerName = networkLayerName;
            TryLoadArray(value);
        }

        private void TryLoadArray(object value)
        {
            if (Dimensions == 1)
            {
                _oneDimensionArray = (T[])value;
                XLength = _oneDimensionArray.GetLength(0);
                YLength = 0;
                ZLength = 0;
                WLength = 0;
            }
            else if (Dimensions == 2)
            {
                _twoDimensionArray = (T[,])value;
                XLength = _twoDimensionArray.GetLength(0);
                YLength = _twoDimensionArray.GetLength(1);
                ZLength = 0;
                WLength = 0;
            }
            else if (Dimensions == 3)
            {
                _threeDimensionArray = (T[,,])value;
                XLength = _threeDimensionArray.GetLength(0);
                YLength = _threeDimensionArray.GetLength(1);
                ZLength = _threeDimensionArray.GetLength(2);
                WLength = 0;
            }
            else if (Dimensions == 4)
            {
                _fourDimensionArray = (T[,,,])value;
                XLength = _fourDimensionArray.GetLength(0);
                YLength = _fourDimensionArray.GetLength(1);
                ZLength = _fourDimensionArray.GetLength(2);
                WLength = _fourDimensionArray.GetLength(3);
            }
            else
            {
                throw new NotSupportedException($"Invalid dimension {Dimensions}. Right now only dimensions 1, 2 and 3 are supported");
            }
        }
        private void ValidateRanges(int x, int y, int z, int w)
        {
            if (x >= XLength || x < 0
                || YLength > 0 && (y < 0 || y >= YLength)
                || ZLength > 0 && (z < 0 || z >= ZLength)
                || WLength > 0 && (w < 0 || w >= WLength))
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}