﻿using System;
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
        ItemNameClass place;
        ArrayList places = new ArrayList();

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
            
            LoadDatagrid();
            
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
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;
            
            // if the TBox is empty then this is a new entry else it is an update
            if (iDTBox.Text == String.Empty)
            {
                // this is a new entry 
                // add to location table
                // MessageBox.Show("This is a new entry ");
                int groupindex = groupFkCombobox.SelectedIndex ;
                if (groupindex == -1)
                {
                    sql_string = "INSERT INTO Locations (Location, Type) VALUES ('"
                    + nameTBox.Text + "', '" + typeTBox.Text +  "');";
                }
                else
                {
                    ItemNameClass selected = (ItemNameClass)places[groupindex];
                    sql_string = "INSERT INTO Locations (Location, Type, LocationFK) VALUES ('"
                    + nameTBox.Text + "', '" + typeTBox.Text + "', '" + selected.getID() + "');";
                }
                // MessageBox.Show(sql_string);
                sql_cmd.CommandText = sql_string;
                sql_cmd.ExecuteNonQuery();
                LoadDatagrid();
              
            }
            else
            {
                // this is an update
                int index = Int32.Parse("0" + iDTBox.Text);
                {
                    // update the record at index 
                    // MessageBox.Show("This is an update on entry " + index);
                    int groupindex = groupFkCombobox.SelectedIndex ;
                    if (groupindex == -1)
                    {
                        sql_string = "UPDATE Locations SET Location, = '" + nameTBox.Text +
                            "', Type = '" + typeTBox.Text + "' WHERE locID = '" + index + "';";
                    }
                    else
                    {
                        ItemNameClass selected = (ItemNameClass)places[groupindex];
                        sql_string = "UPDATE Locations SET Location = '" + nameTBox.Text + "', Type = '" + 
                            typeTBox.Text+ "', LocationFK = '" + selected.getID() + "' WHERE locID = '" + index + "';";
                    }
                    // MessageBox.Show(sql_string);
                    sql_cmd.CommandText = sql_string;
                    sql_cmd.ExecuteNonQuery();
                    LoadDatagrid();

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
                // MessageBox.Show("This  " + selected_row + "  " + drv[0] + "  " + drv[1] + "  " + drv[2]);
                iDTBox.Text = drv[0].ToString();
                nameTBox.Text = drv[1].ToString();
                typeTBox.Text = drv[2].ToString();
                String groupStr = drv[3].ToString();
                int groupindex = getIndex(drv[3].ToString(), places);
                groupFkCombobox.SelectedIndex = groupindex;
                OK_Btn.Content = "Update";
           }
            

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

        private void delete_Btn_Click(object sender, RoutedEventArgs e)
        {
            SQLiteCommand sql_cmd = database.CreateCommand();
            String sql_string;
            int index = Int32.Parse("0" + iDTBox.Text);
            if (index > 0)
            {
                // check if in use
                if (isUsedByLocations(index))
                {
                    MessageBox.Show("You cannot delete a location in use by other locations");
                }
                else
                {
                    if (isUsedByItems(index))
                    {
                        MessageBox.Show("You cannot delete a location in use by items");
                    }
                    else
                    {
                        // confirm deletion
                        MessageBoxResult result = MessageBox.Show("Are you sure you want to delete record" + index, "Deletion", MessageBoxButton.YesNoCancel);
                        if (result == MessageBoxResult.Yes)
                        {
                            // delete the record at index 
                            sql_string = "DELETE FROM Locations WHERE locID = '" + index + "';";
                            sql_cmd.CommandText = sql_string;
                            sql_cmd.ExecuteNonQuery();
                            // reload locations again
                            LoadDatagrid();
                            MessageBox.Show("The record at " + index + " has been deleted");
                            iDTBox.Text = String.Empty;
                            nameTBox.Text = String.Empty;
                            typeTBox.Text = String.Empty;
                            groupFkCombobox.SelectedIndex = -1;
                            OK_Btn.Content = "Add New";
                        }
                    }

                }
                             
                
               }
        }

        private bool isUsedByLocations(int loc)
        {
            bool check = false;
            SQLiteCommand sql_cmd = database.CreateCommand();
            sql_cmd.CommandText = "SELECT * FROM Locations WHERE LocationFK = '" + loc + "';";
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            if (reader.HasRows)
            {
                check = true;
            }
            return check;
       }


        private Boolean isUsedByItems(int loc)
        {
            Boolean check = false;
            SQLiteCommand sql_cmd = database.CreateCommand();
            sql_cmd.CommandText = "SELECT * FROM Item WHERE LocationFK = '" + loc + "';";
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            if (reader.HasRows)
            {
                check = true;
            }
            return check;
        }
        private void LoadDatagrid()
        {
            dt_locations = new DataTable();
            SQLiteCommand load_cmd = database.CreateCommand();
            String load_string;
            load_string = "SELECT * FROM Locations";
            load_cmd.CommandText = load_string;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(load_cmd);
            adapter.Fill(dt_locations);
            locationdataGrid.ItemsSource = dt_locations.DefaultView;
            places.Clear();
            foreach (DataRow r in dt_locations.Rows)
            {
                place = new ItemNameClass(Int32.Parse(r[0].ToString()), r[1].ToString());
                places.Add(place);
            }
            groupFkCombobox.Items.DetachFromSourceCollection();
            
            groupFkCombobox.ItemsSource = places;
        }



        private void undoBtn_Click(object sender, RoutedEventArgs e)
        {
            OK_Btn.Content = "Add New";
            iDTBox.Text = String.Empty;
            nameTBox.Text = String.Empty;
            typeTBox.Text = String.Empty;
            groupFkCombobox.SelectedIndex = -1;
            OK_Btn.Content = "Add New";
        }
    }



    
}
