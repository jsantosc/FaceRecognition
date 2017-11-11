using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FaceRecognition.Domain.Helpers;
using FaceRecognition.Domain.Images;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace FaceRecognition.UnitTests.Domain.Helpers
{
    public class ImageHelpersShould
    {
        [Fact]
        public async Task GenerateNormalizedBatchArray_WhenAnRgbImageIsSupplied()
        {
            await Task.CompletedTask.ConfigureAwait(false);
            const int width = 615;
            const int height = 384;
            float[,,] normalizedValues;
            float[,,] expectedValues;

            var image = await BaseImage.CreateAsync("Domain/Helpers/Resources/image_020_1.jpg", copyToFileServer: true).ConfigureAwait(false);

            using (var rgbValues = image.ToColorPixelsArray(width, height))
            {
                normalizedValues = rgbValues.NormalizeBatch();
            }
            using (var sReader = File.OpenText("Domain/Helpers/Resources/NormalizedImageValues.json"))
            {
                var json = await sReader.ReadToEndAsync().ConfigureAwait(false);
                expectedValues = JsonConvert.DeserializeObject<float[,,]>(json);
            }

            normalizedValues.Should().NotBeNull();
            normalizedValues.GetLength(0).Should().Be(width);
            normalizedValues.GetLength(1).Should().Be(height);
            normalizedValues.GetLength(2).Should().Be(3);
            for (int i = 0; i < normalizedValues.GetLength(0); i++)
            {
                for (int j = 0; j < normalizedValues.GetLength(1); j++)
                {
                    for (int k = 0; k < normalizedValues.GetLength(2); k++)
                    {
                        normalizedValues[i, j, k].Should().Be(expectedValues[i, j, k]);
                    }
                }
            }
        }
    }
}