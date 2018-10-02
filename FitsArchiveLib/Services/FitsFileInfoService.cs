using FitsArchiveLib.Entities;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveLib.Services
{
    public class FitsFileInfoService : IFitsFileInfoService
    {
        public IFitsFileInfo GetFitsFileInfo(string filePath)
        {
            return new FitsFileInfo(filePath);
        }
    }
}
