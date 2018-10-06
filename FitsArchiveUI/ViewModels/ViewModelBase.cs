using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Interfaces;
using FitsArchiveUI.Utils;
using FitsArchiveUI.Views;

namespace FitsArchiveUI.ViewModels
{
    /// <summary>
    /// A nice wrapper that enables a bit easier property change notifications
    /// for ViewModel (-) View bindings.
    /// </summary>
    public abstract class ViewModelBase<TViewModel> : IViewModel<TViewModel>,
        INotifyPropertyChanged where TViewModel : ViewModelBase<TViewModel>
    {
        //-------------------------------------------------------------------------------------------------------
        #region FIELDS AND PROPERTIES
        //-------------------------------------------------------------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;

        public TViewModel ViewModel => this as TViewModel;

        private IView<TViewModel> _ownerView;
        public IView<TViewModel> OwnerView
        {
            set
            {
                _ownerView = value;
                if (value != null)
                {
                    OnOwnerViewSet(value);
                }
            }
            get
            {
                return _ownerView;
            }
        }
        protected ILog Log;

        #endregion


        //-------------------------------------------------------------------------------------------------------
        #region METHODS
        //-------------------------------------------------------------------------------------------------------

        protected ViewModelBase(ILog log)
        {
            Log = log;
        }

        public virtual void Initialize()
        {
            // Does nothing by default.
            // Views should call this after constructing the ViewModel.
        }

        internal virtual void OnOwnerViewSet(IView<TViewModel> view)
        {
        }

        protected void SetNotifyingProperty<T>(Expression<Func<T>> expression, ref T field, T value)
        {
            if (field == null || !field.Equals(value))
            {
                T oldValue = field;
                field = value;
                OnPropertyChanged(this, new PropertyChangedExtendedEventArgs<T>(GetPropertyName(expression), oldValue, value));
            }
        }

        /// <summary>
        /// A notification method for a property that doesn't have a backing field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        protected void SetNotifyingProperty<T>(Expression<Func<T>> expression, T value)
        {
            var oldValue = expression.Compile().Invoke();
            if (!oldValue.Equals(value))
            {
                OnPropertyChanged(this, new PropertyChangedExtendedEventArgs<T>(GetPropertyName(expression), oldValue, value));
            }
        }

        protected void SetNotifyingProperty<T>(Expression<Func<T>> expression)
        {
            OnPropertyChanged(this, new PropertyChangedEventArgs(GetPropertyName(expression)));
        }

        protected string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }

        public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(sender, e);
        }

        #endregion

    }
}
