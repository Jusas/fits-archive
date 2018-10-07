using System;
using System.Windows;
using System.Windows.Controls;
using FitsArchiveUI.ViewModels;

namespace FitsArchiveUI.Views
{
    public class TabItemBase<TViewModel> : TabItem, ITabItem<TViewModel>
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


        protected TabItemBase(IViewModel<TViewModel> viewModel)
        {
            ViewModel = viewModel;
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