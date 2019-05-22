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
using System.Data;
using System.Data.SQLite;

namespace StoresDatabase
{
    /// <summary>
    /// Interaction logic for EditItemTypeDialog.xaml
    /// </summary>
    public partial class EditItemTypeDialog : Window
    {
        SQLiteConnection database;
        DataTable dt_itemTypes;
        ArrayList itemtypes = new ArrayList(); 

        public EditItemTypeDialog()
        {
            InitializeComponent();
        }

        // constructor for new type
        public EditItemTypeDialog(ArrayList types)
        {
            InitializeComponent();
            typeGroupComboBox.ItemsSource = types;
        }

        //constructor for edit type
        public EditItemTypeDialog(int id, String TypeName, int index,ArrayList types)
        {
            InitializeComponent();
            iDTBox.Text = id.ToString(); 
            typeNameTBox.Text = TypeName;
            typeGroupComboBox.ItemsSource = types;
            typeGroupComboBox.SelectedIndex = index;
        }

        // new constructor to pass database
        public EditItemTypeDialog(SQLiteConnection db)
        {
            InitializeComponent();
            database = db;
            LoadDataGrid();
        }


        public string Answer
        {
            get
            {
                return iDTBox.Text + "," + typeNameTBox.Text + "," + typeGroupComboBox.SelectedIndex.ToString(); 
            }
        }

        private void OK_Btn_Click(object sender, RoutedEventArgs e)
        {
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;

            
            if (iDTBox.Text == string.Empty)
            {
                // if iDTBox is empty then this is a new record 
                int grpindex = typeGroupComboBox.SelectedIndex + 1;
                if (grpindex == 0)
                {
                    // there is no group type selected
                    sql_string = "INSERT INTO ItemType (ItemType) VALUES ('"
                    + typeNameTBox.Text + "');";
                }
                else
                {
                    // there is a group selected
                    sql_string = "INSERT INTO ItemType (ItemType, TypeGroup) VALUES ('"
                    + typeNameTBox.Text + "', '" + grpindex + "');";
                }
                sql_cmd.CommandText = sql_string;
                sql_cmd.ExecuteNonQuery();
                LoadDataGrid();
                
            }
            else
            // else this is an update
            {
                int index = Int32.Parse("0"+ iDTBox.Text);
                int grpindex = typeGroupComboBox.SelectedIndex + 1;
                if (grpindex == 0)
                {
                    // if no group selected
                    sql_string = "UPDATE ItemType SET ItemType = '" + typeNameTBox.Text +
                        "' WHERE typeID = '" + index + "';"; 
                }
                else
                {
                    sql_string = "UPDATE ItemType SET ItemType = '" + typeNameTBox.Text + 
                        "' TypeGroup = '" + grpindex + "' WHERE typeID = '" + index + "';";
                }
                sql_cmd.CommandText = sql_string;
                sql_cmd.ExecuteNonQuery();
                LoadDataGrid();
            }

            // clear all textboxes and reset okbtn to add new
            iDTBox.Text = string.Empty;
            typeNameTBox.Text = string.Empty;
            typeGroupComboBox.SelectedIndex = -1;
            OK_Btn.Content = "Add New";

        }

        private bool isUsedByItems(int type)
        {
            bool check = false;
            SQLiteCommand sql_cmd = database.CreateCommand();
            sql_cmd.CommandText = "SELECT * FROM Item WHERE PartTypeFK = '" + type + "';";
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            if (reader.HasRows)
            {
                check = true;
            }
            return check;
        }

        private bool isUsedByTypes(int type)
        {
            bool check = false;
            SQLiteCommand sql_cmd = database.CreateCommand();
            sql_cmd.CommandText = "SELECT * FROM ItemType WHERE TypeGroup = '" + type +"';";
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            if (reader.HasRows)
            {
                check = true;
            }
            return check;
        }

        private void LoadDataGrid()
        {
            SQLiteCommand sql_cmd = database.CreateCommand();
            dt_itemTypes = new DataTable();
            sql_cmd.CommandText = "SELECT * FROM ItemType";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql_cmd);
            adapter.Fill(dt_itemTypes);
            typeDataGrid.ItemsSource = dt_itemTypes.DefaultView;
            itemtypes.Clear();
            foreach (DataRow r in dt_itemTypes.Rows)
            {
                itemtypes.Add(r[1]);
            }
            typeGroupComboBox.ItemsSource = itemtypes;

        }

        private void undoBtn_Click(object sender, RoutedEventArgs e)
        {
            iDTBox.Text = string.Empty;
            typeNameTBox.Text = string.Empty;
            typeGroupComboBox.SelectedIndex = -1;
            OK_Btn.Content = "Add New";
        }

        private void typeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedrow = typeDataGrid.SelectedIndex;
            foreach (DataRowView drv in typeDataGrid.SelectedItems)
            {
                iDTBox.Text = drv[0].ToString();
                typeNameTBox.Text = drv[1].ToString();
                String group = drv[2].ToString();
                int groupindex = Int32.Parse("0"+group);
                if (groupindex > 0)
                {
                    typeGroupComboBox.SelectedIndex = groupindex - 1;
                }
                else
                {
                    typeGroupComboBox.SelectedIndex =  -1;
                }
                // change button text
                OK_Btn.Content = "Update";
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
                // check if type used by other types
                if (isUsedByTypes(index))
                {
                    MessageBox.Show("Cannot delete when this type is used by other types");
                }
                else
                {
                    if (isUsedByItems(index))
                    {
                        MessageBox.Show("Cannot delete when this type is used by some items");
                    }

                    else
                    {
                        // not in use anywhere
                        MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this type" + index, "Deletion", MessageBoxButton.YesNoCancel);
                        if (result == MessageBoxResult.Yes)
                        {
                            sql_string = "DELETE FROM ItemType WHERE typeID = '" + index + "';";
                            sql_cmd.CommandText = sql_string;
                            sql_cmd.ExecuteNonQuery();
                            LoadDataGrid();
                            iDTBox.Text = string.Empty;
                            typeNameTBox.Text = string.Empty;
                            typeGroupComboBox.SelectedIndex = -1;
                            OK_Btn.Content = "Add New";
                        }
                    }
                }
            }

            // check if type used by items
            // confirm messagebox delete

        }
    }
}
