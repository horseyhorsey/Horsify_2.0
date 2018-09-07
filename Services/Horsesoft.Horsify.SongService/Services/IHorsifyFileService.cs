using System.ServiceModel;
using Horsesoft.Music.Data.Model;

namespace Horsesoft.Horsify.SongService
{
    [ServiceContract]
    public interface IHorsifyFileService
    {
        [OperationContract]
        int Add(File file);

        [OperationContract]
        File GetById(long value);
    }
}
