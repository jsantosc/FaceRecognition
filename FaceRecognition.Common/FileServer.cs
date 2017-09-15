using System;
using System.IO;
using FaceRecognition.Common.ConfigFiles;
using Microsoft.Extensions.Configuration;

namespace FaceRecognition.Common
{
    public class FileServer : IFileServer
    {
        public string RootDirectory { get; private set; }
        public string ImageDirectory { get; private set; }
        public string CaffeModelsDirectory { get; private set; }
        public string TemporaryDirectory { get; private set; }
        public string TrainingSetOriginalFilesDirectory { get; private set; }

        public static IFileServer Instance { get; private set; }

        private FileServer(IConfigurationRoot configurationRoot)
        {
            LoadDirectories(configurationRoot);
        }

        public static void Build(IConfigurationRoot configurationRoot)
        {
            Instance = new FileServer(configurationRoot);
        }
        public string GetTemporalFolder()
        {
            string folderName = Path.Combine(TemporaryDirectory, Guid.NewGuid().ToString());

            Directory.CreateDirectory(folderName);
            return folderName;
        }

        private void LoadDirectories(IConfigurationRoot config)
        {
            RootDirectory = Bootstrapper.Instance.FileServerRoot;
            ImageDirectory = Path.Combine(RootDirectory, config.GetAppSetting("imageFolderName"));
            CaffeModelsDirectory = Path.Combine(RootDirectory, config.GetAppSetting("caffeModelsFolderName"));
            TemporaryDirectory = Path.Combine(RootDirectory, config.GetAppSetting("temporaryFolderName"));
            TrainingSetOriginalFilesDirectory = Path.Combine(RootDirectory, config.GetAppSetting("tSetOriginalFilesFolderName"));
        }
    }
}