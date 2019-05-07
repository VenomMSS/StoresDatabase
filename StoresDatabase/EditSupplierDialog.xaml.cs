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

namespace StoresDatabase
{
    /// <summary>
    /// Interaction logic for EditSupplierDialog.xaml
    /// </summary>
    public partial class EditSupplierDialog : Window
    {
        public EditSupplierDialog()
        {
            InitializeComponent();
        }

        public string Answer
        {
            get
            {
                return iDTBox.Text + "," + nameTBox.Text + "," + 
                    addressTBox.Text+ "," + websiteTBox.Text +"," + emailTBox.Text +"," +
                    telphoneTBox.Text;
            }
        }

        private void OK_Btn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
