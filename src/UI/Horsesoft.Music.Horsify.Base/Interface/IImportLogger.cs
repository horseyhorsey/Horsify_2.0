namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IImportLogger
    {
        void Log(string message);
        void Reset();
        void Save();
    }
}
