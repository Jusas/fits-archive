using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using FitsArchiveLib.Attributes;
using FitsArchiveLib.Database;
using FitsArchiveLib.Interfaces;
using LinqToDB.Mapping;
using Ninject.Infrastructure.Language;

namespace FitsArchiveUI.ViewModels
{
    public enum FieldType
    {
        Text,
        TextRange,
        RaDec,
        Xy,
        Latitude,
        TimeRange
    }

    public abstract class QueryComboItem
    {
        public abstract bool IsHeader { get; }
        public abstract string Name { get; }
    }

    public class QueryComboHeader : QueryComboItem
    {
        public QueryComboHeader(string name)
        {
            Name = name;
        }
        public override bool IsHeader => true;
        public override string Name { get; }
    }

    public class QueryComboItemRow : QueryComboItem
    {
        public QueryComboItemRow(string name)
        {
            Name = name;
        }
        public override bool IsHeader => false;
        public override string Name { get; }
    }


    public class QueryType
    {
        public string Name { get; set; }
        public FieldType FieldType { get; set; }
        public string Category { get; set; }
    }

    /// <summary>
    /// Pairs with QueryParameterView, which is a visual representation of a query parameter.
    /// This in turn is a ViewModel representation of a query parameter for FitsDatabase,
    /// which will be used by the QueryTabContentViewModel when finally running a query.
    /// </summary>
    public class QueryParameterViewModel : ViewModelBase<QueryParameterViewModel>
    {
        private static readonly ObservableCollection<QueryType> _queryTypes;
        public ObservableCollection<QueryComboItem> QueryComboItems { get; }

        public FieldType FieldType { get; set; } = FieldType.Text;
        private string _queryFieldText;
        public string QueryFieldText
        {
            get => _queryFieldText;
            set => SetNotifyingProperty(nameof(QueryFieldText), ref _queryFieldText, value);
        }
        public string QueryFieldTextRangeStart { get; set; }
        public string QueryFieldTextRangeEnd { get; set; }
        public string QueryFieldRa { get; set; }
        public string QueryFieldDec { get; set; }
        public string QueryFieldRaDecRadius { get; set; }
        public string QueryFieldX { get; set; }
        public string QueryFieldY { get; set; }
        public string QueryFieldLat { get; set; }
        public string QueryFieldTimeStart { get; set; }
        public string QueryFieldTimeEnd { get; set; }

        static QueryParameterViewModel()
        {
            _queryTypes = new ObservableCollection<QueryType>();
            var fitsTableProperties = typeof(FitsHeaderIndexedRow).GetProperties()
                .Where(p => p.HasAttribute<FitsFieldAttribute>());

            //foreach (var fitsTableProperty in fitsTableProperties)
            //{
            //    var query = new QueryType()
            //    {
            //        Category = "Specific FITS keyword",
            //        FieldType = FieldType.Text,
            //    }
            //    fitsTableProperty.GetCustomAttribute<FitsFieldAttribute>().Name;


            //}
        }

        public QueryParameterViewModel(ILog log) : base(log)
        {
            // TODO: textfields should have "like" combobox, ie. get distinct values for the keyword and suggest "autocomplete" style as user types.
            QueryComboItems = new ObservableCollection<QueryComboItem>()
            {
                new QueryComboHeader("Specific FITS keywords"),
                new QueryComboItemRow("TELESCOP"),
                new QueryComboItemRow("EXPOSURE"),
                new QueryComboItemRow("DATE-OBS")
        };
        }
    }
}