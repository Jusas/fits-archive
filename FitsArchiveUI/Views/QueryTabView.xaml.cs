using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FitsArchiveUI.ViewModels;

namespace FitsArchiveUI.Views
{
    /// <summary>
    /// Interaction logic for QueryTabView.xaml
    /// </summary>
    public partial class QueryTabView : UserControlBase<QueryTabViewModel>
    {
        public QueryTabView() : base(new QueryTabViewModel(null))
        {
            InitializeComponent();
        }

        public QueryTabView(QueryTabViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
