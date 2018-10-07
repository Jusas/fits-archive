using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveUI.Utils
{
    public class TranslationSource : INotifyPropertyChanged
    {
        private readonly ResourceManager _rm = Properties.Resources.ResourceManager;
        private static readonly TranslationSource _instance = new TranslationSource();
        public event PropertyChangedEventHandler PropertyChanged;
        public static TranslationSource Instance => _instance;
        public string this[string key] => _rm.GetString(key, _culture);
        private CultureInfo _culture;

        public TranslationSource()
        {
            
        }

        public CultureInfo CurrentCulture
        {
            get
            {
                return _culture;
            }

            set
            {
                if (_culture != value)
                {
                    _culture = value;
                    var @event = this.PropertyChanged;
                    @event?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                }
            }
        }


    }
}
