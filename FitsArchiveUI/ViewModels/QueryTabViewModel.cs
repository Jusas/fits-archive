using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveUI.ViewModels
{
    public class QueryTabViewModel : ViewModelBase<QueryTabViewModel>
    {
        public class TestDataRow
        {
            public string Filename { get; set; }
            public string ObservationDate { get; set; }
            public string FrameKind { get; set; }
            public string Filter { get; set; }
            public string Ra { get; set; }
            public string Dec { get; set; }
            public string FocalLen { get; set; }
            public string Object { get; set; }
        }

        public List<TestDataRow> QueryResults { get; } = new List<TestDataRow>()
        {
            new TestDataRow()
            {
                Filename = @"C:\tmp\test.fits",
                ObservationDate = "2018-08-17T01:46:01.538Z",
                FrameKind = "Light",
                Filter = "Lum",
                Ra = "2 27 12.41",
                Dec = "33 34 00.63",
                FocalLen = "1480",
                Object = "Unknown"
            }
        };

        public QueryTabViewModel(ILog log) : base(log)
        {
        }


    }
}
