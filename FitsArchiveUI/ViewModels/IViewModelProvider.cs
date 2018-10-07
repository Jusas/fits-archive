using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveUI.ViewModels
{
    public interface IViewModelProvider
    {
        TViewModel Instantiate<TViewModel>() where TViewModel : IViewModel<TViewModel>;
    }
}
