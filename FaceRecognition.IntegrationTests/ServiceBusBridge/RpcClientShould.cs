using System.Threading.Tasks;
using FaceRecognition.ServiceBus.Bridge.Tensorflow.Evaluation;
using FluentAssertions;
using Xunit;

namespace FaceRecognition.IntegrationTests.ServiceBusBridge
{
    public class RpcClientShould
    {
        [Fact]
        public async Task GetResponse_WhenAMessageIsSent()
        {
            // TODO: Launch Docker in Test
            using (var rpcClient = new ONetClient("localhost"))
            {
                var response = rpcClient.Send("Hola");
                response.Should().Be("respuesta");
            }
            using (var rpcClient = new ONetClient("localhost"))
            {
                var response = await rpcClient.SendAsync("Hola").ConfigureAwait(false);
                response.Should().Be("respuesta");
            }
        }
    }
}