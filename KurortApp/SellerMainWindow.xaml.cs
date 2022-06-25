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
        AddOrderWindow addOrderWindow;
        System.Windows.Threading.DispatcherTimer sessionTimer;
        TimeSpan _time;
        public SellerMainWindow()
        {
            InitializeComponent();
            addOrderWindow = new AddOrderWindow();
            acceptEquipmentBtn.Visibility = (SessionContext.CurrentUser.Role == "Продавец") ? Visibility.Hidden : Visibility.Visible;
            SetTimers();
            LoadOrders();
        }

        private void SetTimers()
        {
            _time = SessionContext.CurrentTime;
            sessionTimer = new System.Windows.Threading.DispatcherTimer(new TimeSpan(0, 0, 1), System.Windows.Threading.DispatcherPriority.Normal, delegate
            {
                _time = SessionContext.CurrentTime;
                timerTxt.Text = _time.ToString("c");
                addOrderWindow.timerTxt.Text = _time.ToString("c");
                if (_time == TimeSpan.Zero)
                {
                    sessionTimer.Stop();
                }
            }, Application.Current.Dispatcher);
            sessionTimer.Start();
            SessionContext.CurrentTime = TimeSpan.FromMinutes(150);
            App.sessionTimer.Start();
        }

        private void LoadOrders(string substring = "")
        {
            IEnumerable<Orders> OrderList = null;
            using (var db = new KurortDBEntities())
            {
                OrderList = (from d in db.Orders select d);
                if(substring.Replace(" ", "")!="")                 
                    OrderList = (from o in OrderList
                                 where o.Kod_zakaza.Contains($"{substring}") 
                                 select o);
                foreach (var order in OrderList)
                {
                    var mainBorder = new Border();
                    var gridPanel = new Grid();
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    var sp1 = new StackPanel() { Orientation = Orientation.Vertical };
                    var orderId = new Label() { Content = "Идентификатор: " };
                    var orderNum = new Label() { Content = "Код заказа: " };
                    var date = new Label() { Content = "Дата заказа: " };
                    var time = new Label() { Content = "Время заказа: " };


                    var sp1_5 = new StackPanel() { Orientation = Orientation.Vertical };
                    var services = new Label() { Content = "Услуги:", HorizontalContentAlignment= HorizontalAlignment.Center };
                    var servicesText = new TextBlock() {Text=""};

                    var sp2 = new StackPanel() { Orientation = Orientation.Vertical };
                    var status = new Label() { Content = "Статус заказа: ", HorizontalContentAlignment = HorizontalAlignment.Right };
                    var closeDate = new Label() { Content = "Дата закрытия: ", HorizontalContentAlignment = HorizontalAlignment.Right };
                    var rentalTime = new Label() { Content = "Время проката: ", HorizontalContentAlignment = HorizontalAlignment.Right };
                    var price = new Label() { Content = "Цена: ", HorizontalContentAlignment = HorizontalAlignment.Right };

                    //наполнение
                    var serviceSet = order.Services.Split(new string[] { "," }, StringSplitOptions.None);
                    int priceText = 0;

                    orderId.Content += order.Id.ToString();
                    orderNum.Content += order.Kod_zakaza;
                    date.Content += order.OrderDate.ToShortDateString();
                    time.Content += order.OrderTime.ToString();
                    foreach (var ser in serviceSet)
                    {
                        int serID = Convert.ToInt32(ser);
                        try
                        {
                            var service = (from s in db.Services where s.Id == serID select s).FirstOrDefault();
                            if (service is null)
                                continue;
                            priceText += service.Price;
                            servicesText.Text += ((servicesText.Text == "") ? "" : ",\n") + service.Name;
                        }
                        catch { }
                    }
                    status.Content += order.Status;
                    closeDate.Content += (order.CloseDate == null) ? "Нет данных" : ((DateTime)order.CloseDate).ToShortDateString();
                    rentalTime.Content += order.RentalTime;

                    
                    price.Content += priceText.ToString();
                    //добавление
                    Grid.SetColumn(sp1, 0);
                    Grid.SetColumn(sp1_5, 1);
                    Grid.SetColumn(sp2, 2);
                    sp1.Children.Add(orderId);
                    sp1.Children.Add(orderNum);
                    sp1.Children.Add(date);
                    sp1.Children.Add(time);
                    gridPanel.Children.Add(sp1);

                    sp1_5.Children.Add(services);
                    sp1_5.Children.Add(servicesText);
                    gridPanel.Children.Add(sp1_5);

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

        private void ButtonOrder_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            addOrderWindow.ShowDialog();
            contentPanel.Children.Clear();
            LoadOrders();
            this.ShowDialog();
        }
        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            SessionContext.SetDefaults();
            this.Close();
        }

        private void ButtonAcceptEquipment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            contentPanel.Children.Clear();
            LoadOrders(searchTxt.Text);
        }
    }
}
