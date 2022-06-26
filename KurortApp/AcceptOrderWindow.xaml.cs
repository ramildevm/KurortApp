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
    /// Логика взаимодействия для AcceptOrderWindow.xaml
    /// </summary>
    public partial class AcceptOrderWindow : Window
    {
        System.Windows.Threading.DispatcherTimer sessionTimer;
        TimeSpan _time;
        public AcceptOrderWindow()
        {
            InitializeComponent();
            profileImage.Source = new BitmapImage(new Uri("ResoursesFolder/" + SessionContext.CurrentUser.FIO.Split(' ')[0] + ".jpg", UriKind.Relative));
            SetTimers();
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
        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {
            var smw = new SellerMainWindow();
            this.Close();
            smw.ShowDialog();
        }

        private void ButtonChange_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderData(false);
        }

        private void ButtonAcceptOrder_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderData();
        }

        private void ChangeOrderData(bool isAcceptOption = true)
        {
            string code = txtBarcode.Text;
            if (code.Replace(" ", "") == "" || code.Replace(" ", "").Length < 13)
            {
                MessageBox.Show("Некорректный штрих-код!");
                return;
            }
            using (var db = new KurortDBEntities())
            {
                var barcode = db.Barcodes.Where(b => b.Barcode.Substring(b.Barcode.Length - 13) == txtBarcode.Text).FirstOrDefault();
                if (barcode is null)
                {
                    MessageBox.Show("Штрих-код не действителен!");
                    return;
                }
                Orders order = db.Orders.Find(barcode.OrderId);
                if (isAcceptOption)
                {
                    if(order.Status == "Закрыто")
                    {
                        MessageBox.Show("Заказ уже принят!");
                        return;
                    }
                    order.CloseDate = DateTime.Now.Date;
                    order.Status = "Закрыто";

                    foreach (var s in order.Services.Split(new string[] {","}, StringSplitOptions.None))
                    {
                        int sId = Convert.ToInt32(s);
                        var goods = (from sg in db.ServiceGoods
                                     where sg.ServiceId == sId
                                     select sg.Goods).FirstOrDefault();
                        if (goods is null)
                            continue;
                        if(goods.InPrentCount>0) 
                            goods.InPrentCount--;
                        db.Entry(goods).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                else
                {
                    if (order.Status != "Новая")
                    {
                        MessageBox.Show("Статус уже изменён!");
                        return;
                    }
                    order.Status = "В прокате";
                }
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                txtBarcode.Text = "";
                MessageBox.Show((isAcceptOption)?"Заказ принят.":"Статус изменен");
            }
        }
    }
}
