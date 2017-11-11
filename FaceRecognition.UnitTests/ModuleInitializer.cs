using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FaceRecognition.Common;
using FaceRecognition.Common.ConfigFiles;
using Microsoft.Extensions.Configuration;

namespace FaceRecognition.UnitTests
{
    public static class ModuleInitializer
    {
        public static void Initialize()
        {
            InitBootstrapperAndFileService();
        }

        private static void InitBootstrapperAndFileService()
        {
            var file = new DirectoryInfo(AppContext.BaseDirectory).GetFiles("*UnitTests.dll.config").First().FullName;
            var builder = new ConfigurationBuilder().AddConfigFile(file);

            var config = builder.Build();
            Bootstrapper.Build(config);
            FileServer.Build(config);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            DirectoryInfo dInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var file = dInfo.GetFiles().SingleOrDefault(f => f.Name == $"{args.Name.Substring(0, args.Name.IndexOf(","))}.dll");
            if (file != null)
            {
                return Assembly.LoadFile(file.FullName);
            }
            return null;
        }
    }
}