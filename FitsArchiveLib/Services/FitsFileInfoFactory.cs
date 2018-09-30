using FitsArchiveLib.Entities;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveLib.Services
{
    public class FitsFileInfoFactory : IFitsFileInfoFactory
    {
        public IFitsFileInfo CreateFitsFileInfo(string filePath)
        {
            return new FitsFileInfo(filePath);
        }
    }
}
