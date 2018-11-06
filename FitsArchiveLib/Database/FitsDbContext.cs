using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SQLite;

namespace FitsArchiveLib.Database
{
    internal class FitsDbContext : DataConnection
    {
        private static IDataProvider _dataProvider = new SQLiteDataProvider();

        public FitsDbContext(string connection) : base(_dataProvider, connection)
        {
        }
        
        public ITable<FitsTableRow> Files => GetTable<FitsTableRow>();
        public ITable<FitsHeaderIndexedRow> Headers => GetTable<FitsHeaderIndexedRow>();
        public ITable<PlateSolveRow> PlateSolves => GetTable<PlateSolveRow>();
    }
}
