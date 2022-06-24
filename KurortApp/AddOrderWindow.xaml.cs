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
        }
        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            barcodeTxt.Text = "";
            barcodeCan.Children.RemoveRange(0, barcodeCan.Children.Count - 1);
            if (txtId.Text.Replace(" ", "")=="" || txtRentTime.Text.Replace(" ", "") == "")
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
            code = txtId.Text + DateTime.Now.Day.ToString()+ DateTime.Now.Month.ToString()+ DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + txtRentTime.Text + rnd.Next(100000, 999999).ToString();
            barcodeTxt.Text = code;
            var barcode = new BarcodeGenerator();
            barcodeCan = barcode.MakeBarcode(code, barcodeCan);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap barcode = BarcodeToPDF.GetImage(barcodeCan);
            string filePAth = BarcodeToPDF.SaveAsPng(barcode);
            MessageBox.Show("Файл находиться в папке "+ BarcodeToPDF.SaveImageAsPDF(filePAth,barcodeTxt.Text));
            if (File.Exists(filePAth))
            {
                File.Delete(filePAth);
            }
        }

        private void ButtonSelectUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAddOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonService_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
