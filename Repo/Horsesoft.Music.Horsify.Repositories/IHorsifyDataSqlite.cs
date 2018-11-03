using Horsesoft.Music.Data.Sqlite.Context;

namespace Horsesoft.Music.Horsify.Repositories
{
    public class HorsifyDataSqliteRepo : HorsifyDataRepo
    {
        /// <summary>
        /// Initializes a new instance with <see cref="HorsifyContext"/>
        /// </summary>
        public HorsifyDataSqliteRepo()
        {            
            _context = new HorsifyContext();            
        }
    }
}
