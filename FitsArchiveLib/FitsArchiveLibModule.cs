using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Interfaces;
using FitsArchiveLib.Services;
using Ninject.Modules;

namespace FitsArchiveLib
{
    public class FitsArchiveLibModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogService>().To<LogService>().InSingletonScope();
            Bind<IFitsFileInfoService>().To<FitsFileInfoService>().InSingletonScope();
        }
    }
}
