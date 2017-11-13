using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FaceRecognition.Common.JSonConverter;
using FaceRecognition.Common.NoLoh.SpanImplementation;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace FaceRecognition.UnitTests.Common.JsonConverter
{
    public class NoLohArray4DConverterShould
    {
        [Fact]
        public async Task DeserializeNoLoh4dArray_When4dJsonArrayIsSupplied()
        {
            string json;
            using (var sReader = File.OpenText("Common/JsonConverter/Resources/ConverterData.json"))
            {
                json = await sReader.ReadToEndAsync().ConfigureAwait(false);
            }
            var value = JsonConvert.DeserializeObject<Dto>(json);
            value.Should().NotBeNull();
            value.IsSuccess.Should().BeTrue();
            value.OutputValues.Should().NotBeEmpty();
            value.OutputValues.Count().Should().Be(2);
            foreach (var item in value.OutputValues)
            {
                item.Value.Dispose();
            }
        }

        public class Dto
        {
            public bool IsSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public IEnumerable<InnerDto> OutputValues { get; set; }
        }
        public class InnerDto
        {
            public string Name { get; set; }
            [JsonConverter(typeof(NoLohArray4DConverter))]
            public NoLohArray4D<float> Value { get; set; }
        }
    }
}