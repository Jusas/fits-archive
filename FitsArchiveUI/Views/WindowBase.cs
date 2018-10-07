using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FitsArchiveUI.ViewModels;

namespace FitsArchiveUI.Views
{
    public abstract class WindowBase<TViewModel> : Window, IWindow<TViewModel>
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

        public bool WasClosed { get; }

        public event EventHandler OnViewLoaded;
        public event EventHandler<CancelEventArgs> OnViewClosing;
        public event EventHandler OnViewClosed;

        protected WindowBase(IViewModel<TViewModel> viewModel)
        {
            // ViewModel = viewModel;
            InitializeInterfaceBindings();
        }

        protected void InitializeInterfaceBindings()
        {
            Loaded += OnLoadedHandler;
            Closing += OnClosingHandler;
            Closed += OnViewClosedHandler;
        }

        public bool ShowModal()
        {
            var result = ShowDialog();
            return result ?? false;
        }


        private void OnClosingHandler(object sender, CancelEventArgs cancelEventArgs)
        {
            OnViewClosing?.Invoke(sender, cancelEventArgs);
        }

        private void OnViewClosedHandler(object sender, EventArgs args)
        {
            OnViewClosed?.Invoke(sender, args);
        }

        private void OnLoadedHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            OnViewLoaded?.Invoke(sender, null);
        }
    }
}
