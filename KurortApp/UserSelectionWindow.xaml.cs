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
    /// Логика взаимодействия для UserSelectionWindow.xaml
    /// </summary>
    public partial class UserSelectionWindow : Window
    {
        public UserSelectionWindow()
        {
            InitializeComponent();
            LoadUsers();
        }
        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            SessionContext.SetDefaults();
            this.Close();
        }
        private void LoadUsers()
        {
            using (var db = new KurortDBEntities())
            {
                foreach(var order in db.Orders)
                {
                    var mainBorder = new Border();
                    var gridPanel = new Grid();
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    var sp1 = new StackPanel() { Orientation = Orientation.Vertical };
                    var orderNum = new Label() { Content = "Код заказа: " };
                    var date = new Label() { Content = "Дата заказа: " };
                    var time = new Label() { Content = "Время заказа: " };
                    var services = new Label() { Content = "Услуги: " };

                    var sp2 = new StackPanel() { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Right };
                    var status = new Label() { Content = "Статус заказа: " };
                    var closeDate = new Label() { Content = "Дата закрытия: " };
                    var rentalTime = new Label() { Content = "Время проката: " };
                    var price = new Label() { Content = "Цена: " };

                    //наполнение
                    orderNum.Content += order.Kod_zakaza;
                    date.Content += order.OrderDate.ToShortDateString();
                    time.Content += order.OrderTime.ToString();
                    services.Content += order.Services;
                    status.Content += order.Status;
                    closeDate.Content += (order.CloseDate == null) ? "Нет данных" : order.CloseDate.ToString();
                    rentalTime.Content += order.RentalTime;

                    int priceText = 0;
                    foreach (var ser in order.Services.Split(','))
                    {
                        priceText += db.Services.Where(x=>x.Id==Convert.ToInt32(ser)).Select(x => x.Price).FirstOrDefault();
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
        }
    }
}
