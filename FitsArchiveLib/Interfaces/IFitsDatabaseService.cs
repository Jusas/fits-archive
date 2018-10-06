namespace FitsArchiveLib.Interfaces
{

    public enum FitsDatabaseLogging
    {
        LogBesideDatabase,
        LogToCommonLog
    }

    public interface IFitsDatabaseService
    {
        void SetLoggingOptions(FitsDatabaseLogging opts, string commonLogName = null);
        IFitsDatabase GetFitsDatabase(string databaseFilename, bool createIfNotExist);
    }
}
