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
using System.Data.SQLite;
using System.Collections;
using StockDBClasses;
using System.Data;



namespace StoresDatabase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog openFileDlg;
        SQLiteConnection database;
        SQLiteDataAdapter partsAdapter, locationAdapter, supplierAdapter;
        String Connection;
        Item testItem,part, part2;
        Location location;
        Supplier supplier;
        ItemType partType;
        Order order;
        ArrayList places;
        ArrayList suppliers;
        ArrayList itemtype;
        Boolean DB_Connection = false;
        /// IMPORTANT SQLITE NOTE
        /// IT IS ABSOLUTELY ESSENTIAL THAT THE TABLE AND FIELD VALUE
        /// ONLY CONTAIN A-Z AND 1-0
        /// SPACES, PUNCTUATION ETC WILL CAUSE A FAIL IN CREATING THE TABLE

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

        // table_location fields
        private static String field_locName = "Location";
        private static String field_locType = "Type";
        private static String field_InLocationFK = "LocationFK";

        // table_supplier fields
        private static string field_SupName = "Supplier";
        private static string field_SupAddress = "Address";
        private static string field_Supwebsite = "Web";
        private static string field_Supemail = "Email";
        private static string field_SupTel = "Tel";

        // table_parttype fields
        private static String field_parttype = "ItemType";
        private static String field_typeGroupFK = "TypeGroup";



        public MainWindow()
        {
            InitializeComponent();
            
            // version number in connection string  is the SQLite version and needs to be set to 3.
            // Connection = "Data Source =c:\\Databases\\Stock.db;Version=3;New=True;Compress=True;";
            ///  database = new SQLiteConnection(Connection);
            //database.Open();

            disbleBtnMenu();

           //  CreateAllTables();
           // changed to only open/create database when user clicks appropriate menu item
        }


        private void NewDBase_Click(object sender, RoutedEventArgs e)
        {
            // create a new data base
            // need a dialog to get the database file name
            // or just use a save as dialog?
            DBFileNameDialog getDBName = new DBFileNameDialog();
            if (getDBName.ShowDialog() == true)
            {
                Paragraph para = new Paragraph();
                para.Inlines.Add(getDBName.Answer + '\n');
                ViewDoc.Blocks.Add(para);
            }

            // need to construct connection and then 
            Connection = "Data Source =c:\\Databases\\"+ getDBName.Answer+ ".db;Version=3;New=True;Compress=True;";
            database = new SQLiteConnection(Connection);
            database.Open();
            DB_Connection = true;
            CreateAllTables();
            enableBtnMenu();
        }

        private void OpenDBase_Click(object sender, RoutedEventArgs e)
        {
            // the open file dialog to get the name of the database
            openFileDlg = new OpenFileDialog();
            openFileDlg.FileOk += openDataBaseFile;
            openFileDlg.Title = "Open DataBase";
            openFileDlg.Filter = "db Files(*.db)|*.db|All Files(*.*)|*.*";
            openFileDlg.ShowDialog();
        }

        private void openDataBaseFile(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string fullPathname = openFileDlg.FileName;
            FileInfo src = new FileInfo(fullPathname);
            Paragraph para = new Paragraph();
            //para.Inlines.Add(src.DirectoryName + '\n');
            //para.Inlines.Add(src.FullName + '\n');
            para.Inlines.Add(src.Name + '\n');
            ViewDoc.Blocks.Add(para);
            Connection = "Data Source =c:\\Databases\\" + src.Name + ";Version=3;New=True;Compress=True;";
            database = new SQLiteConnection(Connection);
            database.Open();
            DB_Connection = true;
            CreateAllTables();
            // load data from all tables 
            enableBtnMenu();

        }

        private void FileOpen_Btn_Click(object sender, RoutedEventArgs e)
        {
            // OPen a file and read the contents
            // Firstly need a Openfile dialog
            openFileDlg = new OpenFileDialog();
            openFileDlg.FileOk += openToDBFile;
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

        private void openToDBFile(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string fullPathname = openFileDlg.FileName;
            Paragraph para;
            FileInfo src = new FileInfo(fullPathname);

            string line;
            TextReader reader = src.OpenText();
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;
            int loc_index, type_index, sup_index;
            line = reader.ReadLine();
            while (line != null)
            {
                // add to the database
                string[] fields = line.Split(',');
                loc_index = findLocation(fields[7]);
                type_index = findType(fields[8]);
                sup_index = findSupplier(fields[9]);
                sql_string = "INSERT INTO " + table_parts + " (" + field_partName + ", " +
                                field_PartDescription + ", " + field_partUnit + ", " + field_SupplierPartNo + ", " +
                                field_stock + ", " + field_price + ", " + field_currency + ", " +  field_LocFK + ", " + 
                                field_partTypeFK + ", " + field_SuppFK + ", " +  field_status + ") VALUES ('";
                sql_string = sql_string + fields[0] + "', '" + fields[1] + "', '" + fields[2] + "', '" + fields[3] + "', '" + 
                    fields[4] + "', '" + fields[5] + "', '" + fields[6] + "', '" + loc_index + "', '" + type_index +
                    "', '" +  sup_index + "', '" + fields[10] + "' );";
                sql_cmd.CommandText = sql_string;
                sql_cmd.ExecuteNonQuery();

                // write to the Flowdocument 
                para = new Paragraph();
                para.Inlines.Add(line + '\n' + '\r');
                int n = 0;
                foreach (string f in fields)
                {
                    para.Inlines.Add(" field# " + n + " = " + f + '\n');
                    n++;
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);
                
                
                
                // read next line
                line = reader.ReadLine();

            }
            reader.Close();

        }

        private void CreateAllTables()
        {
            // create all tables in database
            
            SQLiteCommand Cmd;
            Cmd = database.CreateCommand();

            // Create the parts table
            Cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_parts +
                " (partID integer primary key, " + field_partName + " TEXT, " +
                  field_PartDescription + " TEXT, " +  field_partUnit + " TEXT, " +
                  field_SupplierPartNo + " TEXT, " + field_stock + " INTEGER, " + 
                  field_price + " FLOAT, " + field_currency + " TEXT, " +
                  field_LocFK + " INTEGER, " + field_partTypeFK + " INTEGER, " +
                  field_SuppFK + " INTEGER, " + field_status + " TEXT);";
            Cmd.ExecuteNonQuery();

            // Create the location table
            Cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_location +
                " (locID integer primary key, " + field_locName + " TEXT, " +
                  field_locType + " TEXT, " +  field_InLocationFK + " INTEGER);";
            Cmd.ExecuteNonQuery();

            // create the supplier table
            Cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_supplier +
                " (supID integer primary key, " + field_SupName + " TEXT, " +
                  field_SupAddress + " TEXT, " + field_Supwebsite + " TEXT," +
                  field_Supemail + " TEXT, " +  field_SupTel + " TEXT);";
            Cmd.ExecuteNonQuery();

            // create parttype table
            Cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_parttype +
                " (typeID integer primary key, " + field_parttype + " TEXT, " +
                  field_typeGroupFK + " INTEGER);";
            Cmd.ExecuteNonQuery();

            // need to add order table
        }

        

        private void DropAllTables()
        {
            // delete all tables in database
            SQLiteCommand Cmd;
            Cmd = database.CreateCommand();
            Cmd.CommandText = "DROP TABLE " + table_parts;
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = "DROP TABLE " + table_location;
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = "DROP TABLE " + table_supplier;
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = "DROP TABLE " + table_parttype;
            Cmd.ExecuteNonQuery();  
            
            // need to add order table         
        
        }

        public DataTable LoadLocationsFromDB()
        {
            DataTable found = new DataTable("Locations");
            
            String cmd_String;
            SQLiteCommand sqlCmd;
            sqlCmd = database.CreateCommand();
            cmd_String = "SELECT * FROM " + table_location;
            sqlCmd.CommandText = cmd_String;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlCmd);
            adapter.Fill(found);
            return found;

        }

        public DataTable LoadPartTypesFromDB()
        {
            DataTable found = new DataTable("PartTypes");
            String cmd_String;
            SQLiteCommand sqlCmd;
            sqlCmd = database.CreateCommand();
            cmd_String = "SELECT * FROM " + table_parttype;
            sqlCmd.CommandText = cmd_String;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlCmd);
            adapter.Fill(found);
            return found;
        }

        public DataTable LoadSuppliersFromDB()
        {
            DataTable found = new DataTable("PartTypes");
            String cmd_String;
            SQLiteCommand sqlCmd;
            sqlCmd = database.CreateCommand();
            cmd_String = "SELECT * FROM " + table_supplier;
            sqlCmd.CommandText = cmd_String;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlCmd);
            adapter.Fill(found);
            return found;
        }

        public int findLocation(String lookingfor)
        {
            int found =-1;
            DataTable dt_found = new DataTable("Locations");

            String cmd_String, foundrow;
            SQLiteCommand sqlCmd;
            foundrow = null;
            sqlCmd = database.CreateCommand();
            cmd_String = "SELECT * FROM " + table_location + " WHERE " + field_locName 
                + " = '" + lookingfor + "'; "; 
            sqlCmd.CommandText = cmd_String;
            SQLiteDataReader datareader = sqlCmd.ExecuteReader();
            if (datareader.HasRows)
            {
                // record found for this compnumber
                while (datareader.Read())
                {
                    foundrow = datareader["locID"].ToString();
                    found = Int32.Parse(foundrow);
                }
            }

            return found;
        }

        public int findType(String lookingfor)
        {
            int found = -1;
            DataTable dt_found = new DataTable("Types");

            String cmd_String, foundrow;
            SQLiteCommand sqlCmd;
            foundrow = null;
            sqlCmd = database.CreateCommand();
            cmd_String = "SELECT * FROM " + table_parttype + " WHERE " + field_parttype
                + " = '" + lookingfor + "'; ";
            sqlCmd.CommandText = cmd_String;
            SQLiteDataReader datareader = sqlCmd.ExecuteReader();
            if (datareader.HasRows)
            {
                // record found for this compnumber
                while (datareader.Read())
                {
                    foundrow = datareader["typeID"].ToString();
                    found = Int32.Parse(foundrow);
                }
            }
            return found;
        }

        public int findSupplier(String lookingfor)
        {
            int found = -1;
            DataTable dt_found = new DataTable("Suppliers");

            String cmd_String, foundrow;
            SQLiteCommand sqlCmd;
            foundrow = null;
            sqlCmd = database.CreateCommand();
            cmd_String = "SELECT * FROM " + table_supplier + " WHERE " + field_SupName
                + " = '" + lookingfor + "'; ";
            sqlCmd.CommandText = cmd_String;
            SQLiteDataReader datareader = sqlCmd.ExecuteReader();
            if (datareader.HasRows)
            {
                // record found for this compnumber
                while (datareader.Read())
                {
                    foundrow = datareader["supID"].ToString();
                    found = Int32.Parse(foundrow);
                }
            }
            return found;
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            // clear data from the datbase
            //DropAllTables();
            //CreateAllTables();
            SQLiteCommand Cmd;
            Cmd = database.CreateCommand();
            Cmd.CommandText = "DROP TABLE " + table_parts;
            Cmd.ExecuteNonQuery();

            Cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_parts +
                " (partID integer primary key, " + field_partName + " TEXT, " +
                  field_PartDescription + " TEXT, " + field_partUnit + " TEXT, " +
                  field_SupplierPartNo + " TEXT, " + field_stock + " INTEGER, " +
                  field_price + " FLOAT, " + field_currency + " TEXT, " +
                  field_LocFK + " INTEGER, " + field_partTypeFK + " INTEGER, " +
                  field_SuppFK + " INTEGER, " + field_status + " TEXT);";
            Cmd.ExecuteNonQuery();

        }

        private void newItem_Btn_Click(object sender, RoutedEventArgs e)
        { 
            DataTable dt_locations, dt_types, dt_suppliers;
            places = new ArrayList();
            itemtype = new ArrayList();
            suppliers = new ArrayList();
            dt_locations = LoadLocationsFromDB();
            dt_types = LoadPartTypesFromDB();
            dt_suppliers = LoadSuppliersFromDB();
            if (dt_locations.Rows.Count != 0)
            {
                DataRow location_row;
                for (int i = 0; i < dt_locations.Rows.Count; i++)
                {
                    location_row = dt_locations.Rows[i];
                    places.Add(location_row[1]);
                }
            }
            if (dt_types.Rows.Count != 0)
            {
                DataRow type_row;
                for (int i = 0; i < dt_types.Rows.Count; i++)
                {
                    type_row = dt_types.Rows[i];
                    itemtype.Add(type_row[1]);
                }
            }

            if (dt_suppliers.Rows.Count != 0)
            {
                DataRow supplier_row;
                for (int i = 0; i < dt_suppliers.Rows.Count; i++)
                {
                    supplier_row = dt_suppliers.Rows[i];
                    suppliers.Add(supplier_row[1]);
                }
            }


            EditItemDialog itemDialog = new EditItemDialog(places, itemtype, suppliers);
            if (itemDialog.ShowDialog() == true)
            {
                // read answer string and paste it to the Flow Document
                String[] results = itemDialog.Answer.Split(',');
                Paragraph para = new Paragraph();
                para.Inlines.Add(itemDialog.Answer + '\n' + '\r');
                foreach (String s in results)
                {
                    para.Inlines.Add(s + '\n');
                }
                para.Inlines.Add(" "+ '\n' + '\r');
                ViewDoc.Blocks.Add(para);
                SQLiteCommand sqlCmd = new SQLiteCommand(database);
                String cmd_String;
                int amount, loc_select, type_select, sup_select, loc_index, type_index, supplier_index;
                // check stocke entered and set status to "Instock if >0
                results[8] = "OOS";
                amount = Int32.Parse("0"+results[5]);
                if (amount > 0)
                {
                    results[8] = "In Stock";
                }
                
                loc_select = Int32.Parse(results[9]);
                type_select = Int32.Parse(results[10]);
                sup_select = Int32.Parse(results[11]);
                loc_index = -1; type_index = -1; supplier_index = -1;

                if (loc_select != -1)
                {
                    // location was selected
                    loc_index = findLocation((String)places[loc_select]);
                    para.Inlines.Add("Location " + places[loc_select] + '\n' + '\r');
                }

                if (type_select != -1)
                {
                    // type was selected
                    type_index = findType((String)itemtype[type_select]);
                    para.Inlines.Add(" type " + itemtype[type_select] + '\n' + '\r');
                }

                if (sup_select != -1)
                {
                    // supplier was selected
                    supplier_index = findSupplier((String)suppliers[sup_select]);
                    para.Inlines.Add(" supplier " + suppliers[sup_select] + '\n' + '\r');
                }
                               
                ViewDoc.Blocks.Add(para);
                // CREATE THE SQLITE COMMAND STRING
                // Firstly the common part of the string.
                cmd_String = "INSERT INTO " + table_parts + " (" + field_partName + ", " +
                                field_PartDescription + ", " + field_partUnit + ", " + field_SupplierPartNo + ", " +
                                field_stock + ", " + field_price + ", " + field_currency + ", " + field_status;
               
                // then add additional fields for each possible return value 
                // based on which selections made in Combiboxes

                if (loc_index != -1)
                {
                    // there is a location selected
                    if (type_index != -1)
                    {
                        // there is a type selected ( and a location selected)
                        if (supplier_index != -1)
                        {
                            // theres is supplier selected ( and type and location)
                            cmd_String = cmd_String + ", " + field_InLocationFK +", " + field_partTypeFK +", " + field_SuppFK + ") VALUES ('" +
                                results[1] + "', '" + results[2] + "', '" + results[3] + "', '" + results[4] + "', '" +
                                results[5] + "', '" + results[6] + "', '" + results[7] + "', '" + results[8] + "', '" +
                                loc_index + "', '" + type_index + "', '" + supplier_index + "' );";
                            sqlCmd.CommandText = cmd_String;
                            sqlCmd.ExecuteNonQuery();
                            para.Inlines.Add("Location, type & Supplier selected" + '\n' + '\r');
                        }
                        else
                        {
                            // theres is no supplier selected ( but there is type and location)
                            cmd_String = cmd_String + ", " + field_InLocationFK + ", " + field_partTypeFK + ") VALUES ('" +
                                results[1] + "', '" + results[2] + "', '" + results[3] + "', '" + results[4] + "', '" +
                                results[5] + "', '" + results[6] + "', '" + results[7] + "', '" + results[8] + "', '" +
                                loc_index + "', '" + type_index +  "' );";
                            sqlCmd.CommandText = cmd_String;
                            sqlCmd.ExecuteNonQuery();
                            para.Inlines.Add("Location & type selected NO supplier" + '\n' + '\r');
                        }
                    }
                    else
                    {
                        // there is no type selected ( but is locacation slected)
                        if (supplier_index != -1)
                        {
                            // theres is supplier selected ( no  type but is location)
                            cmd_String = cmd_String + ", " + field_InLocationFK + ", "  + field_SuppFK + ") VALUES ('" +
                                results[1] + "', '" + results[2] + "', '" + results[3] + "', '" + results[4] + "', '" +
                                results[5] + "', '" + results[6] + "', '" + results[7] + "', '" + results[8] + "', '" +
                                loc_index + "', '"  + supplier_index + "' );";
                            sqlCmd.CommandText = cmd_String;
                            sqlCmd.ExecuteNonQuery();
                            para.Inlines.Add("Location & Supplier selected NO Type" + '\n' + '\r');
                        }
                        else
                        {
                            // theres is no supplier selected ( and no type but there is location)
                            cmd_String = cmd_String + ", " + field_InLocationFK +  ") VALUES ('" +
                                results[1] + "', '" + results[2] + "', '" + results[3] + "', '" + results[4] + "', '" +
                                results[5] + "', '" + results[6] + "', '" + results[7] + "', '" + results[8] + "', '" +
                                loc_index +  "' );";
                            sqlCmd.CommandText = cmd_String;
                            sqlCmd.ExecuteNonQuery();
                            para.Inlines.Add("Location selected but NO type and NO supplier" + '\n' + '\r');
                        }
                    }
                }
                else
                {
                    // there is no locaation selected
                    if (type_index != -1)
                    {
                        // there is a type selected ( but no  location selected)
                        if (supplier_index != -1)
                        {
                            // theres is supplier selected ( and type but no  location)
                            cmd_String = cmd_String + ", "  + field_partTypeFK + ", " + field_SuppFK + ") VALUES ('" +
                                results[1] + "', '" + results[2] + "', '" + results[3] + "', '" + results[4] + "', '" +
                                results[5] + "', '" + results[6] + "', '" + results[7] + "', '" + results[8] + "', '" +
                                + type_index + "', '" + supplier_index + "' );";
                            sqlCmd.CommandText = cmd_String;
                            sqlCmd.ExecuteNonQuery();
                            para.Inlines.Add("Type & Supplier selected NO Location" + '\n' + '\r');
                        }
                        else
                        {
                            // theres is no supplier selected ( but there is type but no  location)
                            cmd_String = cmd_String + ", " +  field_partTypeFK +  ") VALUES ('" +
                                results[1] + "', '" + results[2] + "', '" + results[3] + "', '" + results[4] + "', '" +
                                results[5] + "', '" + results[6] + "', '" + results[7] + "', '" + results[8] + "', '" +
                                + type_index +  "' );";
                            sqlCmd.CommandText = cmd_String;
                            sqlCmd.ExecuteNonQuery();
                            para.Inlines.Add("Type selected but No Location & NO supplier" + '\n' + '\r');
                        }
                    }
                    else
                    {
                        // there is no type selected ( and no locacation slected)
                        if (supplier_index != -1)
                        {
                            // theres is supplier selected ( but no  type but no  location)
                            cmd_String = cmd_String + ", "  + field_SuppFK + ") VALUES ('" +
                                results[1] + "', '" + results[2] + "', '" + results[3] + "', '" + results[4] + "', '" +
                                results[5] + "', '" + results[6] + "', '" + results[7] + "', '" + results[8] + "', '" +
                                supplier_index + "' );";
                            sqlCmd.CommandText = cmd_String;
                            sqlCmd.ExecuteNonQuery();
                            para.Inlines.Add("Supplier selected but NO Location & NO Type" + '\n' + '\r');
                        }
                        else
                        {
                            // theres is no supplier selected ( and no type or  location)
                            cmd_String = cmd_String + ") VALUES ('" + results[1] + "', '" + results[2] + "', '" + results[3] +
                                "', '" + results[4] + "', '" +  results[5] + "', '" + results[6] + "', '" + results[7] +
                                "', '" + results[8] +  "' );";
                            sqlCmd.CommandText = cmd_String;
                            sqlCmd.ExecuteNonQuery();
                            para.Inlines.Add("NO Location NO Type No Supplier" + '\n' + '\r');
                        }
                    }
                }

                ViewDoc.Blocks.Add(para);
            }
        }

       

        private void editItem_Btn_Click(object sender, RoutedEventArgs e)
        {
            // for testing only, an item is created
            //testItem = new Item();
            //testItem.ID = 22;                testItem.Name = "Sparkplug";
            //testItem.Description = "good";   testItem.Unit = "box of 12";
            //testItem.Amount = 2;             testItem.Currency = "€";
            //testItem.Price = 22.45;          testItem.PartNumber = "123456";
            //testItem.Status = true;          testItem.typeFK = 0;
            //testItem.locFK = 1;              testItem.supFK = 1;
            
                        
            
            EditItemDialog itemDialog = new EditItemDialog(database);
            if (itemDialog.ShowDialog() == true)
            {
                // read answer string and paste it to the Flow Document
                String[] results = itemDialog.Answer.Split(',');
                Paragraph para = new Paragraph();
                para.Inlines.Add(itemDialog.Answer + '\n' + '\r');
                foreach (String s in results)
                {
                    para.Inlines.Add(s + '\n');
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);
            }


        }

        
        private void newLocation_Btn_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt_location;
            places.Clear(); // reread from the database in case anything added
            dt_location = LoadLocationsFromDB();
            if (dt_location.Rows.Count != 0)
            {
                DataRow r;
                for (int i = 0; i < dt_location.Rows.Count; i++)
                {
                    r = dt_location.Rows[i];
                    places.Add(r[1].ToString()); // [1] item is locationname
                }
            }
            
            EditLocationDialog locationDialog = new EditLocationDialog(places);
            if (locationDialog.ShowDialog() == true)
            {
                // read answer string and paste it to the Flow Document
                String[] results = locationDialog.Answer.Split(',');
                Paragraph para = new Paragraph();
                para.Inlines.Add(locationDialog.Answer + '\n' + '\r');
                foreach (String s in results)
                {
                    para.Inlines.Add(s + '\n');
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);
                // add this to the database
                SQLiteCommand sqlCmd = new SQLiteCommand(database);
                String cmd_String;
                // if there is no selection of location group ( i.e. ==-1)
                if (Int32.Parse(results[3]) == -1)
                {
                    cmd_String = "INSERT INTO " + table_location + " (" + field_locName + ", " +
                         field_locType + ") VALUES ('" + results[1] + "', '" + results[2] + "' );";
                    para = new Paragraph();
                    para.Inlines.Add( cmd_String + '\n' + '\r');
                    ViewDoc.Blocks.Add(para);
                    sqlCmd.CommandText = cmd_String;
                    sqlCmd.ExecuteNonQuery();
                }
                else
                {
                    // find the record number of the group location
                    String grouplocationName = (String) places[Int32.Parse(results[3])];
                    para = new Paragraph();
                    para.Inlines.Add(grouplocationName + '\n' + '\r');
                    ViewDoc.Blocks.Add(para);

                    int index = findLocation(grouplocationName);
                    para = new Paragraph();
                    para.Inlines.Add("index =  "+ index + '\n' + '\r');
                    ViewDoc.Blocks.Add(para);

                    cmd_String = "INSERT INTO " + table_location + " (" + field_locName + ", " +
                         field_locType + ", " + field_InLocationFK  +") VALUES ('" + 
                         results[1] + "', '" + results[2] + "', '" + index + "' );";
                    para = new Paragraph();
                    para.Inlines.Add(cmd_String + '\n' + '\r');
                    ViewDoc.Blocks.Add(para);
                    sqlCmd.CommandText = cmd_String;
                    sqlCmd.ExecuteNonQuery();
                }

            }
        }

        private void editLocation_Btn_DragOver(object sender, DragEventArgs e)
        {
            // crete a location for testing only 
            //location = new Location();
            //location.ID = 31;
            //location.Name = "AJS1";
            //location.Type = "PlasticBox";
            //location.groupFK = 1;

            // EditLocationDialog locationDialog = new EditLocationDialog(location.ID, location.Name, location.Type, location.groupFK, places);
            EditLocationDialog locationDialog = new EditLocationDialog(database);
            if (locationDialog.ShowDialog() == true)
            {
                // read answer string and paste it to the Flow Document
                String[] results = locationDialog.Answer.Split(',');
                Paragraph para = new Paragraph();
                para.Inlines.Add(locationDialog.Answer + '\n' + '\r');
                foreach (String s in results)
                {
                    para.Inlines.Add(s + '\n');
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);

            }

        }

        private void editLocation_Btn_Click(object sender, RoutedEventArgs e)
        {
            // crete a location for testing only 
            //location = new Location();
            //location.ID = 31;
            //location.Name = "AJS1";
            //location.Type = "PlasticBox";
            //location.groupFK = 1;

            //EditLocationDialog locationDialog = new EditLocationDialog(location.ID, location.Name, location.Type, location.groupFK, places);
            EditLocationDialog locationDialog = new EditLocationDialog(database);
            if (locationDialog.ShowDialog() == true)
            {
                // read answer string and paste it to the Flow Document
                String[] results = locationDialog.Answer.Split(',');
                Paragraph para = new Paragraph();
                para.Inlines.Add(locationDialog.Answer + '\n' + '\r');
                foreach (String s in results)
                {
                    para.Inlines.Add(s + '\n');
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);

            }

        }

        

        private void newType_Btn_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt_types;
            itemtype.Clear(); // reread from the database in case anything added
            dt_types = LoadPartTypesFromDB();
            if (dt_types.Rows.Count != 0)
            {
                DataRow r;
                for (int i = 0; i < dt_types.Rows.Count; i++)
                {
                    r = dt_types.Rows[i];
                    itemtype.Add(r[1].ToString()); // [1] item is typename
                }
            }
            EditItemTypeDialog typeDialog = new EditItemTypeDialog(itemtype);
            if (typeDialog.ShowDialog() == true)
            {
                String[] results = typeDialog.Answer.Split(',');
                Paragraph para = new Paragraph();
                para.Inlines.Add(typeDialog.Answer + '\n' + '\r');
                foreach (String s in results)
                {
                    para.Inlines.Add(s + '\n');
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);

                // add the new type to the database
                SQLiteCommand sqlCmd = new SQLiteCommand(database);
                String cmd_String;
                // if there is no selection of type group ( i.e. ==-1)
                if (Int32.Parse(results[2]) == -1)
                {
                    cmd_String = "INSERT INTO " + table_parttype + " (" + field_parttype + 
                        ") VALUES ('" + results[1] + "' );";
                    para = new Paragraph();
                    para.Inlines.Add(cmd_String + '\n' + '\r');
                    ViewDoc.Blocks.Add(para);
                    sqlCmd.CommandText = cmd_String;
                    sqlCmd.ExecuteNonQuery();
                }
                else
                {
                    // find the record number of the group type
                    String groupTypeName = (String) itemtype[Int32.Parse(results[2])];
                    para = new Paragraph();
                    para.Inlines.Add(groupTypeName + '\n' + '\r');
                    ViewDoc.Blocks.Add(para);

                    int index = findType(groupTypeName);
                    para = new Paragraph();
                    para.Inlines.Add("index =  " + index + '\n' + '\r');
                    ViewDoc.Blocks.Add(para);

                    cmd_String = "INSERT INTO " + table_parttype + " (" + field_parttype + ", " 
                        + field_typeGroupFK + ") VALUES ('" + results[1] +  "', '" + index + "' );";
                    para = new Paragraph();
                    para.Inlines.Add(cmd_String + '\n' + '\r');
                    ViewDoc.Blocks.Add(para);
                    sqlCmd.CommandText = cmd_String;
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

       

        private void editType_Btn_Click(object sender, RoutedEventArgs e)
        {
            // type created for testing only
            //partType = new ItemType();
            //partType.ID = 4;
            //partType.Name = "AJS Part";
            //partType.typeGroupFK = 1;

            EditItemTypeDialog typeDialog = new EditItemTypeDialog(database);
            // EditItemTypeDialog typeDialog = new EditItemTypeDialog(partType.ID, partType.Name,partType.typeGroupFK, itemtype);
            if (typeDialog.ShowDialog() == true)
            {
                String[] results = typeDialog.Answer.Split(',');
                Paragraph para = new Paragraph();
                para.Inlines.Add(typeDialog.Answer + '\n' + '\r');
                foreach (String s in results)
                {
                    para.Inlines.Add(s + '\n');
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);
            }
        }

       

        private void newSupplier_Btn_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt_suppliers = new DataTable("Suppliers");
            suppliers.Clear();
            dt_suppliers = LoadSuppliersFromDB();
            if (dt_suppliers.Rows.Count != 0)
            {
                DataRow r;
                for (int i = 0; i < dt_suppliers.Rows.Count; i++)
                {
                    r = dt_suppliers.Rows[i];
                    suppliers.Add(r[1]);
                }
            }


            EditSupplierDialog supplierDialog = new EditSupplierDialog();
            if (supplierDialog.ShowDialog() == true)
            {
                String[] results = supplierDialog.Answer.Split(';');
                Paragraph para = new Paragraph();
                para.Inlines.Add(supplierDialog.Answer + '\n' + '\r');
                foreach (String s in results)
                {
                    para.Inlines.Add(s + '\n');
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);

                // add the new supplier to to the database
                SQLiteCommand sqlCmd = new SQLiteCommand(database);
                String cmd_String;
                cmd_String = "INSERT INTO " + table_supplier + " (" +
                    field_SupName + ", " + field_SupAddress + ", " +
                    field_Supwebsite + "," + field_Supemail + ", " + field_SupTel +
                    ") VALUES ('" + results[1] + "', '" + results[2] + "', '" +
                    results[3] + "', '" + results[4] + "', '" + results[5] + "' );";
                sqlCmd.CommandText = cmd_String;
                sqlCmd.ExecuteNonQuery();
                para = new Paragraph();
                para.Inlines.Add(cmd_String + '\n' + '\r');
                ViewDoc.Blocks.Add(para);
            }
        }

        private void editSupplier_Btn_Click(object sender, RoutedEventArgs e)
        {
            // create a supplier for testing only
            //supplier = new Supplier();
            //supplier.Name = "AMC Classic Spares, Steven Sorby";
            //supplier.Address = "Not knowne, Some in , England";
            //supplier.Website = "www.amcclassicspares.com";
            //supplier.Email = "spares@amcclassicspares.com";
            //supplier.phone = "(+44) 01462 811770";

            EditSupplierDialog supplierDialog = new EditSupplierDialog(database);
            // EditSupplierDialog supplierDialog = new EditSupplierDialog(supplier.ID, supplier.Name, supplier.Address, supplier.Website,supplier.Email,supplier.phone);
            if (supplierDialog.ShowDialog() == true)
            {
                String[] results = supplierDialog.Answer.Split(',');
                Paragraph para = new Paragraph();
                para.Inlines.Add(supplierDialog.Answer + '\n' + '\r');
                foreach (String s in results)
                {
                    para.Inlines.Add(s + '\n');
                }
                para.Inlines.Add(" " + '\n' + '\r');
                ViewDoc.Blocks.Add(para);
            }
        }

        private void search_Btn_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow searchWindow = new SearchWindow(database);
            searchWindow.Show();
        }

        private void enableBtnMenu()
        {
            // enable all buttons when a live connection is present
            newItem_Btn.IsEnabled = true;
            editItem_Btn.IsEnabled = true;
            newLocation_Btn.IsEnabled = true;
            editLocation_Btn.IsEnabled = true;
            newType_Btn.IsEnabled = true;
            editType_Btn.IsEnabled = true;
            newSupplier_Btn.IsEnabled = true;
            editSupplier_Btn.IsEnabled = true;
            clearBtn.IsEnabled = true;
            FileOpen_Btn.IsEnabled = true;
            search_Btn.IsEnabled = true;
        }

        private void disbleBtnMenu()
        {
            // disable all buttons when a live connection is no present
            newItem_Btn.IsEnabled = false;
            editItem_Btn.IsEnabled = false;
            newLocation_Btn.IsEnabled = false;
            editLocation_Btn.IsEnabled = false;
            newType_Btn.IsEnabled = false;
            editType_Btn.IsEnabled = false;
            newSupplier_Btn.IsEnabled = false;
            editSupplier_Btn.IsEnabled = false;
            clearBtn.IsEnabled = false;
            FileOpen_Btn.IsEnabled = false;
            search_Btn.IsEnabled = false;
        }

        private void CloseDB_click(object sender, RoutedEventArgs e)
        {
            if (DB_Connection == true)
            {
                database.Close();
                DB_Connection = false;
                disbleBtnMenu();
            }
        }


        private void  Exit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (DB_Connection == true)
            {
                database.Close();
            }
            this.Close();
        }

        


    }
}
