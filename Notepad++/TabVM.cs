using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Notepad__
{
    internal class TabVM : INotifyPropertyChanged
    {
        string _Header;
        public string Header
        {
            get => _Header;
            set
            {
                _Header = value;
                OnPropertyChanged();
            }
        }

        bool _IsPlaceholder = false;
        public bool IsPlaceholder
        {
            get => _IsPlaceholder;
            set
            {
                _IsPlaceholder = value;
                OnPropertyChanged();
            }
        }

        string _Content;
        public string Content
        {
            get => _Content;
            set
            {
                _Content = value;
                Color = "Red";
                OnPropertyChanged();
            }
        }

        string _Path = "";
        public string Path
        {
            get => _Path;
            set
            {
                _Path = value;
            }
        }

        string _Color = "Black";
        public string Color
        {
            get => _Color;
            set
            {
                _Color = value;
                OnPropertyChanged();

            }
        }

        string _Theme = "White";
        public string Theme
        {
            get => _Theme;
            set
            {
                _Theme = value;
                OnPropertyChanged();
            }
        }

        string _CounterTheme = "#424242";
        public string CounterTheme
        {
            get => _CounterTheme;
            set
            {
                _CounterTheme = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
