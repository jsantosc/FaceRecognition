using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Conditions;
using FaceRecognition.Common;
using FaceRecognition.Common.NoLoh.SpanImplementation;
using FaceRecognition.Common.Specification;
using FaceRecognition.Domain.CoreFramework;
using FaceRecognition.Domain.DeepNeuralNetworks;
using FaceRecognition.Domain.Helpers;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.PixelFormats;
using ImageSharp.Processing;
using Newtonsoft.Json;

namespace FaceRecognition.Domain.Images
{
    public class BaseImage : BaseEntity, IAuditable
    {
        private const string ChildsFolderSufix = ".Childs";

        private static readonly Dictionary<byte[], Func<BinaryReader, Size>> ImageFormatDecodersMagicBytes = new Dictionary<byte[], Func<BinaryReader, Size>>()
        {
            { new byte[]{ 0x42, 0x4D }, DecodeBitmap},
            { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, DecodeGif },
            { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, DecodeGif },
            { new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, DecodePng },
            { new byte[]{ 0xff, 0xd8 }, DecodeJfif },
        };

        public Guid FileServerIdentifier { get; protected set; }
        public string Name { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public ICollection<Effect> AppliedEffects { get; protected set; } = new List<Effect>();
        public string AppliedEffectsJson
        {
            get { return JsonConvert.SerializeObject(AppliedEffects); }
            protected set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    AppliedEffects = JsonConvert.DeserializeObject<List<Effect>>(value);
                }
                else
                {
                    AppliedEffects = new List<Effect>();

                }
            }
        }
        public bool IsOriginal => AppliedEffects == null || !AppliedEffects.Any();
        public virtual BaseImage ParentImage { get; protected set; }
        public int? ParentImageId { get; protected set; }
        public virtual ICollection<BaseImage> ChildImage { get; protected set; } = new List<BaseImage>();
        public Vector2[] FacePoints { get; protected set; } = new Vector2[0];
        public string FacePointsJson
        {
            get { return JsonConvert.SerializeObject(FacePoints.Select(i => new[] { i.X, i.Y })); }
            protected set
            {
                var deserializeArray = JsonConvert.DeserializeObject<float[][]>(value);

                FacePoints = deserializeArray.Select(i => new Vector2(i[0], i[1])).ToArray();
            }
        }
        public int? TrainingSetId { get; protected set; }
        public virtual TrainingSet TrainingSet { get; protected set; }

        public bool IsFaceImage => XTopLeftOriginal == 0 && YTopLeftOriginal == 0 && XBottomRightOriginal == Width && YBottomRightOriginal == Height;
        /// <summary>
        /// Gets or sets the X top left original value where the face is located.
        /// </summary>
        /// <value>
        /// The X top left original value of the face.
        /// </value>
        public int? XTopLeftOriginal { get; protected set; }
        /// <summary>
        /// Gets or sets the Y top left original value where the face is located.
        /// </summary>
        /// <value>
        /// The Y top left original value of the face.
        /// </value>
        public int? YTopLeftOriginal { get; protected set; }
        /// <summary>
        /// Gets or sets the X bottom right original value where the face is located.
        /// </summary>
        /// <value>
        /// The X bottom right original value of the face.
        /// </value>
        public int? XBottomRightOriginal { get; protected set; }
        /// <summary>
        /// Gets or sets the Y bottom right original value where the face is located.
        /// </summary>
        /// <value>
        /// The Y bottom right original value of the face.
        /// </value>
        public int? YBottomRightOriginal { get; protected set; }
        /// <summary>
        /// Gets or sets a value indicating if the image is a color image (<c>true</c>) or gray (<c>false</c>).
        /// </summary>
        /// <value>
        /// <c>True</c> if the image is a color image, otherwise <c>false</c>.
        /// </value>
        public bool IsRgb { get; protected set; }

        public BaseImage()
        {
            FileServerIdentifier = Guid.NewGuid();
        }

