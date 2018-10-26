using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FitsArchiveLib.Attributes;
using FitsArchiveLib.Database;
using FitsArchiveLib.Interfaces;
using FitsArchiveUI.Utils;
using LinqToDB.Mapping;
using Ninject.Infrastructure.Language;

namespace FitsArchiveUI.ViewModels
{
    public enum FieldType
    {
        None,
        Text,
        TextRange,
        TextVariance,
        RaDec,
        Xy,
        Latitude,
        TimeRange
    }

    //public abstract class QueryComboItem
    //{
    //    public abstract bool IsHeader { get; }
    //    public abstract string Name { get; }
    //    public abstract string ToolTip { get; }
    //}

    //public class QueryComboHeader : QueryComboItem
    //{
    //    public QueryComboHeader(string name)
    //    {
    //        Name = name;
    //    }
    //    public override bool IsHeader => true;
    //    public override string Name { get; }
    //    public override string ToolTip { get; }
    //}

    public class QueryComboItemRow
    {
        public QueryComboItemRow(string name, string tooltip, bool isHeader, FieldType inputFieldType)
        {
            Name = name;
            ToolTip = tooltip;
            IsHeader = isHeader;
            InputFieldType = inputFieldType;
        }
        public bool IsHeader { get; }
        public string Name { get; }
        public string ToolTip { get; }
        public FieldType InputFieldType { get; }
    }
    
    /// <summary>
    /// Pairs with QueryParameterView, which is a visual representation of a query parameter.
    /// This in turn is a ViewModel representation of a query parameter for FitsDatabase,
    /// which will be used by the QueryTabContentViewModel when finally running a query.
    /// </summary>
    public class QueryParameterViewModel : ViewModelBase<QueryParameterViewModel>
    {
        public ObservableCollection<QueryComboItemRow> QueryComboItems { get; }

        private FieldType _fieldType = FieldType.Text;
        public FieldType FieldType
        {
            get => _fieldType;
            set => SetNotifyingProperty(nameof(FieldType), ref _fieldType, value);
        }

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
        public string QueryFieldVariance { get; set; }

        private int _selectedQueryTypeIndex = 1;

        public int SelectedQueryTypeIndex
        {
            get => _selectedQueryTypeIndex;
            set => SetNotifyingProperty(nameof(SelectedQueryTypeIndex), ref _selectedQueryTypeIndex, value);
        }

        static QueryParameterViewModel()
        {
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
            QueryComboItems = new ObservableCollection<QueryComboItemRow>()
            {
                new QueryComboItemRow("Aggregates", "", true, FieldType.None),
                new QueryComboItemRow("Filename/directory", "Filename search", false, FieldType.Text),
                new QueryComboItemRow("Date taken", "Uses DATE-OBS, DATE-END, TIME-OBS, TIME-END headers", false, FieldType.TimeRange),
                new QueryComboItemRow("RA, Dec with radius", "Uses RA/DEC, RA/DEC_OBJ, OBJCTRA/DEC headers + plate solve results", false, FieldType.RaDec),
                new QueryComboItemRow("Object name", "Uses OBJECT, OBJNAME", false, FieldType.Text),
                new QueryComboItemRow("Image width/height", "Uses NAXIS1, NAXIS2", false, FieldType.Xy),
                new QueryComboItemRow("Binning", "Uses XBINNING, YBINNING", false, FieldType.Xy),
                new QueryComboItemRow("Specific FITS keywords", "", true, FieldType.None)
            };

            var fitsKeywords = typeof(FitsHeaderIndexedRow).GetProperties()
                .Where(p => p.HasAttribute<FitsFieldAttribute>()).Select(x => x.GetCustomAttribute<FitsFieldAttribute>());
            foreach (var kw in fitsKeywords)
            {
                var fieldType = FieldType.Text;
                if (kw.DateLike)
                    fieldType = FieldType.TimeRange;
                if (kw.VarianceValue)
                    fieldType = FieldType.TextVariance;

                var row = new QueryComboItemRow(kw.Name, kw.Name, false, fieldType);
                QueryComboItems.Add(row);
            }

            PropertyChanged += OnQueryTypeIndexPropertyChange;
        }

        private void OnQueryTypeIndexPropertyChange(object sender, PropertyChangedEventArgs ea)
        {
            if (ea.PropertyName == nameof(SelectedQueryTypeIndex))
            {
                var newIndex = SelectedQueryTypeIndex;
                var queryItem = QueryComboItems[newIndex];
                FieldType = queryItem.InputFieldType;
            }
        }
    }
}