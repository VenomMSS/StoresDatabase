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
using System.Collections;

namespace StoresDatabase
{
    /// <summary>
    /// Interaction logic for EditLocationDialog.xaml
    /// </summary>
    public partial class EditLocationDialog : Window
    {
        public EditLocationDialog()
        {
            InitializeComponent();
        }

        // constructor for new loaction
        public EditLocationDialog(ArrayList withinlocations)
        { 
            InitializeComponent();
            groupFkCombobox.ItemsSource = withinlocations;
        }

        // constructor for edit loaction
        public EditLocationDialog(int id, String Name, String loctype, int index, ArrayList withinlocations)
        {
            InitializeComponent();
            iDTBox.Text = id.ToString();
            nameTBox.Text = Name;
            typeTBox.Text = loctype;
            groupFkCombobox.ItemsSource = withinlocations;
            groupFkCombobox.SelectedIndex = index;

        }


        public string Answer
        {
            get
            {
                return iDTBox.Text +"," + nameTBox.Text +"," + typeTBox +","+ groupFkCombobox.SelectedIndex.ToString() ;
            }
        }

        private void OK_Btn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }



    
}
