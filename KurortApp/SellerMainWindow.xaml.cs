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
        public SellerMainWindow()
        {
            InitializeComponent();
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
