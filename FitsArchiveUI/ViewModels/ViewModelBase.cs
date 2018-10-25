using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        protected void SetNotifyingProperty<T>(string propName, ref T field, T value)
        {
            if (field == null || !field.Equals(value))
            {
                T oldValue = field;
                field = value;
                OnPropertyChanged(this, new PropertyChangedExtendedEventArgs<T>(propName, oldValue, value));
            }
        }

        /// <summary>
        /// A notification method for a property that doesn't have a backing field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        protected void SetNotifyingProperty<T>(string propName, T value)
        {
            var property = this.GetType().GetProperty(propName);
            var oldValue = (T)property.GetValue(this);
            property.SetValue(this, value);
            //var prop = Expression.Parameter(GetType(), "x");
            //var body = Expression.PropertyOrField(prop, propName);
            //var funcType = typeof(Func<,>).MakeGenericType(GetType(), typeof(T));
            //var lambda = Expression.Lambda(funcType, body, prop);
            //var oldValue = (T)lambda.Compile().DynamicInvoke(this);
            //var property = (lambda.Body as MemberExpression).Member as PropertyInfo;
            // property.SetValue(this, value);

            OnPropertyChanged(this, new PropertyChangedExtendedEventArgs<T>(propName, oldValue, value));        
        }

        protected void SetNotifyingProperty<T>(Expression<Func<T>> expression)
        {
            OnPropertyChanged(this, new PropertyChangedEventArgs(GetPropertyName(expression)));
        }

        // Deprecated
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
