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
                    var date = new Label() { Content = "Дата последнего входа: " };
                    var result = new Label() { Content = "Результат входа: " };
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
            LoadUsers(searchTxt.Text);
        }
        private void ButtonMakeReportClick(object sender, RoutedEventArgs e)
        {   
            Document doc = new Document();
            Spire.Doc.Section s = doc.AddSection();

            String[] Header = { "Id", "Логин", "ФИО", "Роль", "Последний вход", "Результат входа" };
            String[][] data;
            using (var db = new KurortDBEntities())
            {
                data = new string[db.Users.Count()][];
                int i = 0;
                foreach(var user in db.Users)
                {
                    data[i] = new string[]{user.Id.ToString(),user.Login,user.FIO,user.Role,user.LastEnter,user.Result};
                    i++;
                }
            }
            Table table = s.AddTable(true);
            table.ResetCells(data.Length + 1, Header.Length);
            Spire.Doc.TableRow FRow = table.Rows[0];
            FRow.IsHeader = true;
            FRow.Height = 23;
            FRow.RowFormat.BackColor = System.Drawing.Color.LightGray;
            for (int i = 0; i < Header.Length; i++)
            {
                Paragraph p = FRow.Cells[i].AddParagraph();
                FRow.Cells[i].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                TextRange TR = p.AppendText(Header[i]);
                TR.CharacterFormat.FontName = "Calibri";
                TR.CharacterFormat.FontSize = 12;
                TR.CharacterFormat.TextColor = System.Drawing.Color.White;
                TR.CharacterFormat.Bold = true;
            }
            for (int r = 0; r < data.Length; r++)
            {
                TableRow DataRow = table.Rows[r + 1];
                DataRow.Height = 15;
                for (int c = 0; c < data[r].Length; c++)
                {
                    DataRow.Cells[c].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                    Paragraph p2 = DataRow.Cells[c].AddParagraph();
                    TextRange TR2 = p2.AppendText(data[r][c]);
                    p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    TR2.CharacterFormat.FontName = "Calibri";
                    TR2.CharacterFormat.FontSize = 10;
                    TR2.CharacterFormat.TextColor = System.Drawing.Color.Black;
                }
            }
            string fileName = "Отчёт от " + DateTime.Now.ToString().Replace(":","-").Replace(" ", "/");
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + @"\ReportsFolder\" + fileName + ".docx";
            doc.SaveToFile(filePath);
            var result = MessageBox.Show("Отчёт создан и находится по пути \n" + filePath + "\nНахмите «Да» для открытия", "Результат", MessageBoxButton.YesNoCancel);
            if(result==MessageBoxResult.Yes)
                System.Diagnostics.Process.Start(filePath);
        }
    }
}
