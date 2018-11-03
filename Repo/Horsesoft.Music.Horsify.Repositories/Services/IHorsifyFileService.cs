using Horsesoft.Music.Data.Model;

namespace Horsesoft.Music.Horsify.Repositories.Services
{
    public interface IHorsifyFileService
    {
        int Add(File file);

        File GetById(long value);
    }
}