        public static async Task<BaseImage> CreateAsync(string fileName, Effect newEffect = null, BaseImage parentImage = null, bool copyToFileServer = true, IEnumerable<Vector2> facePoints = null,
            bool isRgb = true, int? xTop = null, int? yTop = null, int? xBottom = null, int? yBottom = null)
        {
            var fInfo = new FileInfo(fileName);

            BaseImage img = new BaseImage
            {
                Name = fInfo.Name,
            };

            img.GetDimensions(fileName);
            SetImageCommonProperties(img, newEffect, parentImage, facePoints, isRgb, (xTop, yTop, xBottom, yBottom));
            if (copyToFileServer)
            {
                var fsPath = img.GetFileServerPath();
                var fInfoDest = new FileInfo(fsPath);

                if (!fInfoDest.Directory.Exists)
                {
                    fInfoDest.Directory.Create();
                }
                using (var readStream = File.OpenRead(fileName))
                using (var writeStream = File.Create(fsPath))
                {
                    await readStream.CopyToAsync(writeStream).ConfigureAwait(false);
                }
            }
            return img;
        }
        public static async Task<BaseImage> CreateAsync<TColor>(Image<TColor> sourceImage, string name, Effect newEffect = null, BaseImage parentImage = null, IEnumerable<Vector2> facePoints = null,
            bool isRgb = true, int? xTop = null, int? yTop = null, int? xBottom = null, int? yBottom = null)
            where TColor : struct, IPixel<TColor>
        {
            BaseImage img = new BaseImage
            {
                Name = name,
                Width = sourceImage.Width,
                Height = sourceImage.Height,
            };

            SetImageCommonProperties(img, newEffect, parentImage, facePoints, isRgb, (xTop, yTop, xBottom, yBottom));

            var fsPath = img.GetFileServerPath();
            var fInfoDest = new FileInfo(fsPath);

            if (!fInfoDest.Directory.Exists)
            {
                fInfoDest.Directory.Create();
            }
            using (var writeStream = File.Create(fsPath))
            {
                sourceImage.SaveAsJpeg(writeStream, new JpegEncoder { Quality = 100 });
            }
            return await Task.FromResult(img).ConfigureAwait(false);
        }

