namespace Horsesoft.Music.Data.Model.Horsify
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
 