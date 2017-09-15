using System;
using System.IO;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Reflection;

namespace FaceRecognition.CommonTests
{
    public static class TestHelpers
    {
        public static void SetPrivateProperty<T, TP>(this T obj, Expression<Func<T, TP>> action, TP value)
            where T : class
        {
            var expression = (MemberExpression)action.Body;
            var name = expression.Member.Name;
            var pInfo = typeof(T).GetRuntimeProperty(name);

            pInfo.SetValue(obj, value);
        }
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                if (file.Name == "")
                {// Assuming Empty for Directory
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }
                file.ExtractToFile(completeFileName, true);
            }
        }
    }
}