using System;
using System.ComponentModel;
using FitsArchiveUI.ViewModels;

namespace FitsArchiveUI.Views
{
    public interface IUserControl<TViewModel> : IView<TViewModel>
    {
        event EventHandler OnUnloaded;
    }
}