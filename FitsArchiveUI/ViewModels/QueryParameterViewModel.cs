using System.Collections.Generic;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveUI.ViewModels
{
    public class QueryParameterViewModel : ViewModelBase<QueryParameterViewModel>
    {
        public enum QType
        {
            TextField,
            RaDec,
            Xy,
            Latitude,
            TimePeriod
        }
        
        public QType QueryType { get; set; }

        public QueryParameterViewModel(ILog log) : base(log)
        {
        }

        


    }
}