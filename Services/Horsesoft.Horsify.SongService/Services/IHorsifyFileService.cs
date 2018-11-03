using System.ServiceModel;
using Horsesoft.Music.Data.Model;

namespace Horsesoft.Horsify.SongService
{
    public interface IHorsifyFileService
    {
        int Add(File file);

        File GetById(long value);
    }
}
