using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace Password_manager
{
    public partial class credentials_groups : Form
    {

        private MySqlConnection conn;
        private string connectionString;
        private int[] ids;
        private int user_id;
        private bool mouseDown;
        private Point lastLocation;



        private void load()
        {
            try
            {
                conn.Open();

                List<Item> items = new List<Item>();


                string query = "SELECT * FROM credentials_groups WHERE user_id = @user_id ORDER BY name ASC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user_id", user_id);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Item(
                            Convert.ToInt32(reader["id"]),
                            reader["name"].ToString(),
                            Convert.ToInt32(reader["user_id"])
                        ));
                    }
                }

                items.Add(new Item(0, "Without group", 0));
                if (items.Count != 1)
                {
                    comboBox1.DataSource = items;
                    comboBox1.SelectedIndex = 0;
                    button1.Enabled = true;
                    comboBox1.Enabled = true;
                    comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else
                {
                    button1.Enabled = false;
                    comboBox1.Enabled = false;
                    comboBox1.DropDownStyle = ComboBoxStyle.Simple;
                    comboBox1.Text = "No groups";
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Error loading groups: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }

        public credentials_groups(int[] ids, int user_id)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);


            this.ids = ids;
            this.user_id = user_id;
            load();


            pictureBox1.SendToBack();

            pictureBox1.Image = Properties.Resources.Blue;


            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(340, -2);
            pictureBox1.Size = new System.Drawing.Size(35, 35);
            AddMouseEventsToAllControls(this);
        }
        private void AddMouseEventsToAllControls(Control parent)
        {
            if (parent is Button || parent is PictureBox || parent is DataGridView)
                return;

            parent.MouseDown += credentials_groups_MouseDown;
            parent.MouseMove += credentials_groups_MouseMove;
            parent.MouseUp += credentials_groups_MouseUp;

            foreach (Control ctrl in parent.Controls)
            {
                AddMouseEventsToAllControls(ctrl);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Item selectedItem = comboBox1.SelectedItem as Item;

            try
            {
                conn.Open();

                int Old_group_id = 0;
                int New_group_id = 0;
                int notUpdatedCount = 0;

                foreach (int id in ids)
                {

                    string OldQuery = "SELECT group_id FROM credentials WHERE id = @id AND user_id = @user_id";
                    using (MySqlCommand cmd1 = new MySqlCommand(OldQuery, conn))
                    {
                        cmd1.Parameters.AddWithValue("@id", id);
                        cmd1.Parameters.AddWithValue("@user_id", user_id);
                        object result = cmd1.ExecuteScalar();
                        Old_group_id = result == DBNull.Value ? -1 : Convert.ToInt32(result);
                    }



                    string query = "UPDATE credentials SET group_id = @group_id WHERE id = @id AND user_id = @user_id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (selectedItem.Id == 0)
                        {
                            cmd.Parameters.AddWithValue("@group_id", DBNull.Value);
                            New_group_id = -1;
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@group_id", selectedItem.Id);
                            New_group_id = selectedItem.Id;
                        }

                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@user_id", user_id);

                        cmd.ExecuteNonQuery();

                    }
                    if (Old_group_id == New_group_id)
                    {
                        notUpdatedCount++;
                    }



                }

                if (notUpdatedCount == ids.Length)
                {
                    Form messagebox = new MyMessageBox(
                        "No credentials were updated. You could not to update cretentials  group.",
                        "Warning",
                        MessageBoxIcon.Warning);
                    messagebox.ShowDialog();
                    this.DialogResult = DialogResult.None;
                    return;
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Error assigning group: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
                this.DialogResult = DialogResult.None;
            }
            finally
            {
                conn.Close();
                if (this.DialogResult == DialogResult.OK)
                {
                    this.Close();
                }
            }
        }

        private void credentials_groups_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = Cursor.Position;
        }

        private void credentials_groups_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point current = Cursor.Position;
                this.Location = new Point(
                    this.Location.X + (current.X - lastLocation.X),
                    this.Location.Y + (current.Y - lastLocation.Y));

                lastLocation = current;
            }
        }

        private void credentials_groups_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            this.Close();

        }


        public class Item
        {
            public string Name { get; set; }
            public int Id { get; set; }

            public int User_Id { get; set; }

            public override string ToString()
            {
                return Name;
            }

            public Item(int id, string name, int user_id)
            {
                Name = name;
                Id = id;
                User_Id = user_id;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string newGroup = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(newGroup))
            {
                Form messagebox = new MyMessageBox("You must enter a new group name", "Warning", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
                this.DialogResult = DialogResult.None;
                return;
            }

            if (newGroup == "All" || newGroup == "Without group")
            {
                Form messagebox = new MyMessageBox($"You must enter other group name than {newGroup}", 
                                                    "Warning", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
                this.DialogResult = DialogResult.None;
                return;
            }



            try
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM credentials_groups WHERE name = @name AND user_id = @user_id";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@name", newGroup);
                    checkCmd.Parameters.AddWithValue("@user_id", user_id);

                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (exists > 0)
                    {
                        Form messagebox = new MyMessageBox("This group already exists.", "Warning", MessageBoxIcon.Warning);
                        messagebox.ShowDialog();
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                }

                string insertQuery = "INSERT INTO credentials_groups(name, user_id) VALUES(@name, @user_id); SELECT LAST_INSERT_ID();";
                int newGroupId;

                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@name", newGroup);
                    insertCmd.Parameters.AddWithValue("@user_id", user_id);
                    newGroupId = Convert.ToInt32(insertCmd.ExecuteScalar());
                }

                string updateQuery = "UPDATE credentials SET group_id = @group_id WHERE id = @id AND user_id = @user_id";

                foreach (int id in ids)
                {
                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@group_id", newGroupId);
                        updateCmd.Parameters.AddWithValue("@id", id);
                        updateCmd.Parameters.AddWithValue("@user_id", user_id);
                        updateCmd.ExecuteNonQuery();
                    }
                }

                this.Close();
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Error creating group: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            catch (Exception ex)
            {
                Form messagebox = new MyMessageBox("An error occurred: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Back || e.Control && e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

            }
        }
    }
}
