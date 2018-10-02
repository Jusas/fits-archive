namespace FitsArchiveLib.Interfaces
{
    public interface IFitsFileInfoService
    {
        IFitsFileInfo GetFitsFileInfo(string filePath);
    }
}
