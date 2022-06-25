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
    /// Логика взаимодействия для ServiceSelectionWindow.xaml
    /// </summary>
    public partial class ServiceSelectionWindow : Window
    {
        public ServiceSelectionWindow()
        {
            InitializeComponent();
            LoadServices();
        }
        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void LoadServices(string substring = "")
        {
            IEnumerable<Services> ServiceList = null;
            using (var db = new KurortDBEntities())
            {
                ServiceList = (from d in db.Services select d);
                if (substring.Replace(" ", "") == "")
                    ServiceList = (from s in ServiceList
                                where s.Name.Contains($"{substring}")
                                select s).ToList();
                foreach (var service in ServiceList)
                {
                    var mainBorder = new Border();
                    var gridPanel = new Grid();
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    var sp1 = new StackPanel() { Orientation = Orientation.Vertical };
                    var Name = new Label() { Content = "Навзание: " };
                    var kod = new Label() { Content = "Код услуги: " };
                    var price = new Label() { Content = "Цена: " };

                    var sp2 = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
                    var addBtn = new Button() { Width = 100, Height = 30, Content = "Добавить", Foreground = Brushes.White, Margin = new Thickness(0, 0, 10, 0), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Cursor = Cursors.Hand };
                    if (OrderContext.ServicesSet.Where(x => x.Id == service.Id).FirstOrDefault() != null)
                    {
                        addBtn.IsEnabled = false;
                        addBtn.Content = "Добавлено";
                    }
                    addBtn.Style = (Style)contentPanel.Resources["RoundedButtonStyle"];
                    addBtn.Tag = service;
                    addBtn.Click += AddBtn_Click;
                    //наполнение
                    Name.Content += service.Name;
                    kod.Content += service.Kod_uslugi;
                    price.Content += service.Price.ToString() + " p.";
                    //добавление
                    Grid.SetColumn(sp1, 0);
                    Grid.SetColumn(sp2, 1);
                    sp1.Children.Add(Name);
                    sp1.Children.Add(kod);
                    sp1.Children.Add(price);
                    gridPanel.Children.Add(sp1);

                    sp2.Children.Add(addBtn);
                    gridPanel.Children.Add(sp2);

                    mainBorder.Child = gridPanel;
                    contentPanel.Children.Add(mainBorder);
                }
            }
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            contentPanel.Children.Clear();
            LoadServices(searchTxt.Text);
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var service = ((sender as Button).Tag as Services);
            int serviceId = service.Id;
            if (OrderContext.ServicesSet.Where(x=>x.Id == serviceId).FirstOrDefault()==null)
                OrderContext.ServicesSet.Add((sender as Button).Tag as Services);
            (sender as Button).Content = "Добавлено";
            (sender as Button).IsEnabled = false;
            
        }

        private void AddBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = new BrushConverter().ConvertFrom("#498C51") as Brush;
        }

        private void AddBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = new BrushConverter().ConvertFrom("#5BAD65") as Brush;
        }
    }
}
