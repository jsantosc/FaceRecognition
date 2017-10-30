using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FaceRecognition.CommonTests;
using FaceRecognition.Domain.Helpers;
using FaceRecognition.Domain.Images;
using FluentAssertions;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using TensorflowSharpCore;
using TensorflowSharpCore.LearnApi;
using TensorflowSharpCore.LearnApi.Layers;
using Xunit;
using TFGraph = TensorflowSharpCore.TFGraph;
using System.Collections.Generic;

namespace FaceRecognition.UnitTests.TensorflowSharpCore
{
    public class TensorflowTest
    {
        [Fact]
        public void GivenICreateANewGraphWithTensorflowVersion_WhenIRunTheSession_ThenTheOutputContainsTheTensorflowVersion()
        {
            var value = $"Hello from {TFCore.Version}";

            using (var graph = new TFGraph())
            using (var tensor = TFTensor.CreateString(Encoding.UTF8.GetBytes(value)))
            {
                var output = graph.Const(tensor, tensor.TensorType, "test");
                using (var session = new TFSession(graph))
                {
                    var run = session.GetRunner().Fetch(output).Run();
                    var bytes = run[0];
                    var bytesDest = new byte[(int)bytes.TensorByteSize];
                    Marshal.Copy(bytes.Data, bytesDest, 0, bytesDest.Length);

                    Encoding.UTF8.GetString(bytesDest).Should().Contain(value);
                }
            }
        }
        [Fact]
        public async Task GivenIHaveAnMtcnnPNetDefinition_WhenIRunItWithValues_ThenWeObtainTheExpectedValues()
        {
            using (var stream = File.OpenRead("TensorflowSharpCore/Resources/mtcnn1.json"))
            using (var textReader = new StreamReader(stream))
            {
                string json = await textReader.ReadToEndAsync().ConfigureAwait(false);
                var obj = JsonConvert.DeserializeObject<LayerSerializationModel[]>(json);
                var network = new Network();

                var conv1 = obj.Single(o => o.Name == "conv1");
                var conv2 = obj.Single(o => o.Name == "conv2");
                var conv3 = obj.Single(o => o.Name == "conv3");
                var conv41 = obj.Single(o => o.Name == "conv4-1");
                var conv42 = obj.Single(o => o.Name == "conv4-2");
                var prelu1 = obj.Single(o => o.Name == "PReLU1");
                var prelu2 = obj.Single(o => o.Name == "PReLU2");
                var prelu3 = obj.Single(o => o.Name == "PReLU3");
                network.AddInput("data", TfConstants.NullDimension, TfConstants.NullDimension, 3, 1)
                    .ContinueWithConv2D("conv1", (3, 3, 10), (1, 1), PaddingType.Valid, conv1.Weights, biases: conv1.Biases, addReLu: false)
                    .ContinueWithPRelu("PReLU1", prelu1.Alpha)
                    .ContinueWithConv2D("conv2", (3, 3, 16), (1, 1), PaddingType.Same, conv2.Weights, biases: conv2.Biases, addReLu: false)
                    .ContinueWithPRelu("PReLU2", prelu2.Alpha)
                    .ContinueWithConv2D("conv3", (3, 3, 32), (1, 1), PaddingType.Same, conv3.Weights, biases: conv3.Biases, addReLu: false)
                    .ContinueWithPRelu("PReLU3", prelu3.Alpha)
                    .ContinueWithConv2D("conv4-1", (1, 1, 2), (1, 1), PaddingType.Same, conv41.Weights, biases: conv41.Biases)
                    .ContinueWithMultidimensionalSoftMax("prob1", 3);
                network.FromLayer("PReLU3")
                    .ContinueWithConv2D("conv4-2", (1, 1, 4), (1, 1), PaddingType.Same, conv42.Weights, biases: conv42.Biases);

                float[,,,] inputValues;
                //using (var imageResource = new TestResource(GetType().Assembly, "FaceRecognition.UnitTests.FaceDetections.Algorithms.Resources.image_020_1.jpg"))
                //using (var imageStream = File.OpenRead(imageResource.ResourcePath))
                //{
                //    var image = Image.Load(imageStream);
                //    var baseImage = await BaseImage.CreateAsync(image, "image_020_1.jpg").ConfigureAwait(false);

                //    using (var inputArrayValues = baseImage.ToColorPixelsArray(615, 384))
                //    {
                //        inputValues = inputArrayValues.NormalizeBatch();
                //    }
                //}
                using (var jsonResurce = new TestResource(GetType().Assembly, "FaceRecognition.UnitTests.TensorflowSharpCore.Resources.data.json"))
                {
                    inputValues = JsonConvert.DeserializeObject<float[,,,]>(File.ReadAllText(jsonResurce.ResourcePath));
                }
                var networkResults = network.Run(new { data = inputValues }, new[] { "conv4-2", "prob1" });

                var points = new List<Vector2>();
                var prob = networkResults.ElementAt(1);

                System.Diagnostics.Debug.WriteLine($"{points.Count} boxes found");
                networkResults.Count().Should().Be(2);
            }
        }
        public async Task LoadMtcnnRNetNetworkAsync()
        {
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("TensorFlowCore.Test.mtcnn2.json"))
            using (var textReader = new StreamReader(stream))
            {
                string json = await textReader.ReadToEndAsync().ConfigureAwait(false);
                var obj = JsonConvert.DeserializeObject<LayerSerializationModel[]>(json);
                Network network = new Network();

                var conv1 = obj.Single(o => o.Name == "conv1");
                var conv2 = obj.Single(o => o.Name == "conv2");
                var conv3 = obj.Single(o => o.Name == "conv3");
                var conv41 = obj.Single(o => o.Name == "conv4-1");
                var conv42 = obj.Single(o => o.Name == "conv4-2");
                var prelu1 = obj.Single(o => o.Name == "PReLU1");
                var prelu2 = obj.Single(o => o.Name == "PReLU2");
                var prelu3 = obj.Single(o => o.Name == "PReLU3");
                network.AddInput("data", 29, 29, 3, 1)
                    .ContinueWithConv2D("conv1", (3, 3, 10), (1, 1), PaddingType.Same, conv1.Weights, biases: conv1.Biases, addReLu: false)
                    .ContinueWithPRelu("PReLU1", prelu1.Alpha)
                    .ContinueWithConv2D("conv2", (3, 3, 16), (1, 1), PaddingType.Same, conv2.Weights, biases: conv2.Biases, addReLu: false)
                    .ContinueWithPRelu("PReLU2", prelu2.Alpha)
                    .ContinueWithConv2D("conv3", (3, 3, 32), (1, 1), PaddingType.Same, conv3.Weights, biases: conv3.Biases, addReLu: false)
                    .ContinueWithPRelu("PReLU3", prelu3.Alpha)
                    .ContinueWithConv2D("conv4-1", (1, 1, 2), (1, 1), PaddingType.Same, conv41.Weights, biases: conv41.Biases, addReLu: false)
                    .ContinueWithMultidimensionalSoftMax("prob1", 3);
                network.FromLayer("PReLU3")
                    .ContinueWithConv2D("conv4-2", (1, 1, 4), (1, 1), PaddingType.Same, conv42.Weights, biases: conv42.Biases, addReLu: false);
            }
        }

        private class LayerSerializationModel
        {
            public string Name { get; set; }
            public float[] Biases { get; set; }
            public float[] Alpha { get; set; }
            public float[,,,] Weights { get; set; }
        }
    }
}