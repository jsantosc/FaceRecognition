using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;

namespace FaceRecognition.CommonTests
{
    public class TestImages : IDisposable
    {
        private readonly List<string> _extractedDirectories;
        private readonly Dictionary<string, Vector2[]> _pointsPerImage = new Dictionary<string, Vector2[]>();

        public string DestinationPath { get; }
        public IEnumerable<string> ExtractedDirectories => _extractedDirectories.ToArray();

        public TestImages(Assembly assembly, string resourceKey, string destinationPath)
        {
            DestinationPath = destinationPath;
            using (var str = assembly.GetManifestResourceStream(resourceKey))
            {
                if (str == null)
                {
                    throw new Exception($"The resource {resourceKey} does not exist in assembly {assembly.FullName}");
                }
                ZipArchive zipArchive = new ZipArchive(str);

                _extractedDirectories = zipArchive.Entries.Where(e => e.Length == 0).Select(e => e.FullName.TrimEnd('/')).ToList();
                zipArchive.ExtractToDirectory(DestinationPath, true);
            }
        }
        public TestImages(Assembly assembly, string resourceKey, string pointsResourceFile, string destinationPath)
        {
            DestinationPath = destinationPath;
            using (var str = assembly.GetManifestResourceStream(resourceKey))
            {
                if (str == null)
                {
                    throw new Exception($"The resource {resourceKey} does not exist in assembly {assembly.FullName}");
                }
                ZipArchive zipArchive = new ZipArchive(str);

                _extractedDirectories = zipArchive.Entries.Where(e => e.Length == 0).Select(e => e.FullName.TrimEnd('/')).ToList();
                zipArchive.ExtractToDirectory(DestinationPath, true);
            }
            using (var str = assembly.GetManifestResourceStream(pointsResourceFile))
            {
                if (str == null)
                {
                    throw new Exception($"The resource {resourceKey} does not exist in assembly {assembly.FullName}");
                }
                using (var reader = new StreamReader(str))
                {
                    var json = reader.ReadToEnd();
                    _pointsPerImage = JsonConvert.DeserializeObject<Dictionary<string, Vector2[]>>(json);
                }
            }
        }

        public Vector2[] GetImagePoints(string imageName)
        {
            string name = imageName.Replace(".jpg", string.Empty).Replace(".png", string.Empty);
            if (_pointsPerImage.ContainsKey(name))
            {
                return _pointsPerImage[name];
            }
            return new Vector2[0];
        }

        public IEnumerable<string> GetImageDirectories()
        {
            return _extractedDirectories.Select(i => Path.Combine(DestinationPath, i));
        }

        public void Dispose()
        {
            try
            {
                var imgDirectories = GetImageDirectories();
                foreach (var dir in imgDirectories)
                {
                    if (Directory.Exists(dir))
                    {
                        Directory.Delete(dir, true);
                    }
                }
            }
            catch (Exception)
            {
                //Don't throw exception if we cannot cleanup files
            }
        }
    }
}