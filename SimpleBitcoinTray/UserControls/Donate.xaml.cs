using Hardcodet.Wpf.TaskbarNotification;
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
using System.Windows.Shapes;

namespace BitcoinTrayTicker.UserControls
{
    /// <summary>
    /// Interaktionslogik für Donate.xaml
    /// </summary>
    public partial class Donate : UserControl 
    {
        public Donate()
        {
            InitializeComponent();            
        }

        private void Button_Click(object sender, RoutedEventArgs e)        
        {
            System.Windows.Clipboard.SetText("1D88wCn7GekvQpvQSHW4xiHyDUjhoTvgUj");

            TaskbarIcon trayIcon = (TaskbarIcon)FindResource("myTaskbarIcon");

            trayIcon.ShowBalloonTip("Thanks!", "Address copied to your clipboard", BalloonIcon.None);

            trayIcon.CloseBalloon();


        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TaskbarIcon trayIcon = (TaskbarIcon)FindResource("myTaskbarIcon");
            
            trayIcon.CloseBalloon();
        }
    }
}
