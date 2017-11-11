using System.Threading.Tasks;
using FaceRecognition.Common;
using FaceRecognition.Common.NoLoh.SpanImplementation;

namespace FaceRecognition.Domain.Helpers
{
    public static class ImageHelpers
    {
        public static float[,,] NormalizeBatch(this NoLohArray3D<float> image)
        {
            float[,,] normalizedBatch = new float[image.XLength, image.YLength, image.ZLength];

            Parallel.For(0, image.XLength, Bootstrapper.Instance.MaxDegreeOfParalelism, row =>
            {
                for (int column = 0; column < image.YLength; column++)
                {
                    for (int channel = 0; channel < image.ZLength; channel++)
                    {
                        normalizedBatch[row, column, channel] = (image[row, column, channel] - 127.5f) * 0.0078125f;
                    }
                }
            });
            return normalizedBatch;
        }
    }
}