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
        }

        private void ClearItem_Click(object sender, RoutedEventArgs e)
        {
            // clear data from item table
            SQLiteCommand sqlcmd = database.CreateCommand();
            sqlcmd.CommandText = "DROP TABLE " + table_parts;
            sqlcmd.ExecuteNonQuery();
            sqlcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_parts +
                " (partID integer primary key, " + field_partName + " TEXT, " +
                  field_PartDescription + " TEXT, " + field_partUnit + " TEXT, " +
                  field_SupplierPartNo + " TEXT, " + field_stock + " INTEGER, " +
                  field_price + " FLOAT, " + field_currency + " TEXT, " +
                  field_LocFK + " INTEGER, " + field_partTypeFK + " INTEGER, " +
                  field_SuppFK + " INTEGER, " + field_status + " TEXT);";
           sqlcmd.ExecuteNonQuery();
        }

        private void ClearLocation_Click(object sender, RoutedEventArgs e)
        {
            // clear data from item table
            SQLiteCommand sqlcmd = database.CreateCommand();
            sqlcmd.CommandText = "DROP TABLE " + table_location;
            sqlcmd.ExecuteNonQuery();
            sqlcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_location +
                " (locID integer primary key, " + field_locName + " TEXT, " +
                  field_locType + " TEXT, " + field_InLocationFK + " INTEGER);";
            
            sqlcmd.ExecuteNonQuery();
        }

        private void ClearTypes_Click(object sender, RoutedEventArgs e)
        {
            // clear data from item table
            SQLiteCommand sqlcmd = database.CreateCommand();
            sqlcmd.CommandText = "DROP TABLE " + table_parttype;
            sqlcmd.ExecuteNonQuery();
            sqlcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_parttype +
                " (typeID integer primary key, " + field_parttype + " TEXT, " +
                  field_typeGroupFK + " INTEGER);";
            sqlcmd.ExecuteNonQuery();
        }

        private void ClearSupplier_Click(object sender, RoutedEventArgs e)
        {
            // clear data from item table
            SQLiteCommand sqlcmd = database.CreateCommand();
            sqlcmd.CommandText = "DROP TABLE " + table_supplier;
            sqlcmd.ExecuteNonQuery();
            sqlcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_supplier +
                " (supID integer primary key, " + field_SupName + " TEXT, " +
                  field_SupAddress + " TEXT, " + field_Supwebsite + " TEXT," +
                  field_Supemail + " TEXT, " + field_SupTel + " TEXT);";
            sqlcmd.ExecuteNonQuery();
        }

        private void editItem_Btn_Click(object sender, RoutedEventArgs e)
        {
            // this method is called to create an EditItemDialog 
            // which allows users to add, edit, delete items from the database
            // and to import new items from a csv file.
            
                        
            
            EditItemDialog itemDialog = new EditItemDialog(database);
            if (itemDialog.ShowDialog() == true)
            {
                // read answer string and paste it to the Flow Document
                // this is not needed
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

        
  

        private void editLocation_Btn_Click(object sender, RoutedEventArgs e)
        {
            // this method is called to create an EditLocationDialog 
            // which allows users to add , edit and delete Locations from database.

            
            EditLocationDialog locationDialog = new EditLocationDialog(database);
            if (locationDialog.ShowDialog() == true)
            {
                // NO NEED for this .read answer string and paste it to the Flow Document
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

        

   
       

        private void editType_Btn_Click(object sender, RoutedEventArgs e)
        {
            // this method is called to create an EditItemTypeDialog 
            // which allows users to add , edit and delete itemtypes from database.


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

      

  
        private void editSupplier_Btn_Click(object sender, RoutedEventArgs e)
        {
            // this method is called to create an EdiSupplierDialog
            // which allows users to add , edit and delete isuppliers from database.


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
           
            editItem_Btn.IsEnabled = true;
            editLocation_Btn.IsEnabled = true;
            editType_Btn.IsEnabled = true;
            editSupplier_Btn.IsEnabled = true;
            clearBtn.IsEnabled = true;
            
            search_Btn.IsEnabled = true;
        }

        private void disbleBtnMenu()
        {
            // disable all buttons when a live connection is no present
            editItem_Btn.IsEnabled = false;
            editLocation_Btn.IsEnabled = false;
            editType_Btn.IsEnabled = false;
            editSupplier_Btn.IsEnabled = false;
            clearBtn.IsEnabled = false;
            
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
