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

        public IFitsDatabase FitsDatabase { get; private set; }

        public ObservableCollection<QueryTabItemViewModel> QueryTabs { get; set; } = new ObservableCollection<QueryTabItemViewModel>();
        
        private IFitsDatabaseService _fitsDatabaseService;
        private IViewModelProvider _viewModelProvider;

        public MainViewModel(ILog log,
            IFitsDatabaseService fitsDatabaseService,
            IViewModelProvider viewModelProvider) : base(log)
        {
            _fitsDatabaseService = fitsDatabaseService;
            _viewModelProvider = viewModelProvider;
            Name = "Hello";
            QueryTabs.Add(_viewModelProvider.Instantiate<QueryTabItemViewModel>());
        }

        private void OpenSelectDatabaseDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "FITS archives (*.fitsdb)|*.fitsdb";
            if (dlg.ShowDialog((Window) this.OwnerView) == true)
            {
                try
                {
                    FitsDatabase = _fitsDatabaseService.GetFitsDatabase(dlg.FileName, false);
                }
                catch (Exception e)
                {
                    Log.Write(LogEventCategory.Error, "Opening of FITS archive failed", e);
                }
            }
        }

        private void OpenNewDatabaseDialog()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "FITS archives (*.fitsdb)|*.fitsdb";
            if (dlg.ShowDialog((Window) this.OwnerView) == true)
            {
                try
                {
                    FitsDatabase = _fitsDatabaseService.GetFitsDatabase(dlg.FileName, true);
                }
                catch (Exception e)
                {
                    Log.Write(LogEventCategory.Error, "Creation of FITS archive failed", e);
                }
            }
        }


    }
}
