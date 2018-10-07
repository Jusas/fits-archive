using System;
using System.ComponentModel;
using FitsArchiveUI.ViewModels;

namespace FitsArchiveUI.Views
{
    public interface IView<TViewModel>
    {
        IViewModel<TViewModel> ViewModel { get; }
       
        event EventHandler OnViewLoaded;
    }
}