using Conditions;

namespace FaceRecognition.Common.NoLoh.SpanImplementation
{
    public class NoLohArray2D<T> : NoLohArray<T>
        where T : struct
    {
        private int _strideX;
        private int _strideY;

        public int XLength { get; private set; }
        public int YLength { get; private set; }
        public (int StrideX, int StrideY) Strides => (_strideX, _strideY);

        public NoLohArray2D(int xLength, int yLength, bool gcPressure = false)
            : base(xLength * yLength, gcPressure)
        {
            XLength = xLength;
            YLength = yLength;
            _strideX = 1;
            _strideY = xLength;
        }

        public T this[int xIndex, int yIndex]
        {
            get
            {
                Condition.Requires(xIndex, nameof(xIndex)).IsInRange(0, XLength - 1);
                Condition.Requires(yIndex, nameof(yIndex)).IsInRange(0, YLength - 1);

                return this[xIndex * _strideX + yIndex * _strideY];
            }
            set
            {
                Condition.Requires(xIndex, nameof(xIndex)).IsInRange(0, XLength - 1);
                Condition.Requires(yIndex, nameof(yIndex)).IsInRange(0, YLength - 1);

                this[xIndex * _strideX + yIndex * XLength] = value;
            }
        }

        public void Transpose()
        {
            var temp = _strideX;

            _strideX = _strideY;
            _strideY = temp;

            temp = YLength;
            YLength = XLength;
            XLength = temp;
        }
    }
}