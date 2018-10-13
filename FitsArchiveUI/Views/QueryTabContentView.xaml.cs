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
    /// Interaction logic for QueryTabContentView.xaml
    /// </summary>
    public partial class QueryTabContentView : QueryTabContentViewBase
    {
        public QueryTabContentView() : base(null)
        {
            InitializeComponent();
        }

        public QueryTabContentView(QueryTabContentViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }
    }
}
