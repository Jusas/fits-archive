using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using FitsArchiveUI.ViewModels;

namespace FitsArchiveUI.Views
{
    public class UserControlBase<TViewModel> : UserControl, IUserControl<TViewModel>
    {
        private IViewModel<TViewModel> _viewModel;
        public IViewModel<TViewModel> ViewModel
        {
            protected set
            {
                _viewModel = value;
                _viewModel.OwnerView = this;
                _viewModel.Initialize();
                DataContext = value.ViewModel;
            }
            get { return _viewModel; }
        }

        public event EventHandler OnViewLoaded;
        public event EventHandler OnUnloaded;


        protected UserControlBase(IViewModel<TViewModel> viewModel)
        {
            // ViewModel = viewModel;
            InitializeInterfaceBindings();
        }

        protected void InitializeInterfaceBindings()
        {
            Loaded += OnLoadedHandler;
            Unloaded += OnUnloadedHandler;
        }
        
        private void OnLoadedHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            OnViewLoaded?.Invoke(sender, null);
        }

        private void OnUnloadedHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            OnUnloaded?.Invoke(sender, null);
        }

    }
}