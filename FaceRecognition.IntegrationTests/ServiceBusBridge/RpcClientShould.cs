using System.Linq;
using System.Threading.Tasks;
using FaceRecognition.CommonTests.Fixtures;
using FaceRecognition.IntegrationTests.XUnitConfiguration;
using FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation;
using FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation.Dtos;
using FluentAssertions;
using Xunit;

namespace FaceRecognition.IntegrationTests.ServiceBusBridge
{
    [Collection(Collections.IntegrationTets)]
    public class RpcClientShould
    {
        private readonly DockerFixture _dockerFixture;

        public RpcClientShould(DockerFixture dockerFixture)
        {
            _dockerFixture = dockerFixture;
        }

        [Fact]
        public async Task GetResponse_WhenAMessageIsSent()
        {
            // TODO: Launch Docker in Test
            using (var rpcClient = new ONetClient("localhost"))
            {
                var response = rpcClient.Send(new ONetRequestDto(new float[0, 0, 0]));
                response.IsSuccess.Should().BeTrue();
                response.ErrorMessage.Should().BeEmpty();
                response.OutputValues.Count().Should().Be(1);
                response.OutputValues.First().Name.Should().Be("conv4-1");
                response.OutputValues.First().Value[0, 0, 0, 0].Should().Be(0);
                response.OutputValues.First().Value[0, 0, 0, 1].Should().Be(1);
                response.OutputValues.First().Value[0, 0, 0, 2].Should().Be(2);
                response.OutputValues.First().Value[0, 0, 0, 3].Should().Be(3);
            }
            using (var rpcClient = new ONetClient("localhost"))
            {
                var response = await rpcClient.SendAsync(new ONetRequestDto(new float[0, 0, 0])).ConfigureAwait(false);
                response.IsSuccess.Should().BeTrue();
                response.ErrorMessage.Should().BeEmpty();
                response.OutputValues.Count().Should().Be(1);
                response.OutputValues.First().Name.Should().Be("conv4-1");
                response.OutputValues.First().Value[0, 0, 0, 0].Should().Be(0);
                response.OutputValues.First().Value[0, 0, 0, 1].Should().Be(1);
                response.OutputValues.First().Value[0, 0, 0, 2].Should().Be(2);
                response.OutputValues.First().Value[0, 0, 0, 3].Should().Be(3);
            }
        }
    }
}