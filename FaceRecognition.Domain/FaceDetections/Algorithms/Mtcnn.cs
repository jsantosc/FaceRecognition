using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conditions;
using FaceRecognition.Domain.Helpers;
using FaceRecognition.Domain.Images;

namespace FaceRecognition.Domain.FaceDetections.Algorithms
{
    public class Mtcnn
    {
        public const int PNetSize = 12;

        /// <summary>
        /// Gets the minimum size of the face.
        /// </summary>
        public int MinimunSize { get; protected set; } = 20;
        //public CaffeDeepNeuralNetwork PNet { get; protected set; }
        //public CaffeDeepNeuralNetwork RNet { get; protected set; }
        //public CaffeDeepNeuralNetwork ONet { get; protected set; }
        public float Threshold1 { get; protected set; } = 0.6f;
        public float Threshold2 { get; protected set; } = 0.7f;
        public float Threshold3 { get; protected set; } = 0.7f;
        /// <summary>
        /// Gets if the image must be resized from the previous piramid one (<b>true</b>) or from the original one.
        /// </summary>
        /// <remarks><b>true</b> value is recommmended for large image.</remarks>
        public bool FastResize { get; protected set; }

        public float Factor { get; protected set; } = 0.709f;

        public async Task LocateFacesAsync(BaseImage image)
        {
            Condition.Requires(image, nameof(image)).IsNotNull();

            var scales = new List<double>();
            var minl = (double)Math.Min(image.Width, image.Height);
            var m = 1.0 * PNetSize / MinimunSize;
            var factorCount = 0;

            minl = minl * m;
            while (minl >= 12)
            {
                scales.Add(m * Math.Pow(Factor, factorCount));
                minl = minl * Factor;
                factorCount++;
            }
            foreach (var scale in scales)
            {
                int newWidth = (int)Math.Ceiling(image.Width * scale);
                int newHeight = (int)Math.Ceiling(image.Height * scale);
                float[,,,] normalizedValues;

                using (var rgba = image.ToColorPixelsArray(newWidth, newHeight))
                {
                    normalizedValues = rgba.NormalizeBatch();
                }

            }
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}