using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveUI.ViewModels
{
    /// <summary>
    /// Does not pair with a specific view class - acts as a model for a TabControl item, which has a header
    /// (TabName) and content (TabContentViewModel).
    /// </summary>
    public class QueryTabContainerViewModel : ViewModelBase<QueryTabContainerViewModel>
    {
        public string TabName { get; set; }
        public QueryTabContentViewModel TabContentViewModel { get; private set; }

        private readonly IViewModelProvider _viewModelProvider;

        public QueryTabContainerViewModel(ILog log, IViewModelProvider viewModelProvider) : base(log)
        {
            TabName = "Query";
            _viewModelProvider = viewModelProvider;
            TabContentViewModel = _viewModelProvider.Instantiate<QueryTabContentViewModel>();
        }
    }
}
