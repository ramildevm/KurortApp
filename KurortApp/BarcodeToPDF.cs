using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KurortApp
{
    class BarcodeToPDF
    {
        static public RenderTargetBitmap GetImage(Canvas sp_ports)
        {
            System.Windows.Size size = sp_ports.DesiredSize;
            if (size.IsEmpty)
                return null;

            RenderTargetBitmap result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

            result.Render(sp_ports);
            return result;
        }
        public static string SaveAsPng(RenderTargetBitmap src)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "PNG Files | *.png";
            dlg.DefaultExt = "png";
            if (dlg.ShowDialog() == true)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(src));
                using (var stream = dlg.OpenFile())
                {
                    encoder.Save(stream);
                }
            }
            return dlg.FileName;
        }
        public static string SaveImageAsPDF(string filePAth, string code)
        {
            var converter = new GroupDocs.Conversion.Converter(filePAth);
            // set the convert options for PDF format
            var convertOptions = converter.GetPossibleConversions()["pdf"].ConvertOptions;
            // convert to PDF format
            converter.Convert(System.AppDomain.CurrentDomain.BaseDirectory+@"\BarcodesFolder\" + code +".pdf", convertOptions);
            return System.AppDomain.CurrentDomain.BaseDirectory + @"\BarcodesFolder\";
        }
    }
}
