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
    /// Interaction logic for EditItemTypeDialog.xaml
    /// </summary>
    public partial class EditItemTypeDialog : Window
    {
        public EditItemTypeDialog()
        {
            InitializeComponent();
        }

        public string Answer
        {
            get
            {
                return iDTBox.Text + "," + typeNameTBox.Text + "," + typeGroupComboBox.SelectedIndex.ToString(); 
            }
        }

        private void OK_Btn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
