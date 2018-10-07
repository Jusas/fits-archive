using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveUI.ViewModels
{
    public class QueryTabItemViewModel : ViewModelBase<QueryTabItemViewModel>
    {
        public string TabName { get; set; }

        public QueryTabItemViewModel(ILog log) : base(log)
        {
            TabName = "Query";
        }
    }
}
