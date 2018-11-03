using Horsesoft.Horsify.SongService.Services;
using Horsesoft.Music.Data.Model;

namespace Horsesoft.Horsify.SongService
{
    public interface IHorsifySongService : IHorsifyFileService, IHorsifySearchService, IHorsifyTagService, IHorsifyFilterService, IHorsifyPlaylistService
    {
        AllJoinedTable GetSongById(int value);

        int GetTotals(string type = "Song");
    }
}