        private static void SetImageCommonProperties(BaseImage image, Effect newEffect, BaseImage parentImage, IEnumerable<Vector2> facePoints, bool isRgb, (int? xTop, int? yTop, int? xBottom, int? yBottom) faceBox)
        {
            if (parentImage != null)
            {
                image.SetParentImage(parentImage);
                foreach (var effect in parentImage.AppliedEffects)
                {
                    image.AppliedEffects.Add(effect);
                }
            }
            if (newEffect != null)
            {
                image.AppliedEffects.Add(newEffect);
            }
            if (facePoints != null)
            {
                image.FacePoints = facePoints.ToArray();
                //await img.SaveFacePointToFileServerAsync().ConfigureAwait(false);
            }
            image.IsRgb = isRgb;
            if (faceBox.xTop.HasValue && faceBox.yTop.HasValue && faceBox.xBottom.HasValue && faceBox.yBottom.HasValue)
            {
                image.XTopLeftOriginal = faceBox.xTop;
                image.XBottomRightOriginal = faceBox.xBottom;
                image.YTopLeftOriginal = faceBox.yTop;
                image.YBottomRightOriginal = faceBox.yBottom;
            }
        }
        public void SetFaceLandmarks(Vector2[] faceLandmarks, float boundingBoxMargin = 0.05f)
        {
            FacePoints = faceLandmarks;
            //Calculate BoundingBox
            XBottomRightOriginal = (int)Math.Ceiling(FacePoints.Max(coor => coor.X));
            XTopLeftOriginal = (int)Math.Floor(FacePoints.Min(coor => coor.X));
            YBottomRightOriginal = (int)Math.Ceiling(FacePoints.Max(coor => coor.Y));
            YTopLeftOriginal = (int)Math.Floor(FacePoints.Min(coor => coor.Y));

            // Always add boundingBoxMargin to the bounding box to get better face
            var xMargin = (XBottomRightOriginal.Value - XTopLeftOriginal.Value) * boundingBoxMargin;
            var yMargin = (YBottomRightOriginal.Value - YTopLeftOriginal.Value) * boundingBoxMargin;

            // Ensure we apply the same margin to both sides, so we must choose the max allowed
            xMargin = Math.Min(XTopLeftOriginal.Value, Math.Min(xMargin, Width - XBottomRightOriginal.Value));
            yMargin = Math.Min(YTopLeftOriginal.Value, Math.Min(yMargin, Height - YBottomRightOriginal.Value));

            XTopLeftOriginal = (int)Math.Floor(XTopLeftOriginal.Value - xMargin);
            XBottomRightOriginal = (int)Math.Ceiling(XBottomRightOriginal.Value + xMargin);
            YTopLeftOriginal = (int)Math.Floor(YTopLeftOriginal.Value - yMargin);
            YBottomRightOriginal = (int)Math.Ceiling(YBottomRightOriginal.Value + yMargin);

        }
        public string GetFileServerPath()
        {
            string basePath = Path.Combine(FileServer.Instance.ImageDirectory, $"{Name}.{FileServerIdentifier}");

            if (ParentImage != null)
            {
                basePath = $"{ParentImage.GetFileServerPath()}{ChildsFolderSufix}";
            }
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            if (!IsOriginal)
            {
                return Path.Combine(basePath, $"{FileServerIdentifier}.jpg");
            }
            return Path.Combine(basePath, "Original.jpg");
        }
        public void SetParentImage(BaseImage image)
        {
            Condition.Requires(image, nameof(image)).IsNotNull();

            ParentImage = image;
            ParentImageId = image.Id;
            image.ChildImage.Add(this);
        }
        public T GetSpecificImage<T>(ISpecification<T> filter)
            where T : BaseImage
        {
            var filterFunc = filter.SatisfiedBy().Compile();

            return (T)CheckImageSpecification(filterFunc, this);
        }
        public WorkingImage ToWorkingImage()
        {
            return new WorkingImage(this);
        }
        public Rgba32[] ToColorPixels()
        {
            using (Stream sourceStream = File.OpenRead(GetFileServerPath()))
            using (var sourceImg = Image.Load(sourceStream))
            {
                int length = Width * Height;
                Rgba32[] pixels = new Rgba32[length];

                Array.Copy(sourceImg.Pixels.ToArray(), pixels, length);
                return pixels;
            }
        }
        public Rgba32[] ToColorPixels(int desiredWidth, int desiredHeight)
        {
            using (Stream sourceStream = File.OpenRead(GetFileServerPath()))
            using (var sourceImg = Image.Load(sourceStream).Resize(desiredWidth, desiredHeight, new NearestNeighborResampler()))
            {
                int length = desiredWidth * desiredHeight;
                Rgba32[] pixels = new Rgba32[length];

                Array.Copy(sourceImg.Pixels.ToArray(), pixels, length);
                return pixels;
            }
        }

        public NoLohArray3D<float> ToColorPixelsArray(int desiredWidth, int desiredHeight)
        {
            using (Stream sourceStream = File.OpenRead(GetFileServerPath()))
            using (var sourceImg = Image.Load(sourceStream).Resize(desiredWidth, desiredHeight, new NearestNeighborResampler()))
            {
                var destArray = new NoLohArray3D<float>(desiredWidth, desiredHeight, 3);

                Parallel.For(0, desiredHeight, Bootstrapper.Instance.MaxDegreeOfParalelism, row =>
                {
                    int startIndex = row * desiredWidth;

                    for (int column = 0; column < desiredWidth; column++)
                    {
                        var pixelRgb = sourceImg.Pixels[startIndex + column];
                        destArray[row, column, 0] = pixelRgb.R;
                        destArray[row, column, 1] = pixelRgb.G;
                        destArray[row, column, 2] = pixelRgb.B;
                    }
                });
                return destArray;
            }
        }

