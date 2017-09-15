using System.Threading.Tasks;
using FaceRecognition.CommonTests;
using FaceRecognition.Domain.FaceDetections.Algorithms;
using FaceRecognition.Domain.Images;
using FluentAssertions;
using Xunit;

namespace FaceRecognition.UnitTests.Images
{
    public class BaseImageTest : BaseTest
    {
        [Fact]
        public async Task GivenIHaveAnImage_WhenIConvertItToPixelsArray_RgbValuesAreReturnedInTheArray()
        {
            using (var testImage = new TestResource(GetType().Assembly, "FaceRecognition.UnitTests.Images.Resources.Anthony_Hopkins_0001.jpg"))
            {
                var image = await BaseImage.CreateAsync(testImage.ResourcePath).ConfigureAwait(false);
                using (var rgbPixels = image.ToColorPixelsArray(4, 4))
                {
                    rgbPixels.XLength.Should().Be(4);
                    rgbPixels.YLength.Should().Be(4);
                    rgbPixels.ZLength.Should().Be(3);
                }
            }
        }
    }
}