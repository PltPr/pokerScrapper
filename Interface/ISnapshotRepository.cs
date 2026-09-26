namespace api.Interface
{
    public interface ISnapshotRepository
    {
        string RecognizeValueWithTesseract(string filePath);
        string RecognizeSymbol(string filePath, string type);

    }
}