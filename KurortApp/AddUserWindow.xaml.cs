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
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
            profileImage.Source = new BitmapImage(new Uri("ResoursesFolder/" + SessionContext.CurrentUser.FIO.Split(' ')[0] + ".jpg", UriKind.Relative));
            using (var db = new KurortDBEntities())
            {
                if (db.Clients.Count() != 0)
                    txtId.Text = ((from c in db.Clients 
                                   orderby c.UserId descending 
                                   select c.UserId).First() + 1).ToString();
                else
                    txtId.Text = "1";
            }
        }

        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonAddUserClick(object sender, RoutedEventArgs e)
        {
            using (var db = new KurortDBEntities())
            {
                string result = CheckData();
                if (result == "Success")
                {
                    Users user = db.Users.Add(new Users()
                    {
                        Id = Convert.ToInt32(txtId.Text),
                        Login = txtLogin.Text,
                        Password = txtPassword.Text,
                        FIO = txtFIO.Text,
                        Role = "Клиент"
                    });
                    db.Clients.Add(new Clients()
                    {
                        UserId = user.Id,
                        Passport = txtPassport.Text,
                        Birthday = DateTime.Parse(dataPicker.Text),
                        Address = txtAddress.Text
                    });
                    int t = db.SaveChanges();
                    if (t > 0)
                        MessageBox.Show("Клиент добавлен");
                    this.Close();
                    return;
                }
                MessageBox.Show(result);
            }
        }

        private string CheckData()
        {
            List<string> txtList = new List<string>{txtId.Text,txtLogin.Text,txtPassword.Text,txtPassport.Text,txtFIO.Text,txtAddress.Text,dataPicker.Text};
            bool result = txtList.Any(x => x.Replace(" ", "") == "");
            if (result)
                return "Не все поля заполнены!";
            using (var db = new KurortDBEntities())
            {
                int testID = Convert.ToInt32(txtId.Text);
                if (!(db.Users.Find(testID) is null))
                    return "Идентификатор уже занят.";
            }
            return "Success";
        }
    }
}
