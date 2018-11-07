using Horsesoft.Music.Horsify.Repositories;
using System.Threading.Tasks;

namespace Horsesoft.Music.Engine.Import
{
    public interface IHorsifyDbConnection
    {
        Task<bool> InitDb(string dbFilePath, string tableToCheck = "Song");
    }

    public class DbConnection : IHorsifyDbConnection
    {
        private IHorsifyDataRepo _horsifyDataRepo;

        public DbConnection()
        {
            _horsifyDataRepo = new HorsifyDataSqliteRepo();
        }

        public Task<bool> InitDb(string dbFilePath, string tableToCheck = "Song")
        {
            return _horsifyDataRepo.InitializeDatabase(dbFilePath, tableToCheck);
        }
    }

}
