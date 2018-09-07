using System.ServiceModel;

namespace Horsesoft.Horsify.SongService.Services
{
    [ServiceContract]
    public interface IHorsifyTagService
    {
        [OperationContract]
        bool UpdatePlayedSong(long songId, int? rating = null);
    }
}
