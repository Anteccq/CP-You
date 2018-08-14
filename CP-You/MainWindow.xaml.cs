using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;

namespace CP_You
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private string PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ProjectATR\CP-You";
        private const string FILE = @"\point.config";

        private DispatcherTimer refreshTimer = new DispatcherTimer(DispatcherPriority.Normal);
        private PerformanceCounter pc;
        private double rightEnd;
        //プロパティ
        private Progress_info progress_Info;
        public Progress_info MeterPercent
        {
            get { return this.progress_Info; }
            set { progress_Info = value; Lighter(); }
        }
        public int CPUpercent
        {
            get { return progress_Info.Percent; }
            set { this.progress_Info.Percent = value; Lighter(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            rightEnd = SystemParameters.WorkArea.Width - this.Width;
            if (!FileRead())
            {
                this.Top = 0;
                this.Left = rightEnd;
            }
            MessageBox.Show(this.Left + " : " + this.Top + " : " + rightEnd);
            MeterPercent = new Progress_info();
            refreshTimer.Interval = new TimeSpan(0, 0, 1);
            refreshTimer.Tick += RefreshTimer_Tick;
        }
        private bool FileRead()
        {
            bool FLAG = true;
            if (!Directory.Exists(PATH)) Directory.CreateDirectory(PATH);
            if (File.Exists(PATH + FILE))
            {
                using (StreamReader sr = new StreamReader(PATH + FILE, Encoding.GetEncoding("utf-8")))
                {
                    Boolean check = false;
                    string data;
                    while ((data = sr.ReadLine()) != null)
                    {
                        if (data.IndexOf("Left:") != -1) this.Left = double.Parse(data.Replace("Left:", ""));
                        if (data.IndexOf("Top:") != -1) this.Top = double.Parse(data.Replace("Top:", ""));
                        if (data.IndexOf("iAoT:") != -1) check = Boolean.Parse(data.Replace("iAoT:",""));
                    }
                    this.AOT.IsChecked = this.Topmost = check;
                }
            }
            else FLAG = false;
            return FLAG;
        }
        private void FileWrite(double left,double top)
        {
            if (!Directory.Exists(PATH)) Directory.CreateDirectory(PATH);
            using (StreamWriter sw = new StreamWriter(PATH + FILE, false, Encoding.GetEncoding("utf-8")))
            {
                sw.WriteLine("DO_NOT_DELETE");
                sw.WriteLine("Left:" + left);
                sw.WriteLine("Top:" + top);
                sw.WriteLine("iAoT:" + AOT.IsChecked);
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (pc != null)
            {
                var db = (double)pc.NextValue();
                CPUpercent = (int)db;
                Meter.Value = CPUpercent;
            }
        }

        /// <summary>
        /// プロパティ変更によるデータ更新ライター
        /// </summary>
        private void Lighter()
        {
            Panel.DataContext = MeterPercent;
            TaskbarIcon.DataContext = MeterPercent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task task = new Task(() => pc = new PerformanceCounter("Processor", "% Processor Time", "_Total") );
            task.Start();
            refreshTimer.Start();
            CPUpercent = 5;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            if (this.Left >= rightEnd) this.Left = rightEnd;
            if (this.Left < 0) this.Left = 0;
        }

        //終了時にタスクトレイに残らないように修正
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TaskbarIcon.Visibility = Visibility.Collapsed;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            FileWrite(this.Left, this.Top);
            MessageBox.Show(this.Left + " : " + this.Top);
            this.Close();
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = this.AOT.IsChecked;
            FileWrite(this.Left, this.Top);
        }
    
        private void Activate(object sender, RoutedEventArgs e)
        {
            this.Activate();
        }
    }

    /// <summary>
    /// プログラスバー情報クラス
    /// </summary>
    public class Progress_info
    {
        public int Percent
        {
            get;set;
        }

        public Progress_info() : this(0)
        {
        }

        public Progress_info(int percent)
        {
            Percent = percent;
        }
    }
}
