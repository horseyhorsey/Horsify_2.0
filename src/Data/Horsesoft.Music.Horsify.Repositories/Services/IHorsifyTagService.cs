namespace Horsesoft.Music.Horsify.Repositories.Services
{
    public interface IHorsifyTagService
    {
        bool UpdatePlayedSong(long songId, int? rating = null);
    }
}
