using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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

namespace StoresDatabase
{
    /// <summary>
    /// Interaction logic for EditSupplierDialog.xaml
    /// </summary>
    public partial class EditSupplierDialog : Window
    {
        SQLiteConnection database;
        DataTable dt_suppliers;
        ArrayList supplierList;
        // default can be used for new supplier as nothing to pass in 
        public EditSupplierDialog()
        {
            InitializeComponent();
        }

        // constructor for use for edit supplier
        public EditSupplierDialog(int ID, String Name, String address, String Web, String email, String Phone)
        {
            InitializeComponent();
            iDTBox.Text = ID.ToString();
            nameTBox.Text = Name;
            addressTBox.Text = address;
            websiteTBox.Text = Web;
            emailTBox.Text = email;
            telphoneTBox.Text = Phone;

        }
        
        // new constructor to pass database 
        public EditSupplierDialog(SQLiteConnection db)
        {
            InitializeComponent();
            database = db;
            LoadDataGrid();
        }

        public string Answer
        {
            get
            {
                return iDTBox.Text + ";" + nameTBox.Text + ";" + 
                    addressTBox.Text+ ";" + websiteTBox.Text +";" + emailTBox.Text +";" +
                    telphoneTBox.Text;
            }
        }

        private void OK_Btn_Click(object sender, RoutedEventArgs e)
        {
            //DialogResult = true;
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;
            if (iDTBox.Text == string.Empty)
            {
                // this is a new entry
                sql_string = "INSERT INTO Suppliers (Supplier, Address, Web, Email, Tel) VALUES ('" +
                 nameTBox.Text + "', '" + addressTBox.Text + "', '" + websiteTBox.Text + "', '" +
                 emailTBox.Text + "', '" + telphoneTBox.Text + "');";
                sql_cmd.CommandText = sql_string;
                sql_cmd.ExecuteNonQuery();

            }
            else
            {
                // this is an update
                int index = Int32.Parse("0"+ iDTBox.Text);
                sql_string = "UPDATE Suppliers  SET Supplier = '" + nameTBox.Text + "', Address = '" + 
                    addressTBox.Text + "', Web = '" + websiteTBox.Text + "', Email = '" +
                    emailTBox.Text + "', Tel = '"  + telphoneTBox.Text + "' WHERE supID = '" + index +"';";
                sql_cmd.CommandText = sql_string;
                sql_cmd.ExecuteNonQuery();
            }
            LoadDataGrid();
            iDTBox.Text = string.Empty;
            nameTBox.Text = string.Empty;
            addressTBox.Text = string.Empty;
            websiteTBox.Text = string.Empty;
            emailTBox.Text = string.Empty;
            telphoneTBox.Text = string.Empty;
            // reset button text
            ok_Btn.Content = "Add new";
           
        }

        private bool isUsedByItems(int supplier)
        {
            bool check = false;
            SQLiteCommand sql_cmd = database.CreateCommand();
            sql_cmd.CommandText = "SELECT * FROM Item WHERE SupplierFK = '" + supplier + "';";
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            if (reader.HasRows)
            {
                check = true;
            }
            return check;
        }

        private void LoadDataGrid()
        {
            dt_suppliers = new DataTable();
            SQLiteCommand sql_cmd = database.CreateCommand();
            sql_cmd.CommandText = "SELECT * FROM Suppliers";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
            adapter.Fill(dt_suppliers);
            supplierDataGrid.ItemsSource = dt_suppliers.DefaultView;
            supplierList = new ArrayList();
            foreach (DataRow r in dt_suppliers.Rows)
            {
                supplierList.Add(r[1]);
            }
        }

        private void undoBtn_Click(object sender, RoutedEventArgs e)
        {
            iDTBox.Text = string.Empty;
            nameTBox.Text = string.Empty;
            addressTBox.Text = string.Empty;
            websiteTBox.Text = string.Empty;
            emailTBox.Text = string.Empty;
            telphoneTBox.Text = string.Empty;
            ok_Btn.Content = "Add New";
        }

        private void supplierDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedrow = supplierDataGrid.SelectedIndex;
            foreach (DataRowView drv in supplierDataGrid.SelectedItems)
            {
                iDTBox.Text = drv[0].ToString();
                nameTBox.Text = drv[1].ToString();
                addressTBox.Text = drv[2].ToString();
                websiteTBox.Text = drv[3].ToString();
                emailTBox.Text = drv[4].ToString();
                telphoneTBox.Text = drv[5].ToString();
                // change button text
                ok_Btn.Content = "Update";
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // delete record
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;
            int index = Int32.Parse("0" + iDTBox.Text);
            if (index > 0)
                {
                // see if this spulier is used by any item
                if (isUsedByItems(index))
                {
                    MessageBox.Show("Cannot deletre a supplier with linked items");
                }
                else
                {
                    // not in use . confimr wit huser
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this supplier", "Deletion", MessageBoxButton.YesNoCancel);
                    if (result == MessageBoxResult.Yes)
                    {
                        // deletion confirmed
                        sql_string = "DELETE FROM Suppliers WHERE supID = '" + index + "';";
                        sql_cmd.CommandText = sql_string;
                        sql_cmd.ExecuteNonQuery();
                        LoadDataGrid();
                        iDTBox.Text = string.Empty;
                        nameTBox.Text = string.Empty;
                        addressTBox.Text = string.Empty;
                        websiteTBox.Text = string.Empty;
                        emailTBox.Text = string.Empty;
                        telphoneTBox.Text = string.Empty;
                        ok_Btn.Content = "Add New";
                    }
                }
                }
        }
    }
}
