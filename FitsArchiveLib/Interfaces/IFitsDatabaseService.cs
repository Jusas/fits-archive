namespace FitsArchiveLib.Interfaces
{
    public interface IFitsDatabaseService
    {
        IFitsDatabase GetFitsDatabase(string databaseFilename, bool createIfNotExist);
    }
}
