using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace FitsArchiveUI.ViewModels
{
    public class ViewModelProvider : IViewModelProvider
    {
        private readonly IReadOnlyKernel _kernel;

        public ViewModelProvider(IReadOnlyKernel kernel)
        {
            _kernel = kernel;
        }

        public TViewModel Instantiate<TViewModel>() where TViewModel: IViewModel<TViewModel>
        {
            return _kernel.Get<TViewModel>();
        }
    }
}
