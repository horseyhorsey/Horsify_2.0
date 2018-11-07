using System.Linq;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Sqlite.Context;

namespace Horsesoft.Music.Horsify.Repositories
{
    public interface IAllJoinedTableRepo
    {
        AllJoinedTable GetById(long id);
    }

    public class AllJoinedTableRepo : IAllJoinedTableRepo
    {
        private HorsifyContext _context;
        public AllJoinedTableRepo(HorsifyContext context)
        {
            _context = context;
        }

        public AllJoinedTable GetById(long id)
        {
            return _context.AllJoinedTables.FirstOrDefault(x => x.Id == id);
        }
    }
}
