using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveUI.Utils;
using Ninject.Modules;

namespace FitsArchiveUI.Views
{
    public class ViewsModule : NinjectModule
    {
        public override void Load()
        {
            var viewTypes =
                ReflectionUtils.GetAllTypesImplementingOpenGenericType(typeof(IView<>), this.GetType().Assembly).ToList();
            viewTypes.ForEach(t => Bind(t).To(t));
        }
    }
}
