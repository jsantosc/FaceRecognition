using System;
using Conditions;

namespace FaceRecognition.Common.NoLoh.SpanImplementation
{
    public class NoLohArray4D<T> : NoLohArray<T>
        where T : struct
    {
        private int _strideX;
        private int _strideY;
        private int _strideZ;
        private int _strideW;

        public int XLength { get; private set; }
        public int YLength { get; private set; }
        public int ZLength { get; private set; }
        public int WLength { get; private set; }

        public (int StrideX, int StrideY, int StrideZ, int StrideW) Strides => (_strideX, _strideY, _strideZ, _strideW);

        public NoLohArray4D(int xLength, int yLength, int zLength, int wLength, bool gcPressure = false)
            : base(xLength * yLength * zLength * wLength, gcPressure)
        {
            XLength = xLength;
            YLength = yLength;
            ZLength = zLength;
            WLength = WLength;
        }

        public T this[int xIndex, int yIndex, int zIndex, int wIndex]
        {
            get
            {
                Condition.Requires(xIndex, nameof(xIndex)).IsInRange(0, XLength - 1);
                Condition.Requires(yIndex, nameof(yIndex)).IsInRange(0, YLength - 1);
                Condition.Requires(zIndex, nameof(zIndex)).IsInRange(0, ZLength - 1);
                Condition.Requires(zIndex, nameof(wIndex)).IsInRange(0, WLength - 1);

                return this[xIndex + yIndex * XLength + zIndex * XLength * YLength + wIndex * XLength * YLength];
            }
            set
            {
                Condition.Requires(xIndex, nameof(xIndex)).IsInRange(0, XLength - 1);
                Condition.Requires(yIndex, nameof(yIndex)).IsInRange(0, YLength - 1);
                Condition.Requires(zIndex, nameof(zIndex)).IsInRange(0, ZLength - 1);

                this[xIndex + yIndex * XLength + zIndex * XLength * YLength + wIndex * XLength * YLength] = value;
            }
        }
    }
}