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

namespace CP_You
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
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

            MeterPercent = new Progress_info(100);
            Panel.DataContext = MeterPercent;
            CPUpercent = 10;
        }

        private void Lighter()
        {
            Panel.DataContext = MeterPercent;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Console.WriteLine("");
        }
    }
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
