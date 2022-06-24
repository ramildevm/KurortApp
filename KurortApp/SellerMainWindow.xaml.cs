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

namespace KurortApp
{
    /// <summary>
    /// Логика взаимодействия для SellerMainWindow.xaml
    /// </summary>
    public partial class SellerMainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer sessionTimer;
        TimeSpan _time;
        public SellerMainWindow()
        {
            InitializeComponent();
            timerTxt.Text = _time.ToString("c");
            _time = SessionContext.CurrentTime;
            sessionTimer = new System.Windows.Threading.DispatcherTimer(new TimeSpan(0, 0, 1), System.Windows.Threading.DispatcherPriority.Normal, delegate
            {
                _time = SessionContext.CurrentTime;
                timerTxt.Text = _time.ToString("c");
                if (_time == TimeSpan.Zero)
                {
                    sessionTimer.Stop();
                }
            }, Application.Current.Dispatcher);
            sessionTimer.Start();
            SessionContext.CurrentTime = TimeSpan.FromMinutes(150);
            App.sessionTimer.Start();
            LoadOrders();
        }

        private void LoadOrders()
        {
            String servicesText = "12, 434, 23, 45";
            for (int i = 0; i < 6; i++)
            {
                var mainBorder = new Border();
                var gridPanel = new Grid();
                gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                var sp1 = new StackPanel() { Orientation = Orientation.Vertical};
                var orderNum = new Label() { Content = "Код заказа: " };
                var date = new Label() { Content = "Дата заказа: " };
                var time = new Label() { Content = "Время заказа: " };
                var services = new Label() { Content = "Услуги: " };

                var sp2 = new StackPanel() { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Right};
                var status = new Label() { Content = "Статус заказа: " };
                var closeDate = new Label() { Content = "Дата закрытия: " };
                var rentalTime = new Label() { Content = "Время проката: " };
                var price = new Label() { Content = "Цена: " };

                //наполнение
                orderNum.Content += i.ToString();
                date.Content += (2000 + i*1).ToString();
                time.Content += "12:00";
                services.Content += servicesText;
                status.Content += (i%2==0)?"IN rent":"Closed";
                string closed = (i % 2 == 0) ? null : "Closed";
                closeDate.Content += (closed == null)?"No data":closed;
                rentalTime.Content += i.ToString() + " h.";

                int priceText = 0;
                foreach (var ser in servicesText.Split(','))
                {
                    priceText += Convert.ToInt32(ser);
                }
                price.Content += priceText.ToString();
                //добавление
                Grid.SetColumn(sp1, 0);
                Grid.SetColumn(sp2, 1);
                sp1.Children.Add(orderNum);
                sp1.Children.Add(date);
                sp1.Children.Add(time);
                sp1.Children.Add(services);
                gridPanel.Children.Add(sp1);

                sp2.Children.Add(status);
                sp2.Children.Add(closeDate);
                sp2.Children.Add(rentalTime);
                sp2.Children.Add(price);
                gridPanel.Children.Add(sp2);

                mainBorder.Child = gridPanel;
                contentPanel.Children.Add(mainBorder);
            }
        }

        private void ButtonOrder_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AddBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = new BrushConverter().ConvertFrom("#498C51") as Brush;
        }

        private void AddBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = new BrushConverter().ConvertFrom("#DD498C51") as Brush;
        }
    }
}
