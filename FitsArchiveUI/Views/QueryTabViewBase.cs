using FitsArchiveUI.ViewModels;

namespace FitsArchiveUI.Views
{
    /// <summary>
    /// A wrapper class because the VS Designer doesn't like inheriting from generic classes directly.
    /// </summary>
    public class QueryTabViewBase : UserControlBase<QueryTabViewModel>
    {
        public QueryTabViewBase(IViewModel<QueryTabViewModel> viewModel) : base(viewModel)
        {
        }
    }
}