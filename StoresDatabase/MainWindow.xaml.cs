using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoresDatabase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog openFileDlg;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileOpen_Btn_Click(object sender, RoutedEventArgs e)
        {
            // OPen a file and read the contents
            // Firstly need a Openfile dialog
            openFileDlg = new OpenFileDialog();
            openFileDlg.FileOk += openCSVFile;
            openFileDlg.Title = "Open File";
            openFileDlg.Filter = "CSV Files(*.csv)|*.csv|Text file(*.txt)|*.txt|All Files(*.*)|*.*";

            openFileDlg.ShowDialog();

        }

        private void openTextFile(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string fullPathname = openFileDlg.FileName;
            Paragraph para;
            FileInfo src = new FileInfo(fullPathname);

            string line;
            
           TextReader reader = src.OpenText();

            // read first line containing the name of event
            line = reader.ReadLine();
            
            while (line != null)
            {
                para = new Paragraph();
                para.Inlines.Add(line + '\n'+'\r');
                string[] fields = line.Split(' ');
                foreach (string f in fields)
                {
                    para.Inlines.Add(f + '\n');
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);
                // read next line
                line = reader.ReadLine();

            }
            reader.Close();

        }

        private void openCSVFile(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string fullPathname = openFileDlg.FileName;
            Paragraph para;
            FileInfo src = new FileInfo(fullPathname);

            string line;

            TextReader reader = src.OpenText();

            // read first line containing the name of event
            line = reader.ReadLine();

            while (line != null)
            {
                para = new Paragraph();
                para.Inlines.Add(line + '\n' + '\r');
                string[] fields = line.Split(',');
                foreach (string f in fields)
                {
                    para.Inlines.Add(f + '\n');
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);
                // read next line
                line = reader.ReadLine();

            }
            reader.Close();

        }
    }
}
