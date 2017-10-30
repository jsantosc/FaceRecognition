namespace TensorflowSharpCore.LearnApi.Results
{
    public class DoubleNetworkResult : NetworkResult<double>
    {
        public DoubleNetworkResult(int dimensions, object value, string networkLayerName)
            : base(dimensions, value, networkLayerName)
        {
        }
    }
}