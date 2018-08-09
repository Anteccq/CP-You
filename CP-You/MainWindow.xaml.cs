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
        private string PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ProjectATR";
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
            if (FileRead() == false)
            {
                this.Top = 0;
                this.Left = rightEnd;
            }
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
                    string data;
                    while ((data = sr.ReadLine()) != null)
                    {
                        if (data.IndexOf("Left:") != -1)
                        {
                            double.TryParse((data.Replace("Left:", "")), out double X);
                            this.Left = X;
                        }
                        if (data.IndexOf("Top:") != -1)
                        {
                            double.TryParse((data.Replace("Top:", "")), out double Y);
                            this.Top = Y;
                        }
                    }
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
            }
        }

        /// <summary>
        /// DispathcerTimerスタート後はPanel.DataContextによる子要素の更新が不可能に。不明。調査中。
        /// </summary>
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
            FileWrite(this.Left,this.Top);
        }

        //終了時にタスクトレイに残らないように修正
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TaskbarIcon.Visibility = Visibility.Collapsed;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
