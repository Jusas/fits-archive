using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using FitsArchiveLib;
using FitsArchiveLib.Interfaces;
using FitsArchiveUI.ViewModels;
using FitsArchiveUI.Views;

namespace FitsArchiveUI
{
    public static class Bootstrap
    {
        public static IReadOnlyKernel Kernel { get; private set; }
        public static readonly string CommonLogName = "*";

        public static void Setup()
        {            
            var kcfg = new KernelConfiguration(
                new FitsArchiveLibModule(), 
                new ViewModelsModule(), 
                new ViewsModule()
            );
            PreKernelBuild(kcfg);
            Kernel = kcfg.BuildReadonlyKernel();
            PostKernelBuild();
        }

        private static void PreKernelBuild(KernelConfiguration kcfg)
        {
            var path = Path.GetDirectoryName(typeof(Bootstrap).Assembly.Location);
            var logFile = Path.Combine(path, "fitsarchive.log");

            // Default ILog binding is the common log.
            kcfg.Bind<ILog>().ToMethod((context) =>
            {
                var ls = context.Kernel.Get<ILogService>();
                return ls.HasLog(CommonLogName) ? ls.GetLog(CommonLogName) : ls.InitializeLog(CommonLogName, logFile);
            });
            
            // Bind ViewModelProvider
            kcfg.Bind<IViewModelProvider>().ToMethod(context => new ViewModelProvider(Kernel)).InSingletonScope();
        }

        private static void PostKernelBuild()
        {
            var dbService = Kernel.Get<IFitsDatabaseService>();
            dbService.SetLoggingOptions(FitsDatabaseLogging.LogToCommonLog, CommonLogName);
        }
    }
}
