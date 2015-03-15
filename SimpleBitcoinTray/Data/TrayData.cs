using Hardcodet.Wpf.TaskbarNotification;
using Info.Blockchain.API.ExchangeRates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitcoinTrayTicker.Data
{
    public static class TrayData
    {
        static TrayData()
        {
            CurrencyData.PriceUpdated += CurrencyData_PriceUpdated;
        }

        static void CurrencyData_PriceUpdated(object sender, EventArgs e)
        {
            updateTrayText(CurrencyData.SelectedCurrency);
        }
     

        static void updateTrayText(Currency price)
        {
            TaskbarIcon trayIcon = (TaskbarIcon)Application.Current.FindResource("myTaskbarIcon");

            Color myColor = Color.Green;

            History history = CurrencyData.getSecondToLast(Properties.Settings.Default.SelectedCurrency);            

            if (history != null)
            {
                Currency oldPrice = history.Prices.SingleOrDefault(x => x.Key == Properties.Settings.Default.SelectedCurrency).Value;

                double percentChanged = CalculateChange(oldPrice.Last, price.Last);

                if (percentChanged > 0)
                {
                    myColor = Color.Green;
                }
                else if (percentChanged < 0)
                {
                    myColor = Color.Red;
                }                
               
                if (percentChanged >= Properties.Settings.Default.NotifyPercentageValue || percentChanged <= -Properties.Settings.Default.NotifyPercentageValue)
                {
                    trayIcon.ShowBalloonTip(Math.Round(percentChanged, 2).ToString() + " %", "Old price: " + oldPrice.Last.ToString() + " " + oldPrice.Symbol + ", New price: " + price.Last.ToString() + " " + price.Symbol, BalloonIcon.Info);                    
                }                              
            }

            Brush brush = new SolidBrush(myColor);

            // Create a bitmap and draw text on it
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmap);

            int charCount = Math.Round(price.Last, 0).ToString().Count();

            Font myfont = null;
            if (charCount == 4)
            {
                myfont = new Font("Verdana", 5);
            }
            if (charCount == 3)
            {
                myfont = new Font("Verdana", 6);
            }
            if (charCount == 2)
            {
                myfont = new Font("Verdana", 7);
            }

            graphics.DrawString(Math.Round(price.Last, 0).ToString(), myfont, brush, 0, 3);

            // Convert the bitmap with text to an Icon
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);

            Application.Current.Dispatcher.Invoke(new Action(() => 
            {
                trayIcon.Icon = icon;
                trayIcon.ToolTipText = price.Last + " " + price.Symbol;
            }));            
        }

        public static void updateTrayText(string textTray, string textTooltip)
        {
            Brush brush = new SolidBrush(Color.White);

            // Create a bitmap and draw text on it
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graphics = Graphics.FromImage(bitmap);

            Font myfont = new Font("Verdana", 7);


            graphics.DrawString(textTray, myfont, brush, 0, 3);

            // Convert the bitmap with text to an Icon
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);

            TaskbarIcon trayIcon = (TaskbarIcon)Application.Current.FindResource("myTaskbarIcon");

            trayIcon.Icon = icon;
            trayIcon.ToolTipText = textTooltip;
        }

        static double CalculateChange(double previous, double current)
        {
            var change = current - previous;
            return ((double)change / previous) * 100;
        }

        
    }
}
