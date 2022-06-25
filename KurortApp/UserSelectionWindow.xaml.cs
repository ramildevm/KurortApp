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
            profileImage.Source = new BitmapImage(new Uri("ResoursesFolder/" + SessionContext.CurrentUser.FIO.Split(' ')[0] + ".jpg", UriKind.Relative));
            LoadUsers();
        }
        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void LoadUsers(string substring="")
        {
            IEnumerable<Users> UserList = null;
            using (var db = new KurortDBEntities())
            {
                UserList = db.Users.Where(user => user.Role == "Клиент").ToList<Users>();
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
                    var FIO = new Label() { Content = "ФИО: " };
                    var date = new Label() { Content = "Дата рождения: " };
                    var passport = new Label() { Content = "Пасспортные данные: " };
                    var address = new Label() { Content = "Адрес: " };

                    var sp2 = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
                    var addBtn = new Button() { Width = 100, Height = 30, Content = "Выбрать", FontFamily = new FontFamily("Comic Sans MS"), FontWeight = FontWeights.Bold, Foreground = Brushes.Black, Margin = new Thickness(0, 0, 10, 0), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Cursor = Cursors.Hand };
                    addBtn.Style = (Style)contentPanel.Resources["RoundedButtonStyle"];
                    addBtn.Tag = user;
                    addBtn.Click += AddBtn_Click;
                    //наполнение
                    var client = db.Clients.Where(c => c.UserId == user.Id).FirstOrDefault();
                    FIO.Content += user.FIO;
                    date.Content += client.Birthday.ToShortDateString();
                    passport.Content += client.Passport;
                    address.Content += client.Address;

                    //добавление
                    Grid.SetColumn(sp1, 0);
                    Grid.SetColumn(sp2, 1);
                    sp1.Children.Add(FIO);
                    sp1.Children.Add(date);
                    sp1.Children.Add(passport);
                    sp1.Children.Add(address);
                    gridPanel.Children.Add(sp1);

                    sp2.Children.Add(addBtn);
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
            LoadUsers(searchTxt.Text);
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            OrderContext.SelectedUser = (sender as Button).Tag as Users;
            this.Close();
        }

        private void AddBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = new BrushConverter().ConvertFrom("#76E383") as Brush;
        }

        private void AddBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = new BrushConverter().ConvertFrom("#ABE4B2") as Brush;
        }

        private void ButtonAddUserClick(object sender, RoutedEventArgs e)
        {
            var auw = new AddUserWindow();
            this.Hide();
            auw.ShowDialog();
            contentPanel.Children.Clear();
            LoadUsers();
            this.ShowDialog();
        }
    }
}
