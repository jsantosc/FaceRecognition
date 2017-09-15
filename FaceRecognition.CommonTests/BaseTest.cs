using System;
using System.IO;
using System.Linq;
using FaceRecognition.Common;
using FaceRecognition.Common.ConfigFiles;
using Microsoft.Extensions.Configuration;

namespace FaceRecognition.CommonTests
{
    public abstract class BaseTest
    {
        protected BaseTest()
        {
            var file = new DirectoryInfo(AppContext.BaseDirectory).GetFiles("*.config").First().FullName;
            var builder = new ConfigurationBuilder().AddConfigFile(file);

            var config = builder.Build();
            Bootstrapper.Build(config);
            FileServer.Build(config);
        }
    }
}