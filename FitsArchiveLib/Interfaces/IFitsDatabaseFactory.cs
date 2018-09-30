namespace FitsArchiveLib.Interfaces
{
    public interface IFitsDatabaseFactory
    {
        IFitsDatabase CreateFitsDatabase(string databaseFilename, bool createIfNotExist);
    }
}
