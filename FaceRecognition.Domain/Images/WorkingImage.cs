using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using FaceRecognition.Common;
using FaceRecognition.Common.MemoryOptimizations;
using FaceRecognition.Domain.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace FaceRecognition.Domain.Images
{
    public class WorkingImage : IDisposable
    {
        private readonly BaseImage _sourceImage;

        internal WorkingImage(BaseImage sourceImage)
        {
            _sourceImage = sourceImage;
        }

        public async Task<BaseImage> GenerateGrayAsync()
        {
            if (!_sourceImage.IsRgb)
            {
                throw new Exception($"The image {_sourceImage.Id} is already converted to gray. Use the original one");
            }
            using (Stream sourceStream = File.OpenRead(_sourceImage.GetFileServerPath()))
            using (var sourceImg = Image.Load(sourceStream))
            {
                sourceImg.Mutate(x => x.Grayscale());
                return await BaseImage.CreateAsync(sourceImg, _sourceImage.Name, Effect.ConvertToGrayEffect(_sourceImage.AppliedEffects.LastOrDefault()), _sourceImage, _sourceImage.FacePoints,
                    xTop: _sourceImage.XTopLeftOriginal, yTop: _sourceImage.YTopLeftOriginal, xBottom: _sourceImage.XBottomRightOriginal, yBottom: _sourceImage.YBottomRightOriginal, isRgb: false).ConfigureAwait(false);
            }
        }
        public async Task<BaseImage> GenerateFlipAsync(FlipMode flipMode)
        {
            if (_sourceImage.AppliedEffects.Any(e => e.Type.HasFlag(EffectType.FlippedHorizontal) || e.Type.HasFlag(EffectType.FlippedVertical) || e.Type.HasFlag(EffectType.FlippedBoth)))
            {
                throw new Exception($"The image {_sourceImage.Id} is already flipped. Use the original one");
            }
            using (var sourceStream = File.OpenRead(_sourceImage.GetFileServerPath()))
            using (var sourceImg = Image.Load(sourceStream))
            {
                switch (flipMode)
                {
                    case FlipMode.Horizontal:
                        sourceImg.Mutate(x => x.Flip(FlipType.Horizontal));
                        return await BaseImage.CreateAsync(sourceImg, _sourceImage.Name, Effect.FlipEffect(flipMode, _sourceImage.AppliedEffects.LastOrDefault()), _sourceImage, _sourceImage.FacePoints.Select(p => new Vector2(_sourceImage.Width - p.X, p.Y)),
                            xTop: _sourceImage.XTopLeftOriginal, yTop: _sourceImage.YTopLeftOriginal, xBottom: _sourceImage.XBottomRightOriginal, yBottom: _sourceImage.YBottomRightOriginal, isRgb: _sourceImage.IsRgb).ConfigureAwait(false);
                    case FlipMode.Vertical:
                        sourceImg.Mutate(x => x.Flip(FlipType.Vertical));
                        return await BaseImage.CreateAsync(sourceImg, _sourceImage.Name, Effect.FlipEffect(flipMode, _sourceImage.AppliedEffects.LastOrDefault()), _sourceImage, _sourceImage.FacePoints.Select(p => new Vector2(p.X, _sourceImage.Height - p.Y)),
                            xTop: _sourceImage.XTopLeftOriginal, yTop: _sourceImage.YTopLeftOriginal, xBottom: _sourceImage.XBottomRightOriginal, yBottom: _sourceImage.YBottomRightOriginal, isRgb: _sourceImage.IsRgb).ConfigureAwait(false);
                    case FlipMode.Both:
                        sourceImg.Mutate(x => x.Flip(FlipType.Horizontal).Flip(FlipType.Vertical));
                        return await BaseImage.CreateAsync(sourceImg, _sourceImage.Name, Effect.FlipEffect(flipMode, _sourceImage.AppliedEffects.LastOrDefault()), _sourceImage, _sourceImage.FacePoints.Select(p => new Vector2(_sourceImage.Width - p.X, _sourceImage.Height - p.Y)),
                            xTop: _sourceImage.XTopLeftOriginal, yTop: _sourceImage.YTopLeftOriginal, xBottom: _sourceImage.XBottomRightOriginal, yBottom: _sourceImage.YBottomRightOriginal, isRgb: _sourceImage.IsRgb).ConfigureAwait(false);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(flipMode), flipMode, null);
                }
            }
        }

        public async Task<BaseImage> GenerateBlurAsync(float sigma = 3)
        {
            if (_sourceImage.AppliedEffects.Any(e => e.Type.HasFlag(EffectType.Blurred)))
            {
                throw new Exception($"The image {_sourceImage.Id} is already blurred. Use the original one");
            }
            using (var sourceStream = File.OpenRead(_sourceImage.GetFileServerPath()))
            using (var sourceImg = Image.Load(sourceStream))
            {
                sourceImg.Mutate(x => x.GaussianBlur(sigma));
                return await BaseImage.CreateAsync(sourceImg, _sourceImage.Name,
                    Effect.BlurEffect(sigma, _sourceImage.AppliedEffects.LastOrDefault()), _sourceImage,
                    _sourceImage.FacePoints,
                    xTop: _sourceImage.XTopLeftOriginal, yTop: _sourceImage.YTopLeftOriginal,
                    xBottom: _sourceImage.XBottomRightOriginal, yBottom: _sourceImage.YBottomRightOriginal,
                    isRgb: _sourceImage.IsRgb).ConfigureAwait(false);
            }
        }

        public async Task<BaseImage> RotateAsync(float angle)
        {
            if (_sourceImage.AppliedEffects.Any(e => e.Type.HasFlag(EffectType.Rotated)))
            {
                throw new Exception($"The image {_sourceImage.Id} is already rotated. Use the original one");
            }
            using (var sourceStream = File.OpenRead(_sourceImage.GetFileServerPath()))
            using (var sourceImg = Image.Load(sourceStream))
            {
                int originalWidth = sourceImg.Width;
                int originalHeight = sourceImg.Height;

                sourceImg.Mutate(x => x.Rotate(angle)); //TODO: Review why we must expand image (otherwise it fails)
                                                        //Recenter the image
                var addedWidth = (sourceImg.Width - originalWidth) / 2f;
                var addedHeight = (sourceImg.Height - originalHeight) / 2f;
                var addedWidthInt = (int)addedWidth;
                var addedHeightInt = (int)addedHeight;
                var vectorToAdd = new Vector2(addedWidth - addedWidthInt, addedHeight - addedHeightInt);

                var xCenter = originalWidth / 2f;
                var yCenter = originalHeight / 2f;
                var facePoints = _sourceImage.FacePoints.Select(p => p.RotatePoint(xCenter, yCenter, angle) + vectorToAdd).ToArray();

                sourceImg.Mutate(x => x.Crop(new Rectangle(addedWidthInt, addedHeightInt, originalWidth, originalHeight)));
                var img = await BaseImage.CreateAsync(sourceImg, _sourceImage.Name, Effect.RotationEffect(angle, _sourceImage.AppliedEffects.LastOrDefault()), _sourceImage, isRgb: _sourceImage.IsRgb).ConfigureAwait(false);

                img.SetFaceLandmarks(facePoints);
                return img;
            }
        }
        //public async Task<BaseImage> GenerateElasticSearchAsync(int kernelSize = 13, double sigma = 6, double alpha = 36)
        //{
        //    if (_sourceImage.AppliedEffects.Any(e => e.Type.HasFlag(EffectType.ElasticDistorsionated)))
        //    {
        //        throw new Exception($"The image {_sourceImage.Id} is already elastic distorsioned. Use the original one");
        //    }
        //    using (var sourceStream = File.OpenRead(_sourceImage.GetFileServerPath()))
        //    using (var sourceImg = Image.Load(sourceStream))
        //    using (var imageE = new Image<Rgba32>(sourceImg.Width, sourceImg.Height))
        //    {
        //        Vector2[] destFacePoints = new Vector2[_sourceImage.FacePoints.Length];

        //        Image.
        //        GenerateElasticPixels(kernelSize, sigma, alpha, sourceImg.Pixels, imageE.Pixels, sourceImg.Width, sourceImg.Height, _sourceImage.FacePoints, destFacePoints);
        //        return await BaseImage.CreateAsync(imageE, _sourceImage.Name, Effect.ElasticDeformationEffect(sigma, alpha, kernelSize, _sourceImage.AppliedEffects.LastOrDefault()), _sourceImage, destFacePoints,
        //            xTop: _sourceImage.XTopLeftOriginal, yTop: _sourceImage.YTopLeftOriginal, xBottom: _sourceImage.XBottomRightOriginal, yBottom: _sourceImage.YBottomRightOriginal, isRgb: _sourceImage.IsRgb).ConfigureAwait(false);
        //    }
        //}
        public async Task<BaseImage> CropAndResizeAsync(int x, int y, int width, int height, int newWidth, int newHeight)
        {
            const float minDownscaleFactor = 0.5f;
            var xFactor = (1f * newWidth) / width;
            var yFactor = (1f * newHeight) / height;
            float xFactorStep = xFactor < minDownscaleFactor ? minDownscaleFactor : xFactor;
            float yFactorStep = yFactor < minDownscaleFactor ? minDownscaleFactor : yFactor;

            using (var sourceStream = File.OpenRead(_sourceImage.GetFileServerPath()))
            using (var sourceImg = Image.Load(sourceStream))
            {
                sourceImg.Mutate(img => img.Crop(new Rectangle(x, y, width, height)).Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Stretch,
                    Size = new Size((int)Math.Round(width * xFactorStep), (int)Math.Round(height * yFactorStep)),
                    Sampler = new Lanczos3Resampler()
                }));
                while (sourceImg.Width > newWidth)
                {
                    xFactorStep = (1f * newWidth) / sourceImg.Width;
                    xFactorStep = xFactorStep < minDownscaleFactor ? minDownscaleFactor : xFactorStep;
                    yFactorStep = (1f * newHeight) / sourceImg.Height;
                    yFactorStep = yFactorStep < minDownscaleFactor ? 0.5f : yFactorStep;
                    sourceImg.Mutate(img => img.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Stretch,
                        Size = new Size((int)Math.Round(sourceImg.Width * xFactorStep), (int)Math.Round(sourceImg.Height * yFactorStep)),
                        Sampler = new Lanczos3Resampler()
                    }));
                }
                int? xTop = _sourceImage.XTopLeftOriginal - x;
                int? yTop = _sourceImage.YTopLeftOriginal - y;
                int? xBottom = xTop + newWidth;
                int? yBottom = yTop + newHeight;

                var facePoints = _sourceImage.FacePoints.Select(p => new Vector2(xFactor * (p.X - x), yFactor * (p.Y - y)));

                return await BaseImage.CreateAsync(sourceImg, _sourceImage.Name, Effect.CropAndResizeEffect(_sourceImage.AppliedEffects.LastOrDefault()), _sourceImage, facePoints,
                    xTop: xTop, yTop: yTop, xBottom: xBottom, yBottom: yBottom, isRgb: _sourceImage.IsRgb).ConfigureAwait(false);
            }
        }


        private void GenerateElasticPixels<TColor>(
            int kernelSize,
            double sigma,
            double alpha,
            Span<TColor> sourcePixels,
            Span<TColor> destPixels,
            int width,
            int height,
            Vector2[] facePointsSource,
            Vector2[] facePointsDest)
            where TColor : struct, IPixel<TColor>
        {
            //var length = width * height;
            double[] kernel = GeometryHelpers.Create2Dgaussian(kernelSize, sigma);
            double[] displacementX = ArrayPoolManager.DoubleArrayPool.Rent(width * height).Random(-1, 1).Multiply(alpha).Convolve(width, height, kernel, kernelSize);
            double[] displacementY = ArrayPoolManager.DoubleArrayPool.Rent(width * height).Random(-1, 1).Multiply(alpha).Convolve(width, height, kernel, kernelSize);
            //double[,] displacementX = new double[width, height];
            //double[,] displacementY = new double[width, height];
            double[] distances = ArrayPoolManager.DoubleArrayPool.Rent(facePointsSource.Length);

            for (int i = 0; i < facePointsSource.Length; i++)
            {
                distances[i] = double.MaxValue;
            }
            try
            {
                //displacementX = displacementX.Random(-1, 1).Multiply(alpha).Convolve(kernel);
                //displacementY = displacementY.Random(-1, 1).Multiply(alpha).Convolve(kernel);

                Parallel.For(0, width, Bootstrapper.Instance.MaxDegreeOfParalelism, xDest =>
                {
                    for (int yDest = 0; yDest < height; yDest++)
                    {
                        var position = xDest + yDest * width;

                        double xOrg = xDest + displacementX[position];
                        double yOrg = yDest + displacementY[position];

                        SetNearest(facePointsSource, facePointsDest, distances, xOrg, yOrg);

                        int xI1 = (int)Math.Floor(xOrg);
                        int xI2 = (int)Math.Ceiling(xOrg);
                        int yI1 = (int)Math.Floor(yOrg);
                        int yI2 = (int)Math.Ceiling(yOrg);

                        int xOrgI1 = xI1.ClampDown(0).ClampUp(width - 1);
                        int xOrgI2 = xI2.ClampDown(0).ClampUp(width - 1);
                        int yOrgI1 = yI1.ClampDown(0).ClampUp(height - 1);
                        int yOrgI2 = yI2.ClampDown(0).ClampUp(height - 1);

                        TColor colorPosI1 = sourcePixels[xOrgI1 + yOrgI1 * width];
                        TColor colorPosI2 = sourcePixels[xOrgI2 + yOrgI1 * width];
                        TColor colorPosI3 = sourcePixels[xOrgI1 + yOrgI2 * width];
                        TColor colorPosI4 = sourcePixels[xOrgI2 + yOrgI2 * width];

                        var colorPosI1Vector = colorPosI1.ToVector4();
                        var colorPosI2Vector = colorPosI2.ToVector4();
                        var colorPosI3Vector = colorPosI3.ToVector4();
                        var colorPosI4Vector = colorPosI4.ToVector4();

                        var rgbVector = new Vector4();
                        rgbVector.X = Interpolate(xOrg, yOrg, xI1, xI2, yI1, yI2, colorPosI1Vector.X, colorPosI2Vector.X,
                            colorPosI3Vector.X, colorPosI4Vector.X);
                        rgbVector.Y = Interpolate(xOrg, yOrg, xI1, xI2, yI1, yI2, colorPosI1Vector.Y, colorPosI2Vector.Y,
                            colorPosI3Vector.Y, colorPosI4Vector.Y);
                        rgbVector.Z = Interpolate(xOrg, yOrg, xI1, xI2, yI1, yI2, colorPosI1Vector.Z, colorPosI2Vector.Z,
                            colorPosI3Vector.Z, colorPosI4Vector.Z);
                        rgbVector.W = 255;

                        TColor newColor = new TColor();
                        newColor.PackFromVector4(rgbVector);
                        destPixels[xDest + yDest * width] = newColor;
                    }
                });
            }
            finally
            {
                ArrayPoolManager.DoubleArrayPool.Return(displacementX);
                ArrayPoolManager.DoubleArrayPool.Return(displacementY);
                ArrayPoolManager.DoubleArrayPool.Return(distances);
            }
        }

        private void SetNearest(Vector2[] source, Vector2[] dest, double[] distances, double x, double y)
        {
            for (var index = 0; index < source.Length; index++)
            {
                var sourcePoint = source[index];

                var distance = Math.Abs(sourcePoint.X - x) + Math.Abs(sourcePoint.Y - y);

                if (distances[index] > distance)
                {
                    dest[index] = new Vector2((float)x, (float)y);
                    distances[index] = distance;
                }
            }
        }

        private byte Interpolate(double xOrg, double yOrg, int xI1, int xI2, int yI1, int yI2, float colorPosI1, float colorPosI2, float colorPosI3, float colorPosI4)
        {
            var color = (colorPosI1 * (xOrg - xI1)
                         + colorPosI2 * (xI2 - xOrg)) * (yOrg - yI1)
                        + (colorPosI3 * (xOrg - xI1)
                           + colorPosI4 * (xI2 - xOrg)) * (yI2 - yOrg);
            return (byte)color;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}