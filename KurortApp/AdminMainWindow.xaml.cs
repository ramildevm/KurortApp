using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
namespace KurortApp
{
    /// <summary>
    /// Логика взаимодействия для AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer sessionTimer;
        TimeSpan _time;
        public AdminMainWindow()
        {
            InitializeComponent();
            profileImage.Source = new BitmapImage(new Uri("ResoursesFolder/" + SessionContext.CurrentUser.FIO.Split(' ')[0] + ".jpg", UriKind.Relative));
            SetTimers();
            LoadUsers();
            contentPanel.Tag = "Пользователи";
        }
        private void SetTimers()
        {
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
        }
        private void LoadUsers(string substring = "")
        {
            IEnumerable<Users> UserList = null;
            using (var db = new KurortDBEntities())
            {
                UserList = db.Users.Where(user => user.Role != "Клиент").ToList<Users>();
                if (substring.Replace(" ", "") != "")
                    UserList = (from u in UserList
                                where u.FIO.Contains($"{substring}")
                                select u).ToList();
                foreach (var user in UserList)
                {
                    var mainBorder = new Border();
                    var gridPanel = new Grid();
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    var sp1 = new StackPanel() { Orientation = Orientation.Vertical };
                    var userid = new Label() { Content = "Идентификатор: " };
                    var login = new Label() { Content = "Логин: " };
                    var FIO = new Label() { Content = "ФИО: " };

                    var sp2 = new StackPanel() { Orientation = Orientation.Vertical };
                    var date = new Label() { Content = "Дата последнего входа: ", HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
                    var result = new Label() { Content = "Результат входа: ", HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
                    //наполнение
                    userid.Content += user.Id.ToString();
                    login.Content += user.Login;
                    FIO.Content += user.FIO;
                    date.Content += user.LastEnter;
                    result.Content += user.Result;

                    //добавление
                    Grid.SetColumn(sp1, 0);
                    Grid.SetColumn(sp2, 1);
                    sp1.Children.Add(userid);
                    sp1.Children.Add(login);
                    sp1.Children.Add(FIO);
                    gridPanel.Children.Add(sp1);

                    sp2.Children.Add(date);
                    sp2.Children.Add(result);
                    gridPanel.Children.Add(sp2);

                    mainBorder.Child = gridPanel;
                    contentPanel.Children.Add(mainBorder);
                }
                contentPanel.Children.Add(new Grid() { Height = 60 });
            }
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            contentPanel.Children.Clear();
            if (contentPanel.Tag as string == "Пользователи")
                LoadUsers(searchTxt.Text);
            else
                LoadOrders(searchTxt.Text);
        }
        private void ButtonMakeReportClick(object sender, RoutedEventArgs e)
        {
            String[] Header = null;
            String[][] data = null;
            string reportType = "";
            if(contentPanel.Tag as string == "Пользователи")
            {
                Header = new string[] { "Id", "Логин", "ФИО", "Роль", "Последний вход", "Результат входа" };
                using (var db = new KurortDBEntities())
                {
                    data = new string[db.Users.Count()][];
                    int i = 0;
                    foreach (var user in db.Users)
                    {
                        data[i] = new string[] { user.Id.ToString(), user.Login, user.FIO, user.Role, user.LastEnter, user.Result };
                        i++;
                    }
                }
                reportType = "Отчёт по пользователям";
            }
            else
            {
                Header = new string[] { "Id", "Код заказа", "Дата заказа", "Время заказа", "Id клиента", "Услуги","Статус","Дата закрытия","Время проката" };
                using (var db = new KurortDBEntities())
                {
                    data = new string[db.Orders.Count()][];
                    int i = 0;
                    foreach (var order in db.Orders)
                    {
                        data[i] = new string[] { order.Id.ToString(), order.Kod_zakaza, order.OrderDate.ToShortDateString(), order.OrderTime.ToString(), order.ClientId.ToString(), order.Services,  order.Status, order.CloseDate.ToString(), order.RentalTime };
                        i++;
                    }
                }
                reportType = "Отчёт по заказам";
            }
            string filePath = ReportMaker.MakeReportMethod(Header, data, reportType);
            var result = MessageBox.Show("Отчёт создан и находится по пути \n" + filePath + "\nНахмите «Да» для открытия", "Результат", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
                System.Diagnostics.Process.Start(filePath);
        }

        

        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonEquipmentList_Click(object sender, RoutedEventArgs e)
        {
            var aew = new AdminEquipmentWindow();
            this.Close();
            aew.ShowDialog();
        }

        private void ButtonOrdersClick(object sender, RoutedEventArgs e)
        {
            if (contentPanel.Tag as string == "Пользователи")
            {
                (sender as Button).Content = "Список пользователей";
                contentPanel.Tag = "Заказы";
                contentPanel.Children.Clear();
                LoadOrders();
            }
            else
            {
                (sender as Button).Content = "Список заказов";
                contentPanel.Tag = "Пользователи";
                contentPanel.Children.Clear();
                LoadUsers();
            }
        }
        private void LoadOrders(string substring = "")
        {
            IEnumerable<Orders> OrderList = null;
            using (var db = new KurortDBEntities())
            {
                OrderList = (from d in db.Orders select d);
                if (substring.Replace(" ", "") != "")
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
                    var services = new Label() {Content = "Услуги:", HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
                    var servicesText = new TextBlock() { Text = "" };

                    var sp2 = new StackPanel() { Orientation = Orientation.Vertical };
                    var status = new Label() { Content = "Статус заказа: ", HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right };
                    var closeDate = new Label() { Content = "Дата закрытия: ", HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right };
                    var rentalTime = new Label() { Content = "Время проката: ", HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right };
                    var price = new Label() { Content = "Цена: ", HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right };

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
                    servicesText.FontFamily = new System.Windows.Media.FontFamily("Comic Sans MS");
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
                contentPanel.Children.Add(new Grid() { Height = 60 });
            }
        }
    }
}
