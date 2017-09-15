using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using FaceRecognition.Common;
using FaceRecognition.Common.MemoryOptimizations;

namespace FaceRecognition.Domain.Helpers
{
    public static class GeometryHelpers
    {
        private static readonly Dictionary<(int dim, double sigma), double[]> Cached2DGaussians = new Dictionary<(int dim, double sigma), double[]>();

        /// <summary>
        /// Creates a 2d gaussian kernel with the standard deviation denoted by sigma
        /// </summary>
        /// <param name="dim">Integer denoting a side(1 - d) of gaussian kernel</param>
        /// <param name="sigma">The standard deviation of the gaussian kernel</param>
        /// <returns>2D array with the gaussian kernel values</returns>
        public static double[] Create2Dgaussian(int dim, double sigma)
        {
            if (dim % 2 == 0)
                throw new Exception("Kernel dimension should be odd");

            var cacheKey = (dim, sigma);
            if (Cached2DGaussians.ContainsKey(cacheKey))
            {
                return Cached2DGaussians[(dim, sigma)];
            }
            var kernel = new double[dim * dim];
            var center = dim / 2;
            var variance = Math.Pow(sigma, 2);
            var coeff = 1.0 / (2 * variance);
            var denom = 2 * variance;
            double sum = 0;

            for (int x = 0; x < dim; x++)
            {
                for (int y = 0; y < dim; y++)
                {
                    var xVal = Math.Abs(x - center);
                    var yVal = Math.Abs(y - center);
                    var numerator = Math.Pow(xVal, 2) + Math.Pow(yVal, 2);
                    var position = x + y * dim;

                    kernel[position] = coeff * Math.Exp(-1 * numerator / denom);
                    sum += kernel[position];
                }
            }

            // normalise the kernel
            var gaussian = kernel.Divide(sum);
            Cached2DGaussians[cacheKey] = gaussian;
            return gaussian;
        }
        public static double[] Divide(this double[] matrix, double factor)
        {
            Parallel.For(0, matrix.Length, Bootstrapper.Instance.MaxDegreeOfParalelism, x =>
            //for (int x = 0; x < matrix.GetLength(0); x++)
            {
                matrix[x] /= factor;
            });
            return matrix;
        }
        public static double[] Multiply(this double[] matrix, double factor)
        {
            Parallel.For(0, matrix.Length, Bootstrapper.Instance.MaxDegreeOfParalelism, x =>
            {
                matrix[x] *= factor;
            });
            return matrix;
        }
        public static double[] Convolve(this double[] matrix, int matrixWidth, int matrixHeight, double[] kernel, int kernelSize)
        {
            if (kernel.Length != kernelSize * kernelSize)
            {
                throw new Exception();
            }
            if (kernelSize % 2 == 0)
            {
                throw new Exception();
            }
            int kernelCenter = kernelSize / 2;
            double[] output = ArrayPoolManager.DoubleArrayPool.Rent(matrixWidth * matrixHeight);

            Parallel.For(0, matrixHeight, Bootstrapper.Instance.MaxDegreeOfParalelism, y =>
            //for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < matrixWidth; x++)
                {
                    double sum = 0;

                    for (int matrixY = -kernelCenter; matrixY <= kernelCenter; matrixY++)
                    {
                        for (int matrixX = -kernelCenter; matrixX <= kernelCenter; matrixX++)
                        {
                            // these coordinates will be outside the bitmap near all edges
                            int sourceX = x + matrixX;
                            int sourceY = y + matrixY;

                            if (sourceX < 0
                                || sourceX >= matrixWidth
                                || sourceY < 0
                                || sourceY >= matrixHeight)
                            {
                                continue;
                            }
                            sum += matrix[sourceX + sourceY * matrixWidth] * kernel[kernelCenter - matrixX + (kernelCenter - matrixY) * kernelSize];
                        }
                    }
                    output[x + y * matrixWidth] = sum;
                }
            });
            ArrayPoolManager.DoubleArrayPool.Return(matrix);
            return output;
        }
        public static double[] Random(this double[] matrix, int min, int max)
        {
            var random = new Random();

            Parallel.For(0, matrix.Length, Bootstrapper.Instance.MaxDegreeOfParalelism, x =>
            {
                matrix[x] = random.Next(min, max + 1);
            });
            return matrix;
        }
        public static int ClampDown(this int value, int minValue)
        {
            return Math.Max(value, minValue);
        }
        public static int ClampUp(this int value, int maxValue)
        {
            return Math.Min(value, maxValue);
        }
        public static Vector2 RotatePoint(this Vector2 point, float xCenter, float yCenter, float angleDegree)
        {
            var radians = angleDegree.ToRadians();
            var sin = Math.Sin(radians);
            var cos = Math.Cos(radians);
            var destPoints = new Vector2(point.X, point.Y);

            // translate point back to origin:
            destPoints.X -= xCenter;
            destPoints.Y -= yCenter;

            // rotate point
            destPoints.X = (float)(destPoints.X * cos - destPoints.Y * sin);
            destPoints.Y = (float)(destPoints.X * sin + destPoints.Y * cos);

            // translate point back:
            destPoints.X += xCenter;
            destPoints.Y += yCenter;
            return destPoints;
        }
        public static double ToRadians(this float angleDegree)
        {
            return (angleDegree * Math.PI) / 180;
        }

        public static int Round(this double value)
        {
            return (int)Math.Round(value, 0);
        }

    }
}