using System;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;

namespace KurortApp
{
    class ReportMaker
    {
        public static string MakeReportMethod(String[] Header, String[][] data, string reportType)
        {
            Document doc = new Document();

            Spire.Doc.Section s = doc.AddSection();
            s.AddParagraph().AppendText(reportType + ":").CharacterFormat.LocaleIdASCII = 1049;
            s.AddParagraph().AppendText("");

            Table table = s.AddTable(true);
            table.ResetCells(data.Length + 1, Header.Length);
            Spire.Doc.TableRow FRow = table.Rows[0];
            FRow.IsHeader = true;
            FRow.Height = 30;
            FRow.RowFormat.BackColor = System.Drawing.Color.LightGray;
            for (int i = 0; i < Header.Length; i++)
            {
                Paragraph p = FRow.Cells[i].AddParagraph();
                FRow.Cells[i].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                p.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                TextRange TR = p.AppendText(Header[i]);
                TR.CharacterFormat.LocaleIdASCII = 1049;
                TR.CharacterFormat.FontName = "Calibri";
                TR.CharacterFormat.FontSize = 12;
                TR.CharacterFormat.TextColor = System.Drawing.Color.White;
                TR.CharacterFormat.Bold = true;
            }
            for (int r = 0; r < data.Length; r++)
            {
                TableRow DataRow = table.Rows[r + 1];
                DataRow.Height = 20;
                for (int c = 0; c < data[r].Length; c++)
                {
                    DataRow.Cells[c].CellFormat.VerticalAlignment = Spire.Doc.Documents.VerticalAlignment.Middle;
                    Paragraph p2 = DataRow.Cells[c].AddParagraph();
                    TextRange TR2 = p2.AppendText(data[r][c]);
                    p2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center;
                    TR2.CharacterFormat.LocaleIdASCII = 1049;
                    TR2.CharacterFormat.FontName = "Calibri";
                    TR2.CharacterFormat.FontSize = 10;
                    TR2.CharacterFormat.TextColor = System.Drawing.Color.Black;
                }
            }
            string fileName = reportType + @"/Отчёт от " + DateTime.Now.ToString().Replace(":", "-");
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + @"\ReportsFolder\" + fileName + ".docx";
            doc.SaveToFile(filePath);
            return filePath;
        }
    }
}
