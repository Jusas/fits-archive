using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveUI.ViewModels
{
    /// <summary>
    /// Pairs with QueryTabContentView.
    /// </summary>
    public class QueryTabContentViewModel : ViewModelBase<QueryTabContentViewModel>
    {
        public class TestDataRow
        {
            public string Filename { get; set; }
            public string ObservationDate { get; set; }
            public string FrameKind { get; set; }
            public string Filter { get; set; }
            public string Ra { get; set; }
            public string Dec { get; set; }
            public string FocalLen { get; set; }
            public string Object { get; set; }
        }

        public ICommand AddQueryParamCommand => new CommandHandler(AddQueryParam);
        public ICommand RemoveQueryParamCommand => new CommandHandler(RemoveQueryParam);

        public List<TestDataRow> QueryResults { get; } = new List<TestDataRow>()
        {
            new TestDataRow()
            {
                Filename = @"C:\tmp\test.fits",
                ObservationDate = "2018-08-17T01:46:01.538Z",
                FrameKind = "Light",
                Filter = "Lum",
                Ra = "2 27 12.41",
                Dec = "33 34 00.63",
                FocalLen = "1480",
                Object = "Unknown"
            }
        };

        public ObservableCollection<QueryParameterViewModel> QueryParams { get; set; } = 
            new ObservableCollection<QueryParameterViewModel>();

        private IViewModelProvider _viewModelProvider;

        public QueryTabContentViewModel(ILog log, IViewModelProvider viewModelProvider) : base(log)
        {
            _viewModelProvider = viewModelProvider;
            var defaultQueryParam = viewModelProvider.Instantiate<QueryParameterViewModel>();
            QueryParams.Add(defaultQueryParam);
        }

        private void AddQueryParam()
        {
            QueryParams.Add(_viewModelProvider.Instantiate<QueryParameterViewModel>());
        }

        private void RemoveQueryParam(object index)
        {
            int idx = (int) index;
            QueryParams.RemoveAt(idx);
        }
    }
}
