using System;
using mshtml;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using UnManaged;
using WebBrowser.ViewModels;

namespace WebBrowser
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _Css;
        private Visibility _IsBrowserToolbarVisible;

        private string _jsScript;
        private bool alwaysOnTop;
        public string WindowTitle;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            string defaultURL = Common.Default.LastURL;
            wbSample.Navigate(defaultURL);
            //JsScript = "alert('ganesh');";
            Css = Common.Default.CSSSetting;
            JsScript = Common.Default.JSSetting;
            txtUrl.Text = defaultURL;

            WindowTitle = Common.Default.WindowTitle;
            CloseApplicationCommand = new RelayCommand<object>(OnCloseApplication, x => true);
            OpenApplicationCommand = new RelayCommand<object>(OnOpenApplication, x => true);
            HideApplicationCommand = new RelayCommand<object>(OnHideApplication, obj => true);
            ToggleToolbarCommand = new RelayCommand<object>(OnToggleToolbarCommand, x => true);
            RefreshBrowserCommand = new RelayCommand<object>(OnRefreshBrowserCommand, x => true);
            
            MoveLeftCommand = new RelayCommand<object>(OnMoveLeftCommand, x => true);
            MoveRightCommand = new RelayCommand<object>(OnMoveRightCommand, x => true);
            MoveTopCommand = new RelayCommand<object>(OnMoveTopCommand, x => true);
            MoveDownCommand = new RelayCommand<object>(OnMoveDownCommand, x => true);

            SizeLeftCommand = new RelayCommand<object>(OnSizeLeftCommand, x => true);
            SizeRightCommand = new RelayCommand<object>(OnSizeRightCommand, x => true);
            SizeTopCommand = new RelayCommand<object>(OnSizeTopCommand, x => true);
            SizeDownCommand = new RelayCommand<object>(OnSizeDownCommand, x => true);


            SaveSettingsCommand = new RelayCommand<object>(SaveSettings, x => true);
            AlwaysOnTop = false;
            IsBrowserToolbarVisible = Visibility.Collapsed;

            var _hotKey = new HotKey(Key.Escape, KeyModifier.Shift, OnHotKeyHandler);
            var _hotKey1 = new HotKey(Key.Tab, KeyModifier.Shift, OnHotKeyHandler1);
            this.Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            Common.Default.Save();
        }

        private void SaveSettings(object obj)
        {
            Common.Default.CSSSetting = Css;
            Common.Default.JSSetting = JsScript;
            Common.Default.LastURL = txtUrl.Text;
            wbSample.Navigate(txtUrl.Text);
        }

        private void OnMoveDownCommand(object obj)
        {
            Top += 2;
        }

        private void OnMoveTopCommand(object obj)
        {
            Top -= 2;
        }

        private void OnMoveRightCommand(object obj)
        {
            Left += 2;

        }

        private void OnMoveLeftCommand(object obj)
        {
            Left -= 2;

        }

        private void OnSizeDownCommand(object obj)
        {
            Height++;
        }

        private void OnSizeTopCommand(object obj)
        {
            Height--;
        }

        private void OnSizeRightCommand(object obj)
        {
            Width++;

        }

        private void OnSizeLeftCommand(object obj)
        {
            Width--;

        }

        private void OnHotKeyHandler1(HotKey obj)
        {
            CustomScreen screen = new CustomScreen();
            Thread t = new Thread(new ThreadStart(() =>
            {
                //byte originalBrightness = CustomScreen.GetCurBrightness();
                //CustomScreen.SetBrightness(100);

                ApplicationManager.ChangeWindow(true);
                 
                //CustomScreen.SetBrightness(originalBrightness);
               
            }));
           // t.IsBackground = true;
            t.Start();
        }
        private void OnHotKeyHandler(HotKey obj)
        {
            CustomScreen screen = new CustomScreen();
            Thread t = new Thread(new ThreadStart(() =>
            {
                //ApplicationManager.SaveScreen();
                ApplicationManager.ChangeWindow(false);
                
            }));

         t.Start();
        }

        private void OnRefreshBrowserCommand(object obj)
        {
            RefreshBrowser();
        }

        public ICommand CloseApplicationCommand { get; private set; }

        public Visibility IsBrowserToolbarVisible
        {
            get { return _IsBrowserToolbarVisible; }
            set
            {
                _IsBrowserToolbarVisible = value;
                ResizeMode = value == Visibility.Visible ? ResizeMode.CanResize : ResizeMode.NoResize;
                WindowStyle = value == Visibility.Visible ? System.Windows.WindowStyle.SingleBorderWindow : WindowStyle.None;
                OnPropertyChanged("IsBrowserToolbarVisible");
            }
        }

        private static Window GetHomeWindow()
        {
            Window homewindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is home);
            return homewindow ?? new MainWindow();
        }

        public RelayCommand<object> ToggleToolbarCommand { get; set; }
        public RelayCommand<object> HideApplicationCommand { get; set; }
        public RelayCommand<object> OpenApplicationCommand { get; set; }

        public string JsScript
        {
            get { return _jsScript; }
            set
            {
                if (_jsScript != value)
                {
                    _jsScript = value;
                    OnPropertyChanged("JsScript");
                    //RefreshBrowser();
                }
            }
        }

        public string Css
        {
            get { return _Css; }
            set
            {
                if (_Css != value)
                {
                    _Css = value;
                    OnPropertyChanged("Css");
                    //RefreshBrowser();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public RelayCommand<object> SaveSettingsCommand { get; set; }

        private void OnOpenApplication(object obj)
        {
            Show();
        }

        private void OnHideApplication(object obj)
        {
            Hide();
            GetHomeWindow().Focus();
        }

        private void OnCloseApplication(object obj)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void OnToggleToolbarCommand(object obj)
        {
            if (IsBrowserToolbarVisible == Visibility.Collapsed)
            {
                IsBrowserToolbarVisible = Visibility.Visible;
            }
            else
            {
                IsBrowserToolbarVisible = Visibility.Collapsed;

            }
        }

        private void txtUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PrependHttp();
                Common.Default.LastURL = txtUrl.Text;
                wbSample.Navigate(txtUrl.Text);
            }
        }

        private void PrependHttp()
        {
            if ((!txtUrl.Text.StartsWith("http://")) && (!txtUrl.Text.StartsWith("https://")))
            {
                txtUrl.Text = "http://" + txtUrl.Text;
            }
        }

        private void wbSample_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            txtUrl.Text = e.Uri.OriginalString;
        }


        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((wbSample != null) && (wbSample.CanGoBack));
        }

        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            wbSample.GoBack();
        }

        private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((wbSample != null) && (wbSample.CanGoForward));
        }

        private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            wbSample.GoForward();
        }

        private void GoToPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void GoToPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            wbSample.Navigate(txtUrl.Text);
        }

        private bool SaveSettings_CanExecute()
        {
            return true;
        }

   

        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void WbSample_OnNavigated(object sender, NavigationEventArgs e)
        {
            var browser = sender as System.Windows.Controls.WebBrowser;
            var doc = browser.Document as HTMLDocument;

            if (doc != null)
            {
                if (IsBrowserToolbarVisible == Visibility.Visible)
                {
                    return;
                }

                //Create the sctipt element 
                var scriptErrorSuppressed = (IHTMLScriptElement)doc.createElement("script");
                scriptErrorSuppressed.type = "text/javascript";
                scriptErrorSuppressed.text = JsScript;

                IHTMLStyleSheet ss = doc.createStyleSheet("", 0);
                // Now that you have the style sheet you have a few options:
                // 1. You can just set the content as text.
                ss.cssText = Css;

                //Inject it to the head of the page 
                IHTMLElementCollection nodes = doc.getElementsByTagName("head");
                foreach (IHTMLElement elem in nodes)
                {
                    var head = (HTMLHeadElement)elem;
                    head.appendChild((IHTMLDOMNode)scriptErrorSuppressed);
                }
            }
        }

        public void RefreshBrowser()
        {
            wbSample.Navigate(txtUrl.Text);
        }

        public RelayCommand<object> RefreshBrowserCommand { get; set; }

        public bool AlwaysOnTop
        {
            get { return alwaysOnTop; }
            set { alwaysOnTop = value; OnPropertyChanged("AlwaysOnTop"); }
        }

        public RelayCommand<object> MoveLeftCommand { get; set; }

        public RelayCommand<object> MoveRightCommand { get; set; }

        public RelayCommand<object> MoveTopCommand { get; set; }

        public RelayCommand<object> MoveDownCommand { get; set; }

        public RelayCommand<object> SizeDownCommand { get; set; }

        public RelayCommand<object> SizeTopCommand { get; set; }

        public RelayCommand<object> SizeRightCommand { get; set; }

        public RelayCommand<object> SizeLeftCommand { get; set; }
    }
}