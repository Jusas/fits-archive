using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FitsArchiveLib.Entities;
using FitsArchiveLib.Interfaces;
using Microsoft.Win32;

namespace FitsArchiveUI.ViewModels
{
    public class MainViewModel : ViewModelBase<MainViewModel>
    {
        public string Name { get; }

        public ICommand OpenDbCommand => new CommandHandler(OpenSelectDatabaseDialog);
        public ICommand NewDbCommand => new CommandHandler(OpenNewDatabaseDialog);
        public ICommand NewQueryTabCommand => new CommandHandler(OpenNewQueryTab);

        private IFitsDatabase _fitsDatabase;

        public IFitsDatabase FitsDatabase
        {
            get { return _fitsDatabase; }
            private set
            {
                SetNotifyingProperty(nameof(FitsDatabase), ref _fitsDatabase, value);
                SetNotifyingProperty(nameof(IsDbOpen), value != null);
            }
        }

        public bool IsDbOpen { get; private set; }

        //private QueryTabContainerViewModel _activeQueryTab;
        //public QueryTabContainerViewModel ActiveQueryTab
        //{
        //    get => _activeQueryTab;
        //    private set => SetNotifyingProperty(nameof(ActiveQueryTab), ref _activeQueryTab, value);
        //}

        public ObservableCollection<QueryTabContainerViewModel> QueryTabs { get; set; } = new ObservableCollection<QueryTabContainerViewModel>();
        
        private IFitsDatabaseService _fitsDatabaseService;
        private IViewModelProvider _viewModelProvider;

        public MainViewModel(ILog log,
            IFitsDatabaseService fitsDatabaseService,
            IViewModelProvider viewModelProvider) : base(log)
        {
            _fitsDatabaseService = fitsDatabaseService;
            _viewModelProvider = viewModelProvider;
            // Name = "Hello";
            // QueryTabs.Add(_viewModelProvider.Instantiate<QueryTabContainerViewModel>());
        }

        private void OpenSelectDatabaseDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog {Filter = "FITS archives (*.fitsdb)|*.fitsdb"};
            if (dlg.ShowDialog((Window) this.OwnerView) == true)
            {
                try
                {
                    FitsDatabase = _fitsDatabaseService.GetFitsDatabase(dlg.FileName, false);
                    this.QueryTabs.Clear();
                    this.QueryTabs.Add(_viewModelProvider.Instantiate<QueryTabContainerViewModel>());
                    // ActiveQueryTab = this.QueryTabs[0];
                }
                catch (Exception e)
                {
                    Log.Write(LogEventCategory.Error, "Opening of FITS archive failed", e);
                }
            }
        }

        private void OpenNewDatabaseDialog()
        {
            SaveFileDialog dlg = new SaveFileDialog {Filter = "FITS archives (*.fitsdb)|*.fitsdb"};
            if (dlg.ShowDialog((Window) this.OwnerView) == true)
            {
                try
                {
                    FitsDatabase = _fitsDatabaseService.GetFitsDatabase(dlg.FileName, true);
                    this.QueryTabs.Clear();
                    this.QueryTabs.Add(_viewModelProvider.Instantiate<QueryTabContainerViewModel>());
                    // ActiveQueryTab = this.QueryTabs[0];
                }
                catch (Exception e)
                {
                    Log.Write(LogEventCategory.Error, "Creation of FITS archive failed", e);
                }
            }
        }

        private void OpenNewQueryTab()
        {
            QueryTabs.Add(_viewModelProvider.Instantiate<QueryTabContainerViewModel>());
            // todo set active
        }


    }
}
