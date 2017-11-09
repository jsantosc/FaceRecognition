using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace FaceRecognition.CommonTests.Fixtures
{
    public class DockerFixture : IDisposable
    {
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
            var dockerPath = DetermineDockerDirectory();
            ProcessStartInfo CreateProcessStartInfo(string arguments)
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "docker-compose",
                    Arguments = arguments,
                    WorkingDirectory = dockerPath,
                    RedirectStandardOutput = true,
                };
                AddEnvironmentVariables(processStartInfo);
                return processStartInfo;
            }
            // First build the Docker container image
            var buildInfo = CreateProcessStartInfo(GetBuildArguments(dockerPath));
            StartAndLogProcess(buildInfo);

            // Now start the docker containers
            var startInfo = CreateProcessStartInfo($"-f docker-compose.yml -p {ServiceName} up -d");
            StartAndLogProcess(startInfo);

            //Waiting to warming up the different services in the containers
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }
        private void StopContainers()
        {
            // Run docker-compose down to stop the containers
            // Note that "--rmi local" deletes the images as well to keep the machine clean
            // But it does so by deleting all untagged images, which may not be desired in all cases
            var dockerPath = DetermineDockerDirectory();
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments = $"-f docker-compose.yml -p {ServiceName} down --rmi local",
                WorkingDirectory = dockerPath
            };
            AddEnvironmentVariables(processStartInfo);

            var process = Process.Start(processStartInfo);

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception($"There was an error launching docker: {process.ExitCode}");
            }
        }
        private void StartAndLogProcess(ProcessStartInfo processStartInfo)
        {
            var process = new Process {StartInfo = processStartInfo};

            process.OutputDataReceived += (sender, args) => Console.WriteLine("DOCKER OUTPUT: {0}", args.Data);
            process.Start();
            process.BeginOutputReadLine();
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
        private string GetBuildArguments(string workingDirectory)
        {
            if (File.Exists(Path.Combine(workingDirectory, $"docker-compose.{Configuration}.yml")))
            {
                return $"-f docker-compose.yml docker-compose.{Configuration}.yml build";
            }
            return "-f docker-compose.yml build";
        }
        private string DetermineDockerDirectory()
        {
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (directory.Parent != null)
            {
                if (directory.GetFiles("*.sln").Any())
                {
                    //Solution directory, we can go to docker path
                    return Path.Combine(directory.FullName, "Docker");
                }
                directory = directory.Parent;
            }
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}