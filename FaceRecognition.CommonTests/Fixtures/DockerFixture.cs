using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FaceRecognition.CommonTests.Fixtures
{
    public class DockerFixture : IDisposable
    {
        public const string ServicePath = "Docker";
        public const string Tag = "Tests";
        public const string ServiceName = "FaceRecognition";
#if DEBUG
        public const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif
        public DockerFixture()
        {
            StartContainers();
        }

        public void Dispose()
        {
            StopContainers();
        }

        private void StartContainers()
        {
            // First build the Docker container image
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments = GetArguments(),
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                RedirectStandardOutput = true,
            };
            AddEnvironmentVariables(processStartInfo);

            var process = new Process { StartInfo = processStartInfo };

            process.OutputDataReceived += (sender, args) => Console.WriteLine("DOCKER OUTPUT: {0}", args.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception($"There was an error launching docker: {process.ExitCode}");
            }

            // Now start the docker containers
            processStartInfo = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments =
                    $"-f {ServicePath}/docker-compose.yml -p {ServiceName} up -d"
            };
            AddEnvironmentVariables(processStartInfo);

            process = Process.Start(processStartInfo);

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception($"There was an error launching docker: {process.ExitCode}");
            }
            
            //Waiting to warming up the different services in the containers
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
        private void StopContainers()
        {
            // Run docker-compose down to stop the containers
            // Note that "--rmi local" deletes the images as well to keep the machine clean
            // But it does so by deleting all untagged images, which may not be desired in all cases
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments =
                    $"-f {ServicePath}/docker-compose.yml -p {ServiceName} down --rmi local"
            };
            AddEnvironmentVariables(processStartInfo);

            var process = Process.Start(processStartInfo);

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception($"There was an error launching docker: {process.ExitCode}");
            }
        }

        private void AddEnvironmentVariables(ProcessStartInfo processStartInfo)
        {
            processStartInfo.Environment["TAG"] = Tag;
            processStartInfo.Environment["CONFIGURATION"] = Configuration;
            processStartInfo.Environment["COMPUTERNAME"] = Environment.MachineName;
        }
        private string GetArguments()
        {
            if (File.Exists(Path.Combine(ServicePath, $"docker-compose.{Configuration}.yml")))
            {
                return $"-f {ServicePath}/docker-compose.yml {ServicePath}/docker-compose.{Configuration}.yml build";
            }
            return $"-f {ServicePath}/docker-compose.yml build";
        }
    }
}