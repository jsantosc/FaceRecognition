using System.Threading.Tasks;
using FaceRecognition.CommonTests;
using FaceRecognition.Domain.FaceDetections.Algorithms;
using FaceRecognition.Domain.Images;
using Xunit;

namespace FaceRecognition.UnitTests.FaceDetections.Algorithms
{
    public class MtcnnTest : BaseTest
    {
        [Fact]
        public async Task GivenIHaveAnImage_WhenIUseItWithTheMtcnnAlgorithm_ThenFaceLocationAndLandmarksAreReturned()
        {
            using (var testImage = new TestResource(GetType().Assembly, "FaceRecognition.UnitTests.FaceDetections.Algorithms.Resources.Anthony_Hopkins_0001.jpg"))
            {
                var image = await BaseImage.CreateAsync(testImage.ResourcePath).ConfigureAwait(false);
                var mtcnnAlgorithm = new Mtcnn();

                await mtcnnAlgorithm.LocateFacesAsync(image).ConfigureAwait(false);
            }
        }
    }
}