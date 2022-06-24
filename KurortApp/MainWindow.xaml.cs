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

namespace KurortApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Windows.Threading.DispatcherTimer sessionTimer1;
        TimeSpan _time;

        System.Windows.Threading.DispatcherTimer logInAccessTimer = new System.Windows.Threading.DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            _time = TimeSpan.FromSeconds(120);
            
            logInAccessTimer.Interval = SessionContext.TimerInterval;
            logInAccessTimer.Tick += LogInAccessTimer_Tick;
            logInAccessTimer.Start();
        }

        private void LogInAccessTimer_Tick(object sender, EventArgs e)
        {
            loginBtn.IsEnabled = true;
            SessionContext.SetTimer();
            logInAccessTimer.Stop();
        }

        private void ShowPasswordCharsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            txtPassword.Visibility = System.Windows.Visibility.Collapsed;
            MyTextBox.Text = txtPassword.Password;
            borderShowPassword.Visibility = System.Windows.Visibility.Visible;

            MyTextBox.Focus();
        }

        private void ShowPasswordCharsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPassword.Visibility = System.Windows.Visibility.Visible;
            borderShowPassword.Visibility = System.Windows.Visibility.Collapsed;
            
            txtPassword.Focus();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if(SessionContext.Attempts >= 2)
            {
                var capthaWindow = new CaptchaWindow();
                this.Hide();
                capthaWindow.ShowDialog();
                loginBtn.IsEnabled = false;
                logInAccessTimer.Interval = SessionContext.TimerInterval;
                logInAccessTimer.Start();
                this.Show();
                return;
            }
            string result = LoginMethod(txtLogin.Text, txtPassword.Password);
            if (result == $"Добро пожаловать, {txtLogin.Text}!")
            {
                var SellerWindow = new SellerMainWindow();
                this.Close();
                SellerWindow.ShowDialog();
                //using (var db = new Pharmacy_ValeriankaEntities())
                //{
                //    Users user = (from u in db.Users where u.UserLogin == txtLogin.Text select u).FirstOrDefault();
                //    if (user.UserRole == "User")
                //    {
                //        SystemContext.typeWindow = "Каталог";
                //        ClientMainWindow cmw = new ClientMainWindow();
                //        this.Close();
                //        cmw.ShowDialog();
                //    }
                //    else if (user.UserRole == "Employee")
                //    {
                //        EmployeeMainWindow emw = new EmployeeMainWindow();
                //        this.Close();
                //        emw.ShowDialog();
                //    }
                //    else if (user.UserRole == "Admin")
                //    {
                //        AdminWindow aw = new AdminWindow();
                //        this.Close();
                //        aw.ShowDialog();
                //    }
                //    else
                //    {
                //        MessageBox.Show("Произошла ошибка с определением роли пользователя", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                //    }
                //}
            }
            else
            {
                SessionContext.Attempts++;
                MessageBox.Show(result, "Результат", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private string LoginMethod(string login, string password)
        {
            if (login != "admin" && password != "admin")
                return "Error";
            //if (login.Length == 0 || password.Length == 0)
            //    return "Не все поля заполнены!";
            //using (var db = new Pharmacy_ValeriankaEntities())
            //{
            //    Users user = (from u in db.Users where u.UserLogin == login select u).FirstOrDefault();
            //    if (user == null)
            //        return "Пользователя с таким логином не существует!";
            //    if (user.UserPassword != password)
            //        return "Неверный пароль!";
            //    if (user.UserRole == "User")
            //        SystemContext.Client = (from c in db.Client where c.UserID == user.UserID select c).FirstOrDefault();
            //    SystemContext.User = user;
            //}
            return $"Добро пожаловать, {login}!";
        }
    }
}
