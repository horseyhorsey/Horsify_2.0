using System;
using Horsesoft.Music.Horsify.Repositories;

namespace Horsesoft.Music.Horsify.RepositoryTests
{
    /// <summary>
    /// Just before the first tests in The Tests is run, xUnit.net will create an instance of SqliteFixture. For each test, it will create a new instance of MYTests, and pass the shared instance of DatabaseFixture to the constructor. 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class SqliteFixture : IDisposable
    {
        protected internal IHorsifyDataRepo _HorsifyDataSqliteRepo;

        public SqliteFixture()
        {
            _HorsifyDataSqliteRepo = new HorsifyDataSqliteRepo();
        }

        public void Dispose()
        {
            _HorsifyDataSqliteRepo = null;
        }
    }
}
