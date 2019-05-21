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
using System.Data.SQLite;
using System.Data;
using System.Collections;

namespace StoresDatabase
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        SQLiteConnection database;
        DataTable dt_items, dt_locations, dt_types, dt_suppliers;
        ArrayList places, itemtypes, suppliers;

        

        public SearchWindow()
        {
            InitializeComponent();
        }

        

        public SearchWindow(SQLiteConnection db)
        {
            InitializeComponent();
            database = db;
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;
            dt_items = new DataTable();
            sql_string = "SELECT *  FROM Item";
            sql_cmd.CommandText = sql_string;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
            adapter.Fill(dt_items);
            itemDataGrid.ItemsSource = dt_items.DefaultView;

            // get locations
            places = new ArrayList();
            dt_locations = new DataTable();
            sql_string = "SELECT *  FROM Locations";
            sql_cmd.CommandText = sql_string;
            adapter = new SQLiteDataAdapter(sql_cmd);
            adapter.Fill(dt_locations);
            foreach (DataRow r in dt_locations.Rows)
            {
                places.Add(r[1]);
            }
            locationComboBox.ItemsSource = places;

            // do same for type and suppliers
            itemtypes = new ArrayList();
            dt_types = new DataTable();
            sql_string = "SELECT * FROM ItemType";
            sql_cmd.CommandText = sql_string;
            adapter = new SQLiteDataAdapter(sql_cmd);
            adapter.Fill(dt_types);
            foreach (DataRow r in dt_types.Rows)
            {
                itemtypes.Add(r[1]);
            }
            typeComboBox.ItemsSource = itemtypes;

            //  and suppliers
            suppliers = new ArrayList();
            dt_suppliers = new DataTable();
            sql_string = "SELECT * FROM Suppliers";
            sql_cmd.CommandText = sql_string;
            adapter = new SQLiteDataAdapter(sql_cmd);
            adapter.Fill(dt_suppliers);
            foreach (DataRow r in dt_suppliers.Rows)
            {
                suppliers.Add(r[1]);
            }
            supplierComboBox.ItemsSource = suppliers;
        }

        

        private void search_Btn_Click(object sender, RoutedEventArgs e)
        {
            // Searches the Item.Name field and the Ite.Description fieldfor any occurrence 
            // of the search term entered into the search textbox. 
            // using like with the % character allows matches so will return rows which contain the
            // search term anywhere in the fields. COLLATE NOCASE will match any case. 
            String searchitem = searchTBox.Text;
            if (searchitem != "")
            {
                DataTable dt_found = new DataTable();
                // itemDataGrid.Items.Clear();
                SQLiteCommand sql_cmd = database.CreateCommand();
                String sql_string;
                sql_string = "SELECT * FROM Item WHERE Name LIKE '%" + searchitem +
                    "%'  COLLATE NOCASE OR Description LIKE '%" + searchitem + "%'; ";
                sql_cmd.CommandText = sql_string;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
                adapter.Fill(dt_found);
                itemDataGrid.ItemsSource = dt_found.DefaultView;
            }
        }

        private void locationSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            // Shows only those items in the selected location
            //the index from the combobox is 0 .. n-1. The index in database is 1..n
            // by adding 1 to the index we get the correct locations in database
            int found = locationComboBox.SelectedIndex + 1;
            if (found > 0)
            
            {
                DataTable dt_found = retrieveItemsbyLocation(found);
                //SQLiteCommand sql_cmd = database.CreateCommand();
                //String sql_string;
                //sql_string = "SELECT * FROM Item WHERE LocationFK = '" + found + "'; ";
                //sql_cmd.CommandText = sql_string;
                //SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
                //adapter.Fill(dt_found);
                itemDataGrid.ItemsSource = dt_found.DefaultView;
            }
        }

        private DataTable retrieveItemsbyLocation(int i)
        {
            DataTable cummulative = new DataTable();
            DataTable dt_retrievelocations = new DataTable();
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
            sql_string = "SELECT * FROM Item WHERE LocationFK = '" + i + "'; ";
            sql_cmd.CommandText = sql_string;
            adapter.Fill(cummulative);

            // need to check for any location which refers back to this location
            sql_string = "SELECT * FROM Locations WHERE LocationFK = '" + i + "'; ";
            sql_cmd.CommandText = sql_string;
            adapter.Fill(dt_retrievelocations);
            // if there are any locations returned then call this method recursively to get the item records
            if (dt_retrievelocations.Rows.Count != 0)
               
            {
                DataTable temp = new DataTable();
                foreach (DataRow r in dt_retrievelocations.Rows)
                {
                    temp = retrieveItemsbyLocation(Int32.Parse(r[0].ToString()));
                    foreach (DataRow dr in temp.Rows)
                    {
                        cummulative.ImportRow(dr);
                    }
                }
            }
            return cummulative;
        }

        private void typeSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            // Shows only those items of the selected type
            //the index from the combobox is 0 .. n-1. The index in database is 1..n
            // by adding 1 to the index we get the correct itemtype in database
            int found = typeComboBox.SelectedIndex + 1;
            if (found > 0)
            {
                DataTable dt_found = retrieveItemsbyType(found);
                //SQLiteCommand sql_cmd = database.CreateCommand();
                //String sql_string;
                //sql_string = "SELECT * FROM Item WHERE PartTypeFK = '" + found + "'; ";
                //sql_cmd.CommandText = sql_string;
                //SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
                //adapter.Fill(dt_found);
                itemDataGrid.ItemsSource = dt_found.DefaultView;
            }
        }

        private DataTable retrieveItemsbyType(int i)
        {
            // MessageBox.Show("retrieving from location" + i);
            DataTable cummulstive = new DataTable();
            DataTable dt_retrievetypes = new DataTable();
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);

            // firstly fill table with records which link to this type
            sql_string = "SELECT * FROM Item WHERE PartTypeFK = '" + i + "'; ";
            sql_cmd.CommandText = sql_string;
            adapter.Fill(cummulstive);
                        
            // then get the type records which link to the type record ( if any )
            sql_string = "SELECT * FROM ItemType WHERE TypeGroup = '" + i + "'; ";
            sql_cmd.CommandText = sql_string;
            
            adapter.Fill(dt_retrievetypes);
            DataTable temp = new DataTable();
            // if there are other type records which link to this one then retrieve records from them
            // i.e. this calls the current methods recursively
            if (dt_retrievetypes.Rows.Count != 0)
            {
                foreach (DataRow r in dt_retrievetypes.Rows)
                    temp = retrieveItemsbyType(Int32.Parse(r[0].ToString()));
                foreach (DataRow dr in temp.Rows)
                {
                    cummulstive.ImportRow(dr);
                }
            }
            
            return cummulstive;
        }

        private void supplierSelect_Btn_Click(object sender, RoutedEventArgs e)
        {
            // Shows only those items from  the selected supplier
            //the index from the combobox is 0 .. n-1. The index in database is 1..n
            // by adding 1 to the index we get the correct supplier in database
            int found = supplierComboBox.SelectedIndex + 1;
            if (found > 0)
            {
                DataTable dt_found = new DataTable();
                SQLiteCommand sql_cmd = database.CreateCommand();
                String sql_string;
                sql_string = "SELECT * FROM Item WHERE SupplierFK = '" + found + "'; ";
                sql_cmd.CommandText = sql_string;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
                adapter.Fill(dt_found);
                itemDataGrid.ItemsSource = dt_found.DefaultView;
            }
        }

        private void all_Btn_Click(object sender, RoutedEventArgs e)
        {
           // 'dt_items' is populated in windows constructor so only need to set as ItemSource
            itemDataGrid.ItemsSource = dt_items.DefaultView;
        }

        private void close_Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
