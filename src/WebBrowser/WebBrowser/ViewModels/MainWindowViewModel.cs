using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WebBrowser.ViewModels
{
    public class MainWindowViewModel:INotifyPropertyChanged
    {
        private Uri _uri;
        public Uri Uri
        {
            get { return _uri; }
            set { _uri = value; PropertyChanged(value, new PropertyChangedEventArgs("Uri")); } 
        }


        public MainWindowViewModel()
        {
            Uri = new Uri("http://google.com");
            BrowseCommand = new RelayCommand<Uri>(onBrowse, x => true);
        }

        private void onBrowse(Uri url)
        {
           
        }

        public static ICommand BrowseCommand { get; private set; }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
