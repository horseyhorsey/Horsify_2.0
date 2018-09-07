using Horsesoft.Horsify.SongService.Services;
using Horsesoft.Music.Data.Model;
using System.ServiceModel;

namespace Horsesoft.Horsify.SongService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IHorsifySongService : IHorsifyFileService, IHorsifySearchService, IHorsifyTagService, IHorsifyFilterService, IHorsifyPlaylistService
    {
        [OperationContract]
        AllJoinedTable GetSongById(int value);

        [OperationContract]
        int GetTotals(string type = "Song");
    }
}
