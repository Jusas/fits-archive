using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib
{
    public class FitsFileFactory : IFitsFileFactory
    {
        public IFitsFile CreateFitsFile(string filePath)
        {
            return new FitsFile(filePath);
        }
    }
}
