using System;
using FaceRecognition.CommonTests.Fixtures;
using Xunit;

namespace FaceRecognition.IntegrationTests
{
    public class UnitTest1 : IClassFixture<DockerFixture>
    {
        public UnitTest1(DockerFixture dockerFixture)
        {
        }

        [Fact]
        public void Test1()
        {

        }
    }
}
