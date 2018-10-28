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

    //public class QueryClause
    //{
    //    public IndexedFitsKeyword FitsKeyword;
    //    public 
    //}

    //public class BuiltQuery
    //{
        
    //}
    
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

        private string _queryFieldTextRangeStart;
        public string QueryFieldTextRangeStart
        {
            get => _queryFieldTextRangeStart;
            set => SetNotifyingProperty(nameof(QueryFieldTextRangeStart), ref _queryFieldTextRangeStart, value);
        }

        private string _queryFieldTextRangeEnd;
        public string QueryFieldTextRangeEnd
        {
            get => _queryFieldTextRangeEnd;
            set => SetNotifyingProperty(nameof(QueryFieldTextRangeEnd), ref _queryFieldTextRangeEnd, value);
        }

        private string _queryFieldRa;
        public string QueryFieldRa
        {
            get => _queryFieldRa;
            set => SetNotifyingProperty(nameof(QueryFieldRa), ref _queryFieldRa, value);
        }

        private string _queryFieldDec;
        public string QueryFieldDec
        {
            get => _queryFieldDec;
            set => SetNotifyingProperty(nameof(QueryFieldDec), ref _queryFieldDec, value);
        }

        private string _queryFieldRaDecRadius;
        public string QueryFieldRaDecRadius
        {
            get => _queryFieldRaDecRadius;
            set => SetNotifyingProperty(nameof(QueryFieldRaDecRadius), ref _queryFieldRaDecRadius, value);
        }

        private string _queryFieldX;
        public string QueryFieldX
        {
            get => _queryFieldX;
            set => SetNotifyingProperty(nameof(QueryFieldX), ref _queryFieldX, value);
        }

        private string _queryFieldY;
        public string QueryFieldY
        {
            get => _queryFieldY;
            set => SetNotifyingProperty(nameof(QueryFieldY), ref _queryFieldY, value);
        }

        private string _queryFieldLat;
        public string QueryFieldLat
        {
            get => _queryFieldLat;
            set => SetNotifyingProperty(nameof(QueryFieldLat), ref _queryFieldLat, value);
        }

        private string _queryFieldTimeStart;
        public string QueryFieldTimeStart
        {
            get => _queryFieldTimeStart;
            set => SetNotifyingProperty(nameof(QueryFieldTimeStart), ref _queryFieldTimeStart, value);
        }

        private string _queryFieldTimeEnd;
        public string QueryFieldTimeEnd
        {
            get => _queryFieldTimeEnd;
            set => SetNotifyingProperty(nameof(QueryFieldTimeEnd), ref _queryFieldTimeEnd, value);
        }

        private string _queryFieldVariance;
        public string QueryFieldVariance
        {
            get => _queryFieldVariance;
            set => SetNotifyingProperty(nameof(QueryFieldVariance), ref _queryFieldVariance, value);
        }

        private int _selectedQueryTypeIndex = 1;
        public int SelectedQueryTypeIndex
        {
            get => _selectedQueryTypeIndex;
            set => SetNotifyingProperty(nameof(SelectedQueryTypeIndex), ref _selectedQueryTypeIndex, value);
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


            var fitsKeywords = FitsDatabaseDescription.IndexedFitsKeywords;
            foreach (var kw in fitsKeywords)
            {
                var fieldType = FieldType.Text;
                if (kw.ValueType == KeywordValueType.Date)
                    fieldType = FieldType.TimeRange;
                if ((kw.QueryHints & KeywordQueryHints.VarianceValue) != 0)
                    fieldType = FieldType.TextVariance;

                var row = new QueryComboItemRow(kw.Keyword, kw.Keyword, false, fieldType);
                QueryComboItems.Add(row);
            }

            PropertyChanged += OnQueryTypeIndexPropertyChange;
        }

        // todo: GetAsQuery - maybe as an expression?

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