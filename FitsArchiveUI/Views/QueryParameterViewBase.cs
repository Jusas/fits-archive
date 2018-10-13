using FitsArchiveUI.ViewModels;

namespace FitsArchiveUI.Views
{
    /// <summary>
    /// A wrapper class because the VS Designer doesn't like inheriting from generic classes directly.
    /// </summary>
    public class QueryTabContentViewBase : UserControlBase<QueryTabContentViewModel>
    {
        public QueryTabContentViewBase(IViewModel<QueryTabContentViewModel> viewModel) : base(viewModel)
        {
        }
    }
}