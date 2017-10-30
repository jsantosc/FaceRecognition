namespace TensorflowSharpCore.LearnApi.Results
{
    public class FloatNetworkResult : NetworkResult<float>
    {
        public FloatNetworkResult(int dimensions, object value, string networkLayerName)
            : base(dimensions, value, networkLayerName)
        {
        }
    }
}