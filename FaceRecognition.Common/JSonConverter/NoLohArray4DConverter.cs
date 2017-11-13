using System;
using FaceRecognition.Common.NoLoh.SpanImplementation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FaceRecognition.Common.JSonConverter
{
    public class NoLohArray4DConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonArray = JArray.Load(reader);
            var firstDim = jsonArray.Count;
            var secondArray = jsonArray[0] as JArray;
            var secondDim = secondArray?.Count;
            var thirdArray = secondArray?[0] as JArray;
            var thirdDim = thirdArray?.Count;
            var fourArray = thirdArray?[0] as JArray;
            var fourDim = fourArray?.Count;

            if (secondDim == null || thirdDim == null || fourDim == null)
            {
                return null;
            }

            var netArray = new NoLohArray4D<float>(firstDim, secondDim.Value, thirdDim.Value, fourDim.Value);
            for (int x = 0; x < firstDim; x++)
            {
                secondArray = jsonArray[x] as JArray;
                if (secondArray == null)
                {
                    throw new Exception("The json is malformed");
                }
                for (int y = 0; y < secondDim; y++)
                {
                    thirdArray = secondArray[y] as JArray;
                    if (thirdArray == null)
                    {
                        throw new Exception("The json is malformed");
                    }
                    for (int z = 0; z < thirdDim; z++)
                    {
                        fourArray = thirdArray[z] as JArray;
                        if (fourArray == null)
                        {
                            throw new Exception("The json is malformed");
                        }
                        for (int w = 0; w < fourDim; w++)
                        {
                            netArray[x, y, z, w] = fourArray[w].Value<float>();
                        }
                    }
                }
            }
            return netArray;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(NoLohArray4D<float>).IsAssignableFrom(objectType);
        }
    }
}