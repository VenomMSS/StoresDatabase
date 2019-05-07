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
    /// Interaction logic for EditItemDialog.xaml
    /// </summary>
    public partial class EditItemDialog : Window
    {
        public EditItemDialog()
        {
            InitializeComponent();
            // these all need default values for stop crash when answer .get is called
            iDTBox.Text = "0";
            nameTBox.Text = "Name";
            descTBox.Text = "Description";
            unitTBox.Text = "Unit size";
            partnoTBox.Text = "PrtNo";
            amountTBox.Text = "Amnt";
            priceTBox.Text = "Price";
            currencyTBox.Text = "Cur";
            statusTBox.Text = "Sts";
            
        }

        public EditItemDialog(int id, String nm, String desc, String Un, String PrtNo,
            int Amnt, double Prce, String Cur, String Sts, int lFK, int tFK, int sFK,
            ArrayList locs, ArrayList types, ArrayList sups)
        {
            // This constructor is called when editing an existing item. 
            // All data passed in but id and status are never editable.
            InitializeComponent();
            iDTBox.Text = id.ToString();
            nameTBox.Text = nm;
            descTBox.Text = desc;
            unitTBox.Text = Un;
            partnoTBox.Text = PrtNo;
            amountTBox.Text = Amnt.ToString();
            priceTBox.Text = Prce.ToString();
            currencyTBox.Text = Cur;
            statusTBox.Text = Sts;
            locComboBox.ItemsSource = locs;
            typeComboBox.ItemsSource = types;
            supplierComboBox.ItemsSource = sups;
            locComboBox.SelectedIndex = lFK;
            typeComboBox.SelectedIndex = tFK;
            supplierComboBox.SelectedIndex = sFK;
        }

        public EditItemDialog(ArrayList locs, ArrayList types, ArrayList sups)
        {
            // this constructor called when a new items is being added
            // Passes in the location,type ansd supplier lists 

            InitializeComponent();
            // these all need default values for stop crash when answer .get is called
            iDTBox.Text = "#";
            nameTBox.Text = "Name";
            descTBox.Text = "Description";
            unitTBox.Text = "Unit size";
            partnoTBox.Text = "PrtNo";
            amountTBox.Text = "Amnt";
            priceTBox.Text = "Price";
            currencyTBox.Text = "Cur";
            statusTBox.Text = "unknown";
            locComboBox.ItemsSource = locs;
            typeComboBox.ItemsSource = types;
            supplierComboBox.ItemsSource = sups;
        }


        public string Answer
        {
            get
            {
                return iDTBox.Text + "," + nameTBox.Text + "," + descTBox.Text + "," + unitTBox.Text + ","
                    + partnoTBox.Text + "," + amountTBox.Text + "," + priceTBox.Text + "," + currencyTBox.Text 
                    + "," + statusTBox.Text + "," + locComboBox.SelectedIndex.ToString() + "," + 
                    typeComboBox.SelectedIndex.ToString() + "," + supplierComboBox.SelectedIndex.ToString(); 
            
            }
        }

        private void OK_btn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
