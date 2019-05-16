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
                DataTable dt_found = new DataTable();
                SQLiteCommand sql_cmd = database.CreateCommand();
                String sql_string;
                sql_string = "SELECT * FROM Item WHERE LocationFK = '" + found + "'; ";
                sql_cmd.CommandText = sql_string;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
                adapter.Fill(dt_found);
                itemDataGrid.ItemsSource = dt_found.DefaultView;
            }
        }

        private void typeSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            // Shows only those items of the selected type
            //the index from the combobox is 0 .. n-1. The index in database is 1..n
            // by adding 1 to the index we get the correct itemtype in database
            int found = typeComboBox.SelectedIndex + 1;
            if (found > 0)
            {
                DataTable dt_found = new DataTable();
                SQLiteCommand sql_cmd = database.CreateCommand();
                String sql_string;
                sql_string = "SELECT * FROM Item WHERE PartTypeFK = '" + found + "'; ";
                sql_cmd.CommandText = sql_string;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
                adapter.Fill(dt_found);
                itemDataGrid.ItemsSource = dt_found.DefaultView;
            }
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
