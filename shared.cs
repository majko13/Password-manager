using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Password_manager
{
    public partial class shared : Form
    {
        private MySqlConnection conn;
        private string connectionString;
        private bool showPasswords = false;
        private int user_id;

        private bool mouseDown;
        private Point lastLocation;

        private void comboBox_load()
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                List<Item> users = new List<Item>();

                string query = @"
            SELECT DISTINCT u.id, u.username
            FROM shared_groups sg
            INNER JOIN credentials_groups cg ON sg.group_id = cg.id
            INNER JOIN users u ON cg.user_id = u.id
            WHERE sg.reciever_id = @receiver_id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@receiver_id", user_id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new Item(
                                Convert.ToInt32(reader["id"]),
                                reader["username"].ToString(),
                                0
                                ));
                        }
                    }
                }

                comboBox1.DataSource = users;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "Id";

                if (users.Count == 0)
                {
                    comboBox1.Enabled = false;
                    comboBox1.Text = "No users shared anything";
                }
                else
                {
                    comboBox1.Enabled = true;
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Database error: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }
        private void load()
        {
            dataGridView1.Rows.Clear();
            try
            {
                Item selectedUser = comboBox1.SelectedItem as Item;

                if (selectedUser == null)
                    return;

                conn.Close();
                conn.Open();

                string query = @"
            SELECT sg.group_id, cg.user_id AS sender_id, u.username AS sender_name, cg.name
            FROM shared_groups sg
            INNER JOIN credentials_groups cg ON sg.group_id = cg.id
            INNER JOIN users u ON cg.user_id = u.id  
            WHERE sg.reciever_id = @current_user_id
            AND cg.user_id = @selected_user_id;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@current_user_id", user_id);
                cmd.Parameters.AddWithValue("@selected_user_id", selectedUser.Id);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dataGridView1.Rows.Add(
                        reader["group_id"],
                        reader["sender_name"],
                        reader["name"]
                    );
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Error while loading shared groups: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }

        public shared(int userId)
        {
            InitializeComponent();
            this.user_id = userId;
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            comboBox_load();
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }


            AddMouseEventsToAllControls(this);
        }
        private void AddMouseEventsToAllControls(Control parent)
        {
            if (parent is Button || parent is PictureBox || parent is DataGridView)
                return;

            parent.MouseDown += shared_MouseDown;
            parent.MouseMove += shared_MouseMove;
            parent.MouseUp += shared_MouseUp;

            foreach (Control ctrl in parent.Controls)
            {
                AddMouseEventsToAllControls(ctrl);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            load();
        }




        private void shared_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = Cursor.Position;
        }

        private void shared_MouseMove(object sender, MouseEventArgs e)
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

        private void shared_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
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







        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                Form messagebox = new MyMessageBox("Please select a group to accept.", "Warning", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
                return;
            }

            int groupId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
            string groupName = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM credentials_groups WHERE user_id = @user_id AND name = @name";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@user_id", user_id);
                    checkCmd.Parameters.AddWithValue("@name", groupName);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        DialogResult result = new MyMessageBox(
                            "You already have a group with this name.\n\nDo you want to delete the shared group?",
                            "Group Already Exists",
                            MessageBoxIcon.Warning,
                            MessageBoxButtons.YesNo).ShowDialog();

                        if (result == DialogResult.Yes)
                        {
                            string delQuery = "DELETE FROM shared_groups WHERE group_id = @group_id AND reciever_id = @receiver_id";
                            using (MySqlCommand deleteCmd = new MySqlCommand(delQuery, conn))
                            {
                                deleteCmd.Parameters.AddWithValue("@group_id", groupId);
                                deleteCmd.Parameters.AddWithValue("@receiver_id", user_id);
                                deleteCmd.ExecuteNonQuery();
                            }

                            dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);
                        }
                        return;
                    }
                }

                string insertQuery = "INSERT INTO credentials_groups (name, user_id) VALUES (@name, @user_id)";

                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@name", groupName);
                    insertCmd.Parameters.AddWithValue("@user_id", user_id);
                    insertCmd.ExecuteNonQuery();
                }

                string deleteQuery = "DELETE FROM shared_groups WHERE group_id = @group_id AND reciever_id = @receiver_id";
                using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@group_id", groupId);
                    deleteCmd.Parameters.AddWithValue("@receiver_id", user_id);
                    deleteCmd.ExecuteNonQuery();
                }

                Form messagebox = new MyMessageBox("Group was successfully accepted!", "Success", MessageBoxIcon.Information);
                messagebox.ShowDialog();


                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);

                if (comboBox1.Items.Count > 0)
                {
                    comboBox_load();
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Database error: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                Form messagebox = new MyMessageBox("Please select a group to remove from shared.", "Warning", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
                return;
            }

            int groupId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
            string groupName = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();

            DialogResult result = new MyMessageBox(
                 $"Do you really want to remove the group '{groupName}' from the sharing offer?",
                 "Confirm Removal",
                 MessageBoxIcon.Question,
                 MessageBoxButtons.YesNo).ShowDialog();

            if (result != DialogResult.Yes)
                return;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                string deleteQuery = "DELETE FROM shared_groups WHERE group_id = @group_id AND reciever_id = @receiver_id";

                using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@group_id", groupId);
                    deleteCmd.Parameters.AddWithValue("@receiver_id", user_id);

                    int rowsAffected = deleteCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Form messagebox = new MyMessageBox("Group has been removed from the sharing offer.", "Success", MessageBoxIcon.Information);
                        messagebox.ShowDialog();

                        dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);

                        if (dataGridView1.Rows.Count == 0)
                        {
                            comboBox_load();
                        }
                    }
                    else
                    {
                        Form messagebox = new MyMessageBox("Failed to remove the group.", "Error", MessageBoxIcon.Error);
                        messagebox.ShowDialog();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Database error: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
