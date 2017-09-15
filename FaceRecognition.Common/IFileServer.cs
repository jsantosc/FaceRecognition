namespace FaceRecognition.Common
{
    public interface IFileServer
    {
        string RootDirectory { get; }
        string ImageDirectory { get; }
        string CaffeModelsDirectory { get; }
        string TemporaryDirectory { get; }
        string TrainingSetOriginalFilesDirectory { get; }

        string GetTemporalFolder();
    }
}