using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveUI.ViewModels
{
    public class MainViewModel : ViewModelBase<MainViewModel>
    {
        public string Name { get; }

        public MainViewModel(ILogService logService) : base(logService)
        {
            Name = "Hello";
        }



    }
}
