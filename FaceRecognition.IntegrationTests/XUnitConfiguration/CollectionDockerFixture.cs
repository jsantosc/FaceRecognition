using FaceRecognition.CommonTests.Fixtures;
using Xunit;

namespace FaceRecognition.IntegrationTests.XUnitConfiguration
{
    [CollectionDefinition(Collections.IntegrationTets, DisableParallelization = false)]
    public class CollectionDockerFixture : ICollectionFixture<DockerFixture>
    {
    }
}