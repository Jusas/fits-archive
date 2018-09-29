using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib
{
    public interface IFitsFileFactory
    {
        IFitsFile CreateFitsFile(string filePath);
    }
}
