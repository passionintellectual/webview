using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WebBrowser
{
    public class SoundHelper
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);
        public static void Mute(IntPtr handle)
        {
            SendMessageW(handle, WM_APPCOMMAND, handle,
                (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }

        public static void Decrement(IntPtr handle)
        {
            SendMessageW(handle, WM_APPCOMMAND, handle,
                (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }

        public static void Increment(IntPtr handle)
        {
            SendMessageW(handle, WM_APPCOMMAND, handle,
                (IntPtr)APPCOMMAND_VOLUME_UP);
        }

        public enum EDataFlow
        {

            eRender,

            eCapture,

            eAll,

            EDataFlow_enum_count

        }

     
       
    }
    public class ApplicationManager
    {
        public static IntPtr CurrentProcesshandle;
        public static int SW_SHOW = 5;
        public static int SW_HIDE = 0;
        
        // this next code is inside a class...
        static ApplicationManager()
        {
            ProcessesClosing = new List<Process>();
            CurrentProcesshandle = Process.GetCurrentProcess().MainWindowHandle;
        }
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        //Initialization
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool EnableWindow(IntPtr hwnd, bool enable);
        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr handle, int x, int y, int width,
        int height, bool redraw);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        [DllImport("user32.dll")]
        private static extern bool SetActiveWindow(IntPtr hwnd);

        private static List<Process> ProcessesClosing;
        public static bool SelectionProcess(Process p)
        {
            if (p.ProcessName.Contains("TutorialVideo"))
            {
                 
                return false;

            }
            if (!string.IsNullOrEmpty(p.MainWindowTitle))
            {
                return true;
            }
            return false;
        }
        public static bool KilledFlag;

        public static void ChangeWindow(bool showWindows)
        {
            if (!showWindows)
            {
                HideWindow("home");
                HideWindow("http://stackoverflow.com/");
            }
            else
            {

                ShowWindow("http://stackoverflow.com/");
                ShowWindow("home");
            }

           
            var processes = Process.GetProcesses().Where(SelectionProcess).ToList();
            int count = 0;
            if (processes.Count > 0)
            {

                foreach (var process in processes)
                {
                    //SetForegroundWindow(process.MainWindowHandle);
                    ShowWindow(process.MainWindowTitle);
                    Thread.Sleep(500);
                    count++;
                    
                    if (count > 3)
                    {
                        break;
                    }
                }
            }
            else
            {
            }
        }

        private static void ShowWindow(string title)
        {
            IntPtr HWND = FindWindow(null, title);
            ShowWindow(HWND, SW_SHOW);
            EnableWindow(HWND, true);
            SetActiveWindow(HWND);
            SoundHelper.Increment(HWND);


        }
        
        private static void HideWindow(string title)
        {
            IntPtr HWND = FindWindow(null, title);
            ShowWindow(HWND, SW_HIDE);
           // EnableWindow(HWND, true);

        }

        public static void SaveScreen( )
        {
           double x = Screen.PrimaryScreen.Bounds.X;
           double y = Screen.PrimaryScreen.Bounds.Y;
           double width = Screen.PrimaryScreen.Bounds.Width;
           double height = Screen.PrimaryScreen.Bounds.Height;


            int ix, iy, iw, ih;
            ix = Convert.ToInt32(x);
            iy = Convert.ToInt32(y);
            iw = Convert.ToInt32(width);
            ih = Convert.ToInt32(height);

            

            try
            {

                Bitmap myImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

                using (Graphics graphics = Graphics.FromImage(myImage))
                {
                    graphics.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                }

                DirectoryInfo d = new DirectoryInfo("C:\\printscreens\\");
                if (!d.Exists)
                {
                    d.Create();
                }
                var imagePath = d.ToString() + DateTime.Now.ToString().Replace("/", "-").Replace(":","--");
                imagePath = imagePath.Replace(",", string.Empty).Replace(" ","_").Replace(".","dot")+".png";

               // DialogResult res = dlg.ShowDialog();
               // if (res == System.Windows.Forms.DialogResult.OK)
                    myImage.Save(imagePath, ImageFormat.Png);
                ProcessStartInfo pinfo = new ProcessStartInfo("chrome");
                pinfo.WindowStyle = ProcessWindowStyle.Maximized;
                pinfo.Arguments = imagePath;
                var procStarted = Process.Start(pinfo);

            }
            catch { }
        }


    }
}
