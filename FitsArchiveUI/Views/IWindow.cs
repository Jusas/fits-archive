using System;
using System.ComponentModel;

namespace FitsArchiveUI.Views
{
    public interface IWindow<TViewModel> : IView<TViewModel>
    {
        bool WasClosed { get; }
        bool? DialogResult { get; set; }

        event EventHandler<CancelEventArgs> OnViewClosing;
        event EventHandler OnViewClosed;

        void Show();
        void Close();
        bool ShowModal();
    }
}