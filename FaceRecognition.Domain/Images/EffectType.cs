namespace FaceRecognition.Domain.Images
{
    public enum EffectType
    {
        None = 0,
        ConvertedToGray = 1,
        Blurred = 2,
        FlippedHorizontal = 4,
        FlippedVertical = 8,
        FlippedBoth = 16,
        Rotated = 32,
        ElasticDistorsionated = 64,
        CroppedAndResized = 128,
    }
}