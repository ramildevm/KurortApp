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
    /// Логика взаимодействия для AdminEquipmentWindow.xaml
    /// </summary>
    public partial class AdminEquipmentWindow : Window
    {
        System.Windows.Threading.DispatcherTimer sessionTimer;
        TimeSpan _time;
        public AdminEquipmentWindow()
        {
            InitializeComponent();
            profileImage.Source = new BitmapImage(new Uri("ResoursesFolder/" + SessionContext.CurrentUser.FIO.Split(' ')[0] + ".jpg", UriKind.Relative));
            SetTimers();
            LoadEquipmentsList();
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
        private void LoadEquipmentsList(string substring = "")
        {
            IEnumerable<Goods> GoodsList = null;
            using (var db = new KurortDBEntities())
            {
                GoodsList = db.Goods;
                if (substring.Replace(" ", "") != "")
                    GoodsList = (from g in GoodsList
                                where g.Name.Contains($"{substring}")
                                select g).ToList();
                foreach (var goods in GoodsList)
                {
                    var mainBorder = new Border();
                    var gridPanel = new Grid();
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                    var sp1 = new StackPanel() { Orientation = Orientation.Vertical };
                    var id = new Label() { Content = "Идентификатор: " };
                    var name = new Label() { Content = "Название товара: " };

                    var sp2 = new StackPanel() { Orientation = Orientation.Vertical };
                    var quantity = new Label() { Content = "Общее количество: " , HorizontalAlignment = System.Windows.HorizontalAlignment.Right};
                    var inRent = new Label() { Content ="В прокате: ", HorizontalAlignment = System.Windows.HorizontalAlignment.Right };
                    //наполнение
                    id.Content += goods.Id.ToString();
                    name.Content += goods.Name;
                    quantity.Content += goods.Quantity.ToString();
                    inRent.Content += goods.InPrentCount.ToString();

                    //добавление
                    Grid.SetColumn(sp1, 0);
                    Grid.SetColumn(sp2, 1);
                    sp1.Children.Add(id);
                    sp1.Children.Add(name);
                    gridPanel.Children.Add(sp1);

                    sp2.Children.Add(quantity);
                    sp2.Children.Add(inRent);
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
            LoadEquipmentsList(searchTxt.Text);
        }
        private void ButtonMakeReportClick(object sender, RoutedEventArgs e)
        {
            String[] Header = { "Id", "Название товара", "Общее количество", "В прокате" };
            String[][] data;
            using (var db = new KurortDBEntities())
            {
                data = new string[db.Goods.Count()][];
                int i = 0;
                foreach (var goods in db.Goods)
                {
                    data[i] = new string[] { goods.Id.ToString(), goods.Name, goods.Quantity.ToString(), goods.InPrentCount.ToString() };
                    i++;
                }
            }
            string filePath = ReportMaker.MakeReportMethod(Header, data, "Отчет по товарам");         
            var result = MessageBox.Show("Отчёт создан и находится по пути \n" + filePath + "\nНахмите «Да» для открытия", "Результат", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
                System.Diagnostics.Process.Start(filePath);
        }

        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonUsersList_Click(object sender, RoutedEventArgs e)
        {
            var amw = new AdminMainWindow();
            this.Close();
            amw.ShowDialog();
        }
    }
}
