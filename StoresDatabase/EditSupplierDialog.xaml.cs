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
        // default can be used for new supplier as nothing to pass in 
        public EditSupplierDialog()
        {
            InitializeComponent();
        }

        // constructor for use for edit supplier
        public EditSupplierDialog(int ID, String Name, String address, String Web, String email, String Phone)
        {
            InitializeComponent();
            iDTBox.Text = ID.ToString();
            nameTBox.Text = Name;
            addressTBox.Text = address;
            websiteTBox.Text = Web;
            emailTBox.Text = email;
            telphoneTBox.Text = Phone;

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
