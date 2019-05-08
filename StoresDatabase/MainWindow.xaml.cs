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
            Connection = "Data Source =c:\\Databases\\Stock.db;Version=3;New=True;Compress=True;";
            database = new SQLiteConnection(Connection);
            database.Open();
           

            CreateAllTables();
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
                  field_SupAddress + " TEXT, " + field_Supwebsite + " TEXT" +
                  field_Supemail + " TEXT, " +  field_SupTel + " TEXT);";
            Cmd.ExecuteNonQuery();

            // create parttype table
            Cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + table_parttype +
                " (typeID integer primary key, " + field_parttype + " TEXT, " +
                  field_typeGroupFK + " INTEGER);";
            Cmd.ExecuteNonQuery();
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
            EditSupplierDialog supplierDialog = new EditSupplierDialog();
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

        private void CloseDB_click(object sender, RoutedEventArgs e)
        {
            database.Close();
        }


        private void Exit_btn_Click(object sender, RoutedEventArgs e)
        {
            database.Close();
            this.Close();
        }


    }
}
