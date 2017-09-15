using System;
using Conditions;

namespace FaceRecognition.Common.NoLoh.SpanImplementation
{
    public class NoLohArray3D<T> : NoLohArray<T>
        where T : struct
    {
        private int _strideX;
        private int _strideY;
        private int _strideZ;

        public int XLength { get; private set; }
        public int YLength { get; private set; }
        public int ZLength { get; private set; }
        public (int StrideX, int StrideY, int StrideZ) Strides => (_strideX, _strideY, _strideZ);

        public NoLohArray3D(int xLength, int yLength, int zLength, bool gcPressure = false)
            : base(xLength * yLength * zLength, gcPressure)
        {
            XLength = xLength;
            YLength = yLength;
            ZLength = zLength;
        }

        public T this[int xIndex, int yIndex, int zIndex]
        {
            get
            {
                Condition.Requires(xIndex, nameof(xIndex)).IsInRange(0, XLength - 1);
                Condition.Requires(yIndex, nameof(yIndex)).IsInRange(0, YLength - 1);
                Condition.Requires(zIndex, nameof(zIndex)).IsInRange(0, ZLength - 1);

                return this[xIndex + yIndex * XLength + zIndex * XLength * YLength];
            }
            set
            {
                Condition.Requires(xIndex, nameof(xIndex)).IsInRange(0, XLength - 1);
                Condition.Requires(yIndex, nameof(yIndex)).IsInRange(0, YLength - 1);
                Condition.Requires(zIndex, nameof(zIndex)).IsInRange(0, ZLength - 1);

                this[xIndex + yIndex * XLength + zIndex * XLength * YLength] = value;
            }
        }

        public void Transpose(byte newX, byte newY, byte newZ)
        {
            Condition.Requires(newX, nameof(newX)).IsInRange(0, 2);
            Condition.Requires(newY, nameof(newY)).IsInRange(0, 2);
            Condition.Requires(newZ, nameof(newZ)).IsInRange(0, 2);

            int tempStrideX = _strideX;
            int tempStrideY = _strideY;
            int tempStrideZ = _strideZ;
            int tempLengthX = XLength;
            int tempLengthY = YLength;
            int tempLengthZ = ZLength;

            if (newX == newY || newX == newZ || newY == newZ)
            {
                throw new ArgumentException();
            }

            switch (newX)
            {
                case 1:
                    _strideX = tempStrideY;
                    XLength = tempLengthY;
                    break;
                case 2:
                    _strideX = tempStrideZ;
                    XLength = tempLengthZ;
                    break;
            }
            switch (newY)
            {
                case 0:
                    _strideY = tempStrideX;
                    YLength = tempLengthX;
                    break;
                case 2:
                    _strideY = tempStrideZ;
                    YLength = tempLengthZ;
                    break;
            }
            switch (newZ)
            {
                case 0:
                    _strideZ = tempStrideX;
                    ZLength = tempLengthX;
                    break;
                case 1:
                    _strideZ = tempStrideY;
                    ZLength = tempLengthY;
                    break;
            }
        }
    }
}