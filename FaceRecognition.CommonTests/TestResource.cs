using System;
using System.IO;
using System.Reflection;

namespace FaceRecognition.CommonTests
{
    public class TestResource : IDisposable
    {
        public string ResourcePath { get; }

        public TestResource(Assembly assembly, string resourceKey, string destinationPath)
        {
            using (var str = assembly.GetManifestResourceStream(resourceKey))
            {
                string finalPath = destinationPath;

                using (var finalStr = File.Create(finalPath))
                {
                    str.CopyTo(finalStr);
                }
                ResourcePath = finalPath;
            }
        }
        public TestResource(Assembly assembly, string resourceKey)
            : this(assembly, resourceKey, Path.GetTempFileName())
        {
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(ResourcePath))
                {
                    File.Delete(ResourcePath);
                }
            }
            catch (Exception)
            {
                //Don't throw exception if we cannot cleanup files
            }
        }
    }
}