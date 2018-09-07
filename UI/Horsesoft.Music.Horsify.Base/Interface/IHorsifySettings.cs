namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IHorsifySettings
    {
        string HorsifyArtworkPath { get; set; }
        string HorsifyPath { get; set; }
        string LogPath { get; set; }
        string PlaylistPath { get; set; }
        void Load();
        void Save();
    }
}
 