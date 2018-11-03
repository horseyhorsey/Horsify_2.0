using System.ServiceModel;

namespace Horsesoft.Horsify.SongService.Services
{
    
    public interface IHorsifyTagService
    {
        
        bool UpdatePlayedSong(long songId, int? rating = null);
    }
}
