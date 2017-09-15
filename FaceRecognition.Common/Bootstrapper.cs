using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FaceRecognition.Common.ConfigFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FaceRecognition.Common
{
    public partial class Bootstrapper
    {


        private IConfigurationRoot _config;

        public const double DoubleTolerance = 0.0001;
        public const string MaxDoubleFixedPoint = "0.###################################################################################################################################################################################################################################################################################################################################################";

        public static Bootstrapper Instance { get; private set; }

        private Bootstrapper()
        {
            var file = new DirectoryInfo(AppContext.BaseDirectory).GetFiles("*.config").First().FullName;
            var builder = new ConfigurationBuilder().AddConfigFile(file);

            _config = builder.Build();
        }
        private Bootstrapper(IConfigurationRoot configurationRoot)
        {
            _config = configurationRoot;
        }

        public static void Build(IConfigurationRoot configurationRoot)
        {
            Instance = new Bootstrapper(configurationRoot);
        }

        internal void SetConfiguration(IConfigurationRoot config) => _config = config;
        public string PythonDirectory => _config.GetAppSetting("pythonDirectory");
#if DEBUG
        public ParallelOptions MaxDegreeOfParalelism { get; } = new ParallelOptions() { MaxDegreeOfParallelism = 1 };
#else
        public ParallelOptions MaxDegreeOfParalelism { get; } = new ParallelOptions() { Environment.ProcessorCount};
#endif
        public string FileServerRoot => _config.GetAppSetting("fileServerRoot");
        public PythonConfiguration Python { get; } = new PythonConfiguration();
        public ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();


        public partial class PythonConfiguration
        {
            public string Hostname { get; } = "localhost";
            public RabbitMqExchanges Exchanges { get; } = new RabbitMqExchanges();
            public RabbitMqQueues Queues { get; } = new RabbitMqQueues();
            public RabbitMqRoutingKeys RoutingKeys { get; } = new RabbitMqRoutingKeys();
            public PythonEndpointMethods EndpointMethods { get; } = new PythonEndpointMethods();
        }
    }
}