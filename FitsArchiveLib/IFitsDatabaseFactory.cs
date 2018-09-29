using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib
{
    public interface IFitsDatabaseFactory
    {
        IFitsDatabase CreateFitsDatabase(string databaseFilename, bool createIfNotExist);
    }
}
