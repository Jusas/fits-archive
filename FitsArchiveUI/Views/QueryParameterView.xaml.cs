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
    /// Interaction logic for QueryParameterView.xaml
    /// </summary>
    public partial class QueryParameterView : QueryParameterViewBase
    {
        public QueryParameterView() : base(null)
        {
            InitializeComponent();
        }

        public QueryParameterView(QueryParameterViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }
    }
}
