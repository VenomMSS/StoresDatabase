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
    /// Interaction logic for EditItemDialog.xaml
    /// </summary>
    public partial class EditItemDialog : Window
    {
        // Database tables
        private static String table_parts = "Item";
        private static String table_parttype = "ItemType";
        private static String table_location = "Locations";
        private static String table_supplier = "Suppliers";

        // table_part fields
        private static String field_partName = "Name";
        private static String field_PartDescription = "Description";
        private static String field_SupplierPartNo = "PartNo";
        private static String field_stock = "Amount";
        private static String field_partUnit = "Unit";
        private static String field_price = "Price";
        private static String field_currency = "Currency";
        private static String field_partTypeFK = "PartTypeFK";
        private static String field_LocFK = "LocationFK";
        private static String field_SuppFK = "SupplierFK";
        private static String field_status = "Status";

        SQLiteConnection database;
        DataTable dt_items;
        ItemNameClass places, types, suppliers;
        ArrayList placelist, typelist, supplierlist;

        public EditItemDialog()
        {
            InitializeComponent();
            // these all need default values for stop crash when answer .get is called
                      
        }

        public EditItemDialog(ArrayList locs, ArrayList types, ArrayList sups)
        {
            // This constructor is called to add a new item. 
            // only lists lassed in passed in
            InitializeComponent();
            locComboBox.ItemsSource = locs;
            typeComboBox.ItemsSource = types;
            supplierComboBox.ItemsSource = sups;
            
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

        // constructo to take database reference
        public EditItemDialog(SQLiteConnection db)
        {
            InitializeComponent();
            database = db;
            clear();
            LoadDataGrid();

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
            // DialogResult = true;
            SQLiteCommand sqlCmd = new SQLiteCommand(database);
            String cmd_String;
            int loc_selected, type_selected, supplier_selected;
            int loc_index, typ_index, sup_index;
            ItemNameClass selected;
            String status = "OOS";
            int stock =0;

            loc_selected = locComboBox.SelectedIndex;
            type_selected = typeComboBox.SelectedIndex;
            supplier_selected = supplierComboBox.SelectedIndex; 
            try
            {
                stock = Int32.Parse("0" + amountTBox.Text);
            }
            catch (Exception es)
            {
                stock = 0;
            }

            if (stock > 0)
            {
                status = "In Stock";
            }

            if (iDTBox.Text == String.Empty)
            {
                // this is a new entry
                // produce the common part of command string for an insert
                 cmd_String = "INSERT INTO " + table_parts + " (" + field_partName + ", " +
                     field_PartDescription + ", " + field_partUnit + ", " + field_SupplierPartNo + ", " +
                     field_stock + ", " + field_price + ", " + field_currency + ", " + field_status;
                if (loc_selected == -1)
                {
                    // !location 
                    if (type_selected == -1)
                    {
                        // there is !location or !type 
                        if (supplier_selected == -1)
                        {
                            // there is  !location !type !supplier 
                            cmd_String = cmd_String + ") VALUES ('" +
                                nameTBox.Text + "', '" + descTBox.Text + "', '" + unitTBox.Text + "', '" + partnoTBox.Text + "', '" +
                                amountTBox.Text + "', '" + priceTBox.Text + "', '" + currencyTBox.Text + "', '" + status + "', '" +
                                 "' );";
                        }
                        else
                        {
                            // there is  !location  !type but there is a SUPPLIER
                            selected = (ItemNameClass)supplierlist[supplier_selected];
                            sup_index = selected.getID();
                            cmd_String = cmd_String + ", " + field_SuppFK + ") VALUES ('" +
                                nameTBox.Text + "', '" + descTBox.Text + "', '" + unitTBox.Text + "', '" + partnoTBox.Text + "', '" +
                                amountTBox.Text + "', '" + priceTBox.Text + "', '" + currencyTBox.Text + "', '" + status + "', '" +
                                sup_index + "' );";
                        }
                    }
                    else
                    {
                        // there is  !location, there is TYPE selected
                        selected = (ItemNameClass)typelist[type_selected];
                        typ_index = selected.getID();
                        if (supplier_selected == -1)
                        {
                            // there is  !location  !supplier there is TYPE  selected
                            cmd_String = cmd_String + ", " + field_partTypeFK + ") VALUES ('" +
                                nameTBox.Text + "', '" + descTBox.Text + "', '" + unitTBox.Text + "', '" + partnoTBox.Text + "', '" +
                                amountTBox.Text + "', '" + priceTBox.Text + "', '" + currencyTBox.Text + "', '" + status + "', '" +
                                typ_index + "' );";
                        }
                        else
                        {
                            // there is !location  but there is TYPE and SUPPLIER selected
                            selected = (ItemNameClass)supplierlist[supplier_selected];
                            sup_index = selected.getID();
                            cmd_String = cmd_String + ", " +  field_partTypeFK + ", " + field_SuppFK + ") VALUES ('" +
                                nameTBox.Text + "', '" + descTBox.Text + "', '" + unitTBox.Text + "', '" + partnoTBox.Text + "', '" +
                                amountTBox.Text + "', '" + priceTBox.Text + "', '" + currencyTBox.Text + "', '" + status + "', '" +
                                typ_index + "', '" + sup_index + "' );";
                        }
                    }
                }
                else
                {
                    // there is a LOCATION selected
                    selected = (ItemNameClass)placelist[loc_selected];
                    loc_index = selected.getID();
                    if (type_selected == -1)
                    {
                        // there is a LOCATION  selected but !type
                        if (supplier_selected == -1)
                        {
                            // !supplier or !type but is LOCATION
                            cmd_String = cmd_String + ", " + field_LocFK +  ") VALUES ('" +
                                nameTBox.Text + "', '" + descTBox.Text + "', '" + unitTBox.Text + "', '" + partnoTBox.Text + "', '" +
                                amountTBox.Text + "', '" + priceTBox.Text + "', '" + currencyTBox.Text + "', '" + status + "', '" +
                                loc_index +  "' );";
                        }
                        else
                        {
                            // there is a LOCATION AND SUPPLIER   but !type
                            selected = (ItemNameClass)supplierlist[supplier_selected];
                            sup_index = selected.getID();
                            cmd_String = cmd_String + ", " + field_LocFK + ", " + field_SuppFK + ") VALUES ('" +
                                nameTBox.Text + "', '" + descTBox.Text + "', '" + unitTBox.Text + "', '" + partnoTBox.Text + "', '" +
                                amountTBox.Text + "', '" + priceTBox.Text + "', '" + currencyTBox.Text + "', '" + status + "', '" +
                                loc_index + "', '" + sup_index + "' );";
                        }

                    }
                    else
                    {
                        // there is LOCATION ,and TYPE selected
                        selected = (ItemNameClass)typelist[type_selected];
                        typ_index = selected.getID();
                        if (supplier_selected == -1)
                        {
                            //  LOCATION AND TYPE no !supplier 
                            cmd_String = cmd_String + ", " + field_LocFK + ", " + field_partTypeFK + ") VALUES ('" +
                                nameTBox.Text + "', '" + descTBox.Text + "', '" + unitTBox.Text + "', '" + partnoTBox.Text + "', '" +
                                amountTBox.Text + "', '" + priceTBox.Text + "', '" + currencyTBox.Text + "', '" + status + "', '" +
                                loc_index + "', '" + typ_index +  "' );";
                        }
                        else
                        {
                            // there is a LOCATION TYPE AND SUPPLIER
                            selected = (ItemNameClass)supplierlist[supplier_selected];
                            sup_index = selected.getID();
                            cmd_String = cmd_String + ", " + field_LocFK + ", " + field_partTypeFK + ", " + field_SuppFK + ") VALUES ('" +
                                nameTBox.Text + "', '" + descTBox.Text + "', '" + unitTBox.Text + "', '" + partnoTBox.Text + "', '" +
                                amountTBox.Text + "', '" + priceTBox.Text + "', '" + currencyTBox.Text + "', '" + status + "', '" +
                                loc_index + "', '" + typ_index + "', '" + sup_index + "' );";
                        }
                    }
                }
                sqlCmd.CommandText = cmd_String;
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show(cmd_String,"INSERTION MADE");
                clear();
                LoadDataGrid();
            }

            else
            {
                // this is an update
                // produce the common part of command string for an update
                cmd_String = "UPDATE " + table_parts + " SET " + field_partName + " = '" + nameTBox.Text + "', " +
                    field_PartDescription + " = '" + descTBox.Text + "', " + field_partUnit + " = '" + unitTBox.Text + "', " + 
                    field_SupplierPartNo + " = '" + partnoTBox.Text +"', " + field_stock + " = '" + amountTBox.Text +"', " + 
                    field_price + " = '" + priceTBox.Text +"', " + field_currency + " = '" + currencyTBox.Text +"', " +
                    field_status + " = '" + status + "'" ;
                if (loc_selected == -1)
                {
                    // !location 
                    if (type_selected == -1)
                    {
                        // there is !location or !type 
                        if (supplier_selected == -1)
                        {
                            // there is  !location !type !supplier
                            cmd_String = cmd_String +  ";";
                        }
                        else
                        {
                            // there is  !location  !type but there is a SUPPLIER
                            selected = (ItemNameClass)supplierlist[supplier_selected];
                            sup_index = selected.getID();
                            cmd_String = cmd_String + ", " +  field_SuppFK + " = '" + sup_index + "'";
                        }
                    }
                    else
                    {
                        // there is  !location, there is TYPE selected
                        selected = (ItemNameClass)typelist[type_selected];
                        typ_index = selected.getID();
                        if (supplier_selected == -1)
                        {
                            // there is  !location  !supplier there is TYPE  selected
                            cmd_String = cmd_String + ", "  +  field_partTypeFK + " = '" + typ_index + "'";
                        }
                        else
                        {
                            // there is !location  but there is TYPE and SUPPLIER selected
                            selected = (ItemNameClass)supplierlist[supplier_selected];
                            sup_index = selected.getID();
                            cmd_String = cmd_String + ", " + 
                              field_partTypeFK + " = '" + typ_index + "', " + field_SuppFK + " = '" + sup_index + "'";
                        }
                    }
                }
                else
                {
                    // there is a LOCATION selected
                    selected = (ItemNameClass)placelist[loc_selected];
                    loc_index = selected.getID();
                    if (type_selected == -1)
                    {
                        // there is a LOCATION  selected but !type
                        if (supplier_selected == -1)
                        {
                            // !supplier or !type but is LOCATION
                            cmd_String = cmd_String + ", " + field_LocFK + " = '" + loc_index +  "'";
                        }
                        else
                        {
                            // there is a LOCATION AND SUPPLIER   but !type
                            selected = (ItemNameClass)supplierlist[supplier_selected];
                            sup_index = selected.getID();
                            cmd_String = cmd_String + ", " + field_LocFK + " = '" + loc_index + "', " +
                               field_SuppFK + " = '" + sup_index + "'";
                        }

                    }
                    else
                    {
                        // there is LOCATION ,and TYPE selected
                        selected = (ItemNameClass)typelist[type_selected];
                        typ_index = selected.getID();
                        if (supplier_selected == -1)
                        {
                            //  LOCATION AND TYPE no !supplier 
                            cmd_String = cmd_String + ", " + field_LocFK + " = '" + loc_index + "', " +
                              field_partTypeFK + " = '" + typ_index +  "'";
                        }
                        else
                        {
                            // there is a LOCATION TYPE AND SUPPLIER
                            selected = (ItemNameClass)supplierlist[supplier_selected];
                            sup_index = selected.getID();
                            cmd_String = cmd_String + ", " + field_LocFK + " = '" + loc_index + "', " +
                                field_partTypeFK + " = '" + typ_index + "', " + field_SuppFK + " = '" + sup_index + "'";
                        }
                    }
                }
                cmd_String = cmd_String + " WHERE partID = '" + Int32.Parse( iDTBox.Text) + "';";
                sqlCmd.CommandText = cmd_String;
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show(cmd_String, "UPDATED");
                clear();
                LoadDataGrid();
            }

        }
       

        private void undoBtn_Click(object sender, RoutedEventArgs e)
        {
            clear();
            OK_btn.Content = "Add New";
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // delete current item from items table
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;
            int index = Int32.Parse("0" + iDTBox.Text);
            if (index > 0)
            {
                // comfirm deletion
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete record " + index + " "+ nameTBox.Text, "DELETION",MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    // comfirmed to delete
                    sql_string = "DELETE FROM " + table_parts + " WHERE partID = '" + index + "';";
                    sql_cmd.CommandText = sql_string;
                    sql_cmd.ExecuteNonQuery();
                    LoadDataGrid();
                    clear();
                }
            }
        }

        private void itemsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (DataRowView drv in itemsDataGrid.SelectedItems)
            {
                iDTBox.Text = drv[0].ToString();
                nameTBox.Text = drv[1].ToString();
                descTBox.Text = drv[2].ToString();
                unitTBox.Text = drv[3].ToString();
                partnoTBox.Text = drv[4].ToString();
                amountTBox.Text = drv[5].ToString();
                priceTBox.Text = drv[6].ToString();
                currencyTBox.Text = drv[7].ToString();
                
                int loc = getIndex(drv[8].ToString(), placelist);
                locComboBox.SelectedIndex = loc;
                int typ = getIndex(drv[9].ToString(), typelist);
                typeComboBox.SelectedIndex = typ ;
                int sup = getIndex(drv[10].ToString(), supplierlist);
                supplierComboBox.SelectedIndex = sup;
                statusTBox.Text = drv[11].ToString();
                OK_btn.Content = "Update";
            }
        }

        private void LoadDataGrid()
        {
            SQLiteCommand sql_cmd = database.CreateCommand();
            sql_cmd.CommandText = "SELECT * FROM Item";
            dt_items = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
            adapter.Fill(dt_items);
            itemsDataGrid.ItemsSource = dt_items.DefaultView;
            
            // need to also load location, suppliers and itemtypes
            sql_cmd.CommandText = "SELECT * FROM Locations";
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            placelist = new ArrayList();
            while (reader.Read())
            {
                // placelist.Add(reader.GetString(1));
                places = new ItemNameClass(reader.GetInt32(0), reader.GetString(1));
                placelist.Add( places);
            }
            locComboBox.ItemsSource = placelist;
            reader.Close();

            // need to also load  itemtypes
            sql_cmd.CommandText = "SELECT * FROM ItemType";
            reader = sql_cmd.ExecuteReader();
            typelist = new ArrayList();
            while (reader.Read())
            {
                types = new ItemNameClass(reader.GetInt32(0), reader.GetString(1));
                typelist.Add(types);
            }
            typeComboBox.ItemsSource = typelist;
            reader.Close();

            // need to also load  suppliers 
            sql_cmd.CommandText = "SELECT * FROM Suppliers";
            reader = sql_cmd.ExecuteReader();
            supplierlist = new ArrayList();
            while (reader.Read())
            {
                suppliers = new ItemNameClass(reader.GetInt32(0), reader.GetString(1));
                supplierlist.Add(suppliers);
            }
            supplierComboBox.ItemsSource = supplierlist;
            reader.Close();
        }

        private int getIndex(String lookfor, ArrayList inlist)
        {
            ItemNameClass look_at;
            int looking_for = 0;
            int i = 0;
            bool found = false;
            int foundindex = -1;
            try
            {
                looking_for = Int32.Parse("0" + lookfor);
            }
            catch (Exception e)
            {
                looking_for = -1;
            }
                        
            if (looking_for > 0)
            {
                while (i < inlist.Count && found == false)
                {
                    look_at = (ItemNameClass)inlist[i];
                    if (look_at.getID() == looking_for)
                    {
                        found = true;
                        foundindex = i;
                    }
                    i++;
                }
            }
            
            return foundindex;
        }

        private void clear()
        {
            iDTBox.Text = String.Empty;
            nameTBox.Text = String.Empty;
            amountTBox.Text = String.Empty;
            descTBox.Text = String.Empty;
            currencyTBox.Text = String.Empty;
            partnoTBox.Text = String.Empty;
            priceTBox.Text = String.Empty;
            statusTBox.Text = String.Empty;
            unitTBox.Text = String.Empty;
            locComboBox.SelectedIndex = -1;
            typeComboBox.SelectedIndex = -1;
            supplierComboBox.SelectedIndex = -1;
            OK_btn.Content = "Add New";
        }
    }
}
