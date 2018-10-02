﻿using FitsArchiveUI.Views;

namespace FitsArchiveUI.ViewModels
{
    public interface IViewModel<TViewModel>
    {
        TViewModel ViewModel { get; }
        IView<TViewModel> OwnerView { get; set; }
        void Initialize();
    }
}
