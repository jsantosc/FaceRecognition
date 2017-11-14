using System;
using FaceRecognition.Common;
using FaceRecognition.Common.NoLoh.SpanImplementation;
using FaceRecognition.Domain.FaceDetections;
using FluentAssertions;
using Xunit;

namespace FaceRecognition.UnitTests.Domain.FaceDetections
{
    public class BoxShould
    {
        [Fact]
        public void ReturnBoundingBoxes_WhenAHeatmapProbabilityIsSupplied()
        {
            const float threshold = 0.7f;
            const float scale = 0.5f;
            using (var probabilities = new NoLohArray3D<float>(50, 50, 2))
            using (var heatmap = new NoLohArray4D<float>(1, 50, 50, 4))
            {
                probabilities[2, 4, 1] = 0.8f;
                probabilities[4, 8, 1] = 0.9f;

                var boxes = Box.CreateFromMtcnnHeatMap(probabilities, heatmap, scale, threshold);

                boxes.Should().NotBeEmpty();
                boxes.Should().HaveCount(2);
                boxes.Should().Contain(item =>
                    item.MinX == 8 && item.MaxX == 32 && item.MinY == 16 && item.MaxY == 40 && Math.Abs(item.Score - 0.8f) < Bootstrapper.FloatTolerance);
                boxes.Should().Contain(item =>
                    item.MinX == 16 && item.MaxX == 40 && item.MinY == 32 && item.MaxY == 56 && Math.Abs(item.Score - 0.9f) < Bootstrapper.FloatTolerance);
            }
        }
    }
}