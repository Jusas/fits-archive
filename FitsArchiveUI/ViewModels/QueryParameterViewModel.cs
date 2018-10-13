using System.Collections.Generic;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveUI.ViewModels
{
    public enum QType
    {
        TextField,
        RaDec,
        Xy,
        Latitude,
        TimePeriod
    }

    /// <summary>
    /// Pairs with QueryParameterView, which is a visual representation of a query parameter.
    /// This in turn is a ViewModel representation of a query parameter for FitsDatabase,
    /// which will be used by the QueryTabContentViewModel when finally running a query.
    /// </summary>
    public class QueryParameterViewModel : ViewModelBase<QueryParameterViewModel>
    {
        public QType QueryType { get; set; } = QType.TextField;

        public QueryParameterViewModel(ILog log) : base(log)
        {
        }

        


    }
}