using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FaceRecognition.Common;
using FaceRecognition.CommonTests.Fixtures;
using FaceRecognition.Domain.Helpers;
using FaceRecognition.Domain.Images;
using FaceRecognition.IntegrationTests.XUnitConfiguration;
using FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation;
using FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace FaceRecognition.IntegrationTests.ServiceBusBridge.Tensorflow.Evaluation
{
    [Collection(Collections.IntegrationTets)]
    public class PNetClientShould
    {
        private readonly DockerFixture _dockerFixture;

        public PNetClientShould(DockerFixture dockerFixture)
        {
            _dockerFixture = dockerFixture;
        }

        [Fact]
        public async Task GetResponse_WhenAMessageIsSent()
        {
            const int width = 615;
            const int height = 384;
            float[,,] normalizedValues;
            PNetResponseDto expectedPNetResponse;

            var image = await BaseImage
                .CreateAsync("ServiceBusBridge/Tensorflow/Evaluation/Resources/image_020_1.jpg")
                .ConfigureAwait(false);
            using (var rgbValues = image.ToColorPixelsArray(width, height))
            {
                normalizedValues = rgbValues.NormalizeBatch();
            }
            using (var streamReader = File.OpenText("ServiceBusBridge/Tensorflow/Evaluation/Resources/PNetResponse.json"))
            {
                string json = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                expectedPNetResponse = JsonConvert.DeserializeObject<PNetResponseDto>(json);
            }
            using (var rpcClient = new PNetClient("localhost"))
            {
                var response = await rpcClient.SendAsync(new PNetRequestDto(normalizedValues, LoadWeightsMode.FromJson, "/facerec/Tests/PNet.json")).ConfigureAwait(false);
                response.IsSuccess.Should().BeTrue();
                response.ErrorMessage.Should().BeEmpty();
                response.OutputValues.Count().Should().Be(expectedPNetResponse.OutputValues.Count());
                foreach (var outputValue in response.OutputValues)
                {
                    var expectedOutputValue = expectedPNetResponse.OutputValues.SingleOrDefault(v => v.Name == outputValue.Name);
                    expectedOutputValue.Should().NotBeNull();
                    outputValue.Value.GetLength(0).Should().Be(expectedOutputValue.Value.GetLength(0));
                    outputValue.Value.GetLength(1).Should().Be(expectedOutputValue.Value.GetLength(1));
                    outputValue.Value.GetLength(2).Should().Be(expectedOutputValue.Value.GetLength(2));
                    outputValue.Value.GetLength(3).Should().Be(expectedOutputValue.Value.GetLength(3));
                    for (int c = 0; c < outputValue.Value.GetLength(0); c++)
                    {
                        for (int x = 0; x < outputValue.Value.GetLength(1); x++)
                        {
                            for (int y = 0; y < outputValue.Value.GetLength(2); y++)
                            {
                                for (int z = 0; z < outputValue.Value.GetLength(3); z++)
                                {
                                    (outputValue.Value[c, x, y, z] - outputValue.Value[c, x, y, z]).Should()
                                        .BeLessOrEqualTo(Bootstrapper.FloatTolerance);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}