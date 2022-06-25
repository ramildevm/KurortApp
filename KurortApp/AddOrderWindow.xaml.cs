using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddOrderWindow.xaml
    /// </summary>
    public partial class AddOrderWindow : Window
    {
        public AddOrderWindow()
        {
            InitializeComponent();
            OrderContext.SetDefaults();
            using (var db = new KurortDBEntities())
            {
                if (db.Orders.Count() != 0)
                    txtId.Text = (db.Orders.ToList().Last().Id + 1).ToString();
                else
                    txtId.Text = "1";
            }
        }
        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            barcodeTxt.Text = "";
            barcodeCan.Children.RemoveRange(0, barcodeCan.Children.Count - 1);
            if (txtId.Text.Replace(" ", "") == "" || txtRentTime.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Не все поля заполнены!");
                return;
            }
            try
            {
                Convert.ToInt32(txtId.Text);
                Convert.ToInt32(txtRentTime.Text);
            }
            catch
            {
                MessageBox.Show("Некорректные данные!");
                return;
            }
            String code = "";
            var rnd = new Random();
            code = txtId.Text + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + txtRentTime.Text + rnd.Next(100000, 999999).ToString();
            barcodeTxt.Text = code;
            var barcode = new BarcodeGenerator();
            barcodeCan = barcode.MakeBarcode(code, barcodeCan);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap barcode = BarcodeToPDF.GetImage(barcodeCan);
            string filePAth = BarcodeToPDF.SaveAsPng(barcode);
            string result = BarcodeToPDF.SaveImageAsPDF(filePAth, barcodeTxt.Text);
            if (result != "Ошибка")
                MessageBox.Show("Файл находиться в папке " + result);
            if (File.Exists(filePAth))
            {
                File.Delete(filePAth);
            }
        }

        private void ButtonSelectUser_Click(object sender, RoutedEventArgs e)
        {
            var usw = new UserSelectionWindow();
            this.Hide();
            usw.ShowDialog();
            if (!(OrderContext.SelectedUser is null))
            {
                txtUserId.Text = OrderContext.SelectedUser.Id.ToString();
            }
            this.ShowDialog();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonAddOrder_Click(object sender, RoutedEventArgs e)
        {
            string result = CheckData();
            if (result == "Success")
            {
                using (var db = new KurortDBEntities())
                {
                    Orders order = db.Orders.Add(new Orders()
                    {
                        Id = Convert.ToInt32(txtId.Text),
                        Kod_zakaza = txtUserId.Text + "/" + DateTime.Now.ToShortDateString(),
                        OrderDate = DateTime.Now.Date,
                        OrderTime = DateTime.Now.TimeOfDay,
                        ClientId = OrderContext.SelectedUser.Id,
                        Services = txtServiceList.Text,
                        Status = "Новая",
                        CloseDate = null,
                        RentalTime = txtRentTime.Text + " часа."
                    });
                    db.Barcodes.Add(new Barcodes()
                    {
                        OrderId = order.Id,
                        Barcode = barcodeTxt.Text
                    });
                    int t = db.SaveChanges();
                    if (t > 0)
                        MessageBox.Show("Заказ оформлен");
                }
                this.Close();
            }
            else
            {
                MessageBox.Show(result, "Внимание!");
            }
        }
        string CheckData()
        {
            if (txtId.Text.Length == 0 || txtRentTime.Text.Length == 0)
                return "Не все поля заполнены!";
            else if (txtId.Text.Replace(" ", "") == "" || txtRentTime.Text.Replace(" ", "") == "")
                return "Не все поля заполнены!";
            else if (barcodeTxt.Text == "")
                return "Штрих-код не сгенерирован!";
            else if (txtUserId.Text == "")
                return "Клиент не выбран!";
            else if (txtServiceList.Text == "")
                return "Ни одна из услуг не выбрана!";
            using (var db = new KurortDBEntities())
            {
                int testID = Convert.ToInt32(txtId.Text);
                if (!(db.Orders.Find(testID) is null))
                    return "Идентификатор уже занят.";
            }
            return "Success";
        }

        private void ButtonService_Click(object sender, RoutedEventArgs e)
        {
            var ssw = new ServiceSelectionWindow();
            this.Hide();
            ssw.ShowDialog();
            ServicesToTxt();
            this.ShowDialog();
        }

        private void ServicesToTxt()
        {
            string services = "";
            int iter = 0;
            foreach (var s in OrderContext.ServicesSet)
            {
                iter++;
                if (iter == OrderContext.ServicesSet.Count())
                    services += s.Id.ToString();
                else
                    services += s.Id.ToString() + ", ";
            }
            txtServiceList.Text = services;
        }
    }
}
