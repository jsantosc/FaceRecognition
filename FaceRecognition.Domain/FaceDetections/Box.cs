using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FaceRecognition.Common.NoLoh.SpanImplementation;

namespace FaceRecognition.Domain.FaceDetections
{
    public class Box
    {
        public int MinX { get; protected set; }
        public int MinY { get; protected set; }
        public int MaxX { get; protected set; }
        public int MaxY { get; protected set; }
        public float Score { get; protected set; }

        public static IEnumerable<Box> CreateFromMtcnnHeatMap(NoLohArray3D<float> probabilities, NoLohArray4D<float> heatmap, float scale, float threshold)
        {
            const float stride = 2f;
            const float cellSize = 12f;
            const int zIndex = 1;
            var positions = new ConcurrentBag<(int X, int Y, float Score)>();

            Parallel.For(0, probabilities.XLength, x =>
            {
                for (int y = 0; y < probabilities.YLength; y++)
                {
                    if (probabilities[x, y, zIndex] > threshold)
                    {
                        positions.Add((x, y, probabilities[x, y, zIndex]));
                    }
                }
            });

            if (positions.Count == 0)
            {
                return Enumerable.Empty<Box>();
            }

            var boundingBoxes = new List<Box>();
            foreach (var position in positions)
            {
                boundingBoxes.Add(new Box
                {
                    MinX = (int)Math.Round((stride * position.X) / scale),
                    MinY = (int)Math.Round((stride * position.Y) / scale),
                    MaxX = (int)Math.Round((stride * position.X + cellSize) / scale),
                    MaxY = (int)Math.Round((stride * position.Y + cellSize) / scale),
                    Score = position.Score
                });
            }
            return boundingBoxes;
        }
    }
}