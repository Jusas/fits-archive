using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using FitsArchiveLib;
using FitsArchiveUI.ViewModels;
using FitsArchiveUI.Views;

namespace FitsArchiveUI
{
    public static class Bootstrap
    {
        public static IReadOnlyKernel Kernel { get; private set; }

        public static void SetupDI()
        {            
            var kcfg = new KernelConfiguration(
                new FitsArchiveLibModule(), 
                new ViewModelsModule(), 
                new ViewsModule()
            );
            Kernel = kcfg.BuildReadonlyKernel();

        }
    }
}