        #region Get Quick Size Image

        /// <summary>
        /// Gets the dimensions of an image.
        /// </summary>
        /// <param name="path">The path of the image to get the dimensions of.</param>
        /// <returns>The dimensions of the specified image.</returns>
        /// <exception cref="ArgumentException">The image was of an unrecognized format.</exception>
        private void GetDimensions(string path)
        {
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
            {
                try
                {
                    var dims = GetDimensions(binaryReader);

                    Width = dims.Width;
                    Height = dims.Height;
                }
                catch (ArgumentException e)
                {
                    if (e.Message.StartsWith("Can't identify image format"))
                    {
                        throw new ArgumentException("Can't identify image format", e);
                    }
                    throw;
                }
            }
        }
        /// <summary>
        /// Gets the dimensions of an image.
        /// </summary>
        /// <param name="binaryReader">The binary reader of the image to get the dimensions of.</param>
        /// <returns>The dimensions of the specified image.</returns>
        /// <exception cref="ArgumentException">The image was of an unrecognized format.</exception>    
        private Size GetDimensions(BinaryReader binaryReader)
        {
            int maxMagicBytesLength = ImageFormatDecodersMagicBytes.Keys.OrderByDescending(x => x.Length).First().Length;

            byte[] magicBytes = new byte[maxMagicBytesLength];

            for (int i = 0; i < maxMagicBytesLength; i += 1)
            {
                magicBytes[i] = binaryReader.ReadByte();

                foreach (var kvPair in ImageFormatDecodersMagicBytes)
                {
                    if (magicBytes.StartsWith(kvPair.Key))
                    {
                        return kvPair.Value(binaryReader);
                    }
                }
            }

            throw new ArgumentException("Can't identify image format");
        }

        private static Size DecodeBitmap(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(16);
            int width = binaryReader.ReadInt32();
            int height = binaryReader.ReadInt32();
            return new Size(width, height);
        }
        private static Size DecodeGif(BinaryReader binaryReader)
        {
            int width = binaryReader.ReadInt16();
            int height = binaryReader.ReadInt16();
            return new Size(width, height);
        }
        private static Size DecodePng(BinaryReader binaryReader)
        {
            binaryReader.ReadBytes(8);
            int width = binaryReader.ReadLittleEndianInt32();
            int height = binaryReader.ReadLittleEndianInt32();
            return new Size(width, height);
        }
        private static Size DecodeJfif(BinaryReader binaryReader)
        {
            while (binaryReader.ReadByte() == 0xff)
            {
                byte marker = binaryReader.ReadByte();
                short chunkLength = binaryReader.ReadLittleEndianInt16();

                if (marker >= 0xc0 && marker <= 0xcf) // SOFn regions. All are the same for desiredWidth and desiredHeight
                {
                    binaryReader.ReadByte();

                    int height = binaryReader.ReadLittleEndianInt16();
                    int width = binaryReader.ReadLittleEndianInt16();
                    return new Size(width, height);
                }
                binaryReader.ReadBytes(chunkLength - 2);
            }

            throw new ArgumentException("Can't identify image format");
        }

        #endregion

        private BaseImage CheckImageSpecification<T>(Func<T, bool> checkFunction, BaseImage image)
            where T : BaseImage
        {
            T convertedImage = image as T;

            if (convertedImage != null)
            {
                var filteredImage = checkFunction(convertedImage);

                if (filteredImage)
                {
                    return image;
                }
            }
            foreach (var child in image.ChildImage)
            {
                var foundImage = CheckImageSpecification(checkFunction, child);

                if (foundImage != null && foundImage is T)
                {
                    return foundImage;
                }
            }
            return null;
        }
    }
}