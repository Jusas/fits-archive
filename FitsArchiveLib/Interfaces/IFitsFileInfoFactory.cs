namespace FitsArchiveLib.Interfaces
{
    public interface IFitsFileInfoFactory
    {
        IFitsFileInfo CreateFitsFileInfo(string filePath);
    }
}
