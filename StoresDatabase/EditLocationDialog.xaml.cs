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
using System.Data.SQLite;
using System.Data;

namespace StoresDatabase
{
    /// <summary>
    /// Interaction logic for EditLocationDialog.xaml
    /// </summary>
    public partial class EditLocationDialog : Window
    {
        SQLiteConnection database;
        DataTable  dt_locations;
        ArrayList places;

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

        // new constructor ro pass database 
        public EditLocationDialog(SQLiteConnection db)
        {
            InitializeComponent();
            database = db;
            dt_locations = new DataTable();
            iDTBox.Text = "";
            nameTBox.Text = "";
            typeTBox.Text = "";
            //groupFkCombobox.ItemsSource = withinlocations;
            //groupFkCombobox.SelectedIndex = index;
            // populate the datagrid with current locations
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;
            sql_string = "SELECT * FROM Locations";
            sql_cmd.CommandText = sql_string;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
            adapter.Fill(dt_locations);
            locationdataGrid.ItemsSource = dt_locations.DefaultView;
            places = new ArrayList();
            foreach (DataRow r in dt_locations.Rows)
            {
                places.Add(r[1].ToString());
            }
            groupFkCombobox.ItemsSource = places;
        }


        public string Answer
        {
            get
            {
                return iDTBox.Text +"," + nameTBox.Text +"," + typeTBox.Text +","+ groupFkCombobox.SelectedIndex.ToString() ;
            }
        }

        private void OK_Btn_Click(object sender, RoutedEventArgs e)
        {
            // remove this 
           //  DialogResult = true;
            // change this to process the values in the textboxes
            // if the TBox is empty then this is a new entry else it is an update
            if (iDTBox.Text == String.Empty)
            {
                // this is a new entry 
                // add to location table
                MessageBox.Show("This is a new entry ");

            }
            else
            {
                // this is an update
                int index = Int32.Parse("0" + iDTBox.Text);
                if (index > 0)
                {
                    // update the record at index 
                    MessageBox.Show("This is an update on entry " + index);
                }
            }

            // clear the entries in the textboxes
            iDTBox.Text = String.Empty;
            nameTBox.Text = String.Empty;
            typeTBox.Text = String.Empty;
            groupFkCombobox.SelectedIndex = -1;
            OK_Btn.Content = "Add New";
            // default, with empty textboxes , is Add new record
        }

        private void locationdataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selected_row = locationdataGrid.SelectedIndex ;
            foreach (DataRowView drv in locationdataGrid.SelectedItems)
            {
                MessageBox.Show("This  " + selected_row + "  " + drv[0] + "  " + drv[1] + "  " + drv[2]);
                iDTBox.Text = drv[0].ToString();
                nameTBox.Text = drv[1].ToString();
                typeTBox.Text = drv[2].ToString();
                String groupStr = drv[3].ToString();
                int groupindex = Int32.Parse("0" + groupStr);
                if (groupindex > 0)
                {
                    groupFkCombobox.SelectedIndex = groupindex - 1;
                }
                OK_Btn.Content = "Update";
           }
            

        }

        private void delete_Btn_Click(object sender, RoutedEventArgs e)
        {
            int index = Int32.Parse("0" + iDTBox.Text);
            if (index > 0)
            {
                // delete the record at index 
                iDTBox.Text = String.Empty;
                nameTBox.Text = String.Empty;
                typeTBox.Text = String.Empty;
                groupFkCombobox.SelectedIndex = -1;
                OK_Btn.Content = "Add New";
                MessageBox.Show("The record at "  + index+ " will be deleted");
            }
        }

        
}



    
}
