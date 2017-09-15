using System.Buffers;

namespace FaceRecognition.Common.MemoryOptimizations
{
    public class ArrayPoolManager
    {
        public const int MaxDoubleArray = 256 * 1024 * 1024;
        public const int MaxByteArray = 256 * 1024 * 1024;

        public static ArrayPool<double> DoubleArrayPool = ArrayPool<double>.Create(MaxDoubleArray, 50);
        public static ArrayPool<byte> ByteArrayPool = ArrayPool<byte>.Create(MaxByteArray, 50);
    }
}