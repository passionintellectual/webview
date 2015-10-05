using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WebBrowser.ViewModels;

namespace WebBrowser
{
    /// <summary>
    /// Interaction logic for home.xaml
    /// </summary>
    public partial class home : Window
    {
        public home()
        {
            InitializeComponent();
            DataContext = this;
            CloseApplicationCommand = new RelayCommand<object>(OnCloseApplication, x => true);
            OpenApplicationCommand = new RelayCommand<object>(OnOpenApplication, x => true);
            HideApplicationCommand = new RelayCommand<object>(OnHideApplication, obj => true);
            GetMainWindow().Show();
            //this.Closed += home_Closed;
            
            
    
        }

        //void home_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = true;

        //    if (!KilledFlag)
        //    {
        //        KilledFlag = !KilledFlag;
        //        GetMainWindow().Hide();
        //        Hide();
        //    }
        //    else
        //    {
        //        KilledFlag = !KilledFlag;
        //        GetMainWindow().Show();
        //        Show();
        //    }

        //}
         

        private void OnOpenApplication(object obj)
        {
            GetMainWindow().Show();
        }

        private static Window GetMainWindow()
        {
            Window mainwindow =  Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow);
            return mainwindow == null ? new MainWindow() : mainwindow;
        }

        private void OnHideApplication(object obj)
        {
            GetMainWindow().Hide();
        }

        private void OnCloseApplication(object obj)
        {
            Process.GetCurrentProcess().Kill();
        }

        public RelayCommand<object> CloseApplicationCommand { get; set; }

        public RelayCommand<object> OpenApplicationCommand { get; set; }

        public RelayCommand<object> HideApplicationCommand { get; set; }
    }
}
