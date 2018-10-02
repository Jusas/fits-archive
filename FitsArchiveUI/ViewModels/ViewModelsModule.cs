using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveUI.Utils;
using Ninject.Modules;

namespace FitsArchiveUI.ViewModels
{
    public class ViewModelsModule : NinjectModule
    {
        public override void Load()
        {
            var viewModelTypes =
                ReflectionUtils.GetAllTypesImplementingOpenGenericType(typeof(IViewModel<>), this.GetType().Assembly).ToList();
            viewModelTypes.ForEach(t => Bind(t).To(t));
        }
    }
}
