using System.Threading.Tasks;

namespace FaceRecognition.Domain.Images
{
    public interface IWorkingImage
    {
        Task<BaseImage> GenerateGrayAsync();
        Task<BaseImage> GenerateFlip(FlipMode flipMode);
        Task<BaseImage> GenerateBlur(float sigma = 3);
        Task<BaseImage> GenerateElasticSearch(int kernelSize = 13, int sigma = 6, float alpha = 36);
        Task<BaseImage> Rotate(double angle);
        Task<BaseImage> CropAndResize(int x, int y, int width, int heigh, int newWidth, int newHeight);
    }
}