using Hardcodet.Wpf.TaskbarNotification;
using Info.Blockchain.API.ExchangeRates;
using Newtonsoft.Json.Linq;
using BitcoinTrayTicker.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using BitcoinTrayTicker.UserControls;
using Microsoft.Win32;
using System.Reflection;
using Xceed.Wpf.Toolkit;
using Info.Blockchain.API;


namespace BitcoinTrayTicker
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        Timer myTimer = new Timer(new TimeSpan(0, 0, BitcoinTrayTicker.Properties.Settings.Default.UpdateInterval).TotalMilliseconds);

        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);   

        TaskbarIcon trayIcon;

        App()
        {
            Info.Blockchain.API.HttpClient.TimeoutMs = 20000;

            BitcoinTrayTicker.Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        void trayIcon_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            throw new NotImplementedException();
        }

        async void myTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await updatePrice(BitcoinTrayTicker.Properties.Settings.Default.SelectedCurrency);
        }

        async Task updatePrice(string currency)
        {
            try
            {
                await CurrencyData.updatePricesAsync(currency);
            }
            catch
            {
                // TODO
            }
        }

        void App_Exit(object sender, ExitEventArgs e)
        {
            BitcoinTrayTicker.Properties.Settings.Default.Save();

            myTimer.Dispose();

            trayIcon.Visibility = Visibility.Hidden;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            trayIcon = (TaskbarIcon)FindResource("myTaskbarIcon");

            TrayData.updateTrayText("...", "Loading data...");

            myTimer.Elapsed += myTimer_Elapsed;

            myTimer_Elapsed(null, null);

            myTimer.Start();

            this.Exit += App_Exit;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void cbCurrency_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add("USD");
            data.Add("EUR");
            data.Add("GBP");

            data.Sort();

            var comboBox = sender as ComboBox;

            comboBox.ItemsSource = data;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Donate donateWindow = new Donate();

            trayIcon.ShowCustomBalloon(donateWindow, System.Windows.Controls.Primitives.PopupAnimation.Slide, null);
        }

        async void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedCurrency":
                    await updatePrice(BitcoinTrayTicker.Properties.Settings.Default.SelectedCurrency);
                    break;
                case "UpdateInterval":
                    myTimer.Interval = new TimeSpan(0, 0, BitcoinTrayTicker.Properties.Settings.Default.UpdateInterval).TotalMilliseconds;
                    break;
                default:
                    break;
            }
        }

        private void DoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BitcoinTrayTicker.Properties.Settings.Default.Save();

        }

        private void ddUpdateInterval_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BitcoinTrayTicker.Properties.Settings.Default.Save();
        }           

        private void cbWindowsAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            rkApp.SetValue(Assembly.GetEntryAssembly().GetName().Name, Assembly.GetExecutingAssembly().Location);
        }

        private void cbWindowsAutoStart_Unchecked(object sender, RoutedEventArgs e)
        {
            rkApp.DeleteValue(Assembly.GetEntryAssembly().GetName().Name);
        }

        private void cbWindowsAutoStart_Initialized(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            if (rkApp.GetValue(Assembly.GetEntryAssembly().GetName().Name) == null)
            {                
                item.IsChecked = false;
            }
            else
            {               
                item.IsChecked = true;
            }
        }

        private async void WatermarkTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                WatermarkTextBox wmt = (WatermarkTextBox)sender;

              
                try
                {
                    double result = await CurrencyData.convertToBTC(Convert.ToDouble(wmt.Text));
                    trayIcon.ShowBalloonTip("", wmt.Text + " " + BitcoinTrayTicker.Properties.Settings.Default.SelectedCurrency + " = " + (result / 100000000).ToString() + " BTC", BalloonIcon.Info);
                }
                catch
                {
                   // TODO
                }
                finally
                {
                    wmt.Text = String.Empty;
                }
            }

          
        }

        private void WatermarkTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
