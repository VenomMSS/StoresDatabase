﻿using Microsoft.Win32;
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
            // these ArrayLists  are for testing only
            places = new ArrayList();
            places.Add("Box1"); places.Add("Box2"); places.Add("Box3");
            suppliers = new ArrayList();
            suppliers.Add("AMOC"); suppliers.Add("AMC"); suppliers.Add("EBAY");
            itemtype = new ArrayList();
            itemtype.Add("Part m/c"); itemtype.Add("Tool"); itemtype.Add("Tool consumable");

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

            CreateAllTables();
            // load data from all tables 
            enableBtnMenu();

        }

        private void FileOpen_Btn_Click(object sender, RoutedEventArgs e)
        {
            // OPen a file and read the contents
            // Firstly need a Openfile dialog
            openFileDlg = new OpenFileDialog();
            openFileDlg.FileOk += openCSVFile;
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
            DropAllTables();
            CreateAllTables();

           // fix error in Supplier table
            //SQLiteCommand sqlCmd;
            //sqlCmd = database.CreateCommand();
            //String cmdString;
            //sqlCmd.CommandText = "DROP TABLE " + table_supplier;
            //sqlCmd.ExecuteNonQuery();

            //sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_supplier +
            //    " (supID integer primary key, " + field_SupName + " TEXT, " +
            //      field_SupAddress + " TEXT, " + field_Supwebsite + " TEXT," +
            //      field_Supemail + " TEXT, " + field_SupTel + " TEXT);";
            //sqlCmd.ExecuteNonQuery();

        }

        private void newItem_Btn_Click(object sender, RoutedEventArgs e)
        {
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
            }
        }

       

        private void editItem_Btn_Click(object sender, RoutedEventArgs e)
        {
            // for testing only, an item is created
            testItem = new Item();
            testItem.ID = 22;                testItem.Name = "Sparkplug";
            testItem.Description = "good";   testItem.Unit = "box of 12";
            testItem.Amount = 2;             testItem.Currency = "€";
            testItem.Price = 22.45;          testItem.PartNumber = "123456";
            testItem.Status = true;          testItem.typeFK = 0;
            testItem.locFK = 1;              testItem.supFK = 1;
            
                        
            
            EditItemDialog itemDialog = new EditItemDialog(testItem.ID,testItem.Name,testItem.Description,testItem.Unit,
                testItem.PartNumber,testItem.Amount,testItem.Price,testItem.Currency,testItem.Status.ToString(),
                testItem.locFK,testItem.typeFK,testItem.supFK,places,itemtype,suppliers);
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
            location = new Location();
            location.ID = 31;
            location.Name = "AJS1";
            location.Type = "PlasticBox";
            location.groupFK = 1;

            EditLocationDialog locationDialog = new EditLocationDialog(location.ID, location.Name, location.Type, location.groupFK, places);

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
            location = new Location();
            location.ID = 31;
            location.Name = "AJS1";
            location.Type = "PlasticBox";
            location.groupFK = 1;

            EditLocationDialog locationDialog = new EditLocationDialog(location.ID, location.Name, location.Type, location.groupFK, places);

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
            partType = new ItemType();
            partType.ID = 4;
            partType.Name = "AJS Part";
            partType.typeGroupFK = 1;

            EditItemTypeDialog typeDialog = new EditItemTypeDialog(partType.ID, partType.Name,partType.typeGroupFK, itemtype);
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
            supplier = new Supplier();
            supplier.Name = "AMC Classic Spares, Steven Sorby";
            supplier.Address = "Not knowne, Some in , England";
            supplier.Website = "www.amcclassicspares.com";
            supplier.Email = "spares@amcclassicspares.com";
            supplier.phone = "(+44) 01462 811770";

            EditSupplierDialog supplierDialog = new EditSupplierDialog(supplier.ID, supplier.Name, supplier.Address, supplier.Website,supplier.Email,supplier.phone);
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
        }

        private void CloseDB_click(object sender, RoutedEventArgs e)
        {
            database.Close();
        }


        private void  Exit_btn_Click(object sender, RoutedEventArgs e)
        {
            database.Close();
            this.Close();
        }

        


    }
}
