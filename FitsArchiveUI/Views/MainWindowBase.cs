using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveUI.ViewModels;

namespace FitsArchiveUI.Views
{
    public class MainWindowBase : WindowBase<MainViewModel>
    {
        public MainWindowBase(IViewModel<MainViewModel> viewModel) : base(viewModel)
        {
        }
    }
}
