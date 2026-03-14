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

        bool updated = false;

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
                    comboBox1.DropDownStyle = ComboBoxStyle.Simple;
                    button1.Enabled = false;
                    button3.Enabled = false;
                    comboBox1.Text = "No users shared anything";
                }
                else
                {
                    comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
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
            AND cg.user_id = @selected_user_id ORDER BY name ASC";

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


            pictureBox1.Image = Properties.Resources.Blue;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(668, 0);
            pictureBox1.Size = new System.Drawing.Size(35, 35);
            pictureBox1.SendToBack();

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
            if (updated)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.None;
            }
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
                Form messagebox = new MyMessageBox("Please select at least one group to accept.",
                                    "Warning", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
                return;
            }

            List<int> successfulGroups = new List<int>();
            List<string> removedGroups = new List<string>();
            List<DataGridViewRow> rowsToRemove = new List<DataGridViewRow>();

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                {

                    int groupId = Convert.ToInt32(selectedRow.Cells[0].Value);
                    string groupName = selectedRow.Cells[2].Value.ToString();

                    try
                    {
                        string checkQuery = "SELECT COUNT(*) FROM credentials_groups WHERE user_id = @user_id AND name = @name";
                        using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@user_id", user_id);
                            checkCmd.Parameters.AddWithValue("@name", groupName);

                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                            if (count > 0)
                            {
                                DialogResult result = new MyMessageBox(
                                    $"Group '{groupName}' already exists.\n\nDo you want to delete the shared group?",
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

                                    rowsToRemove.Add(selectedRow);
                                    removedGroups.Add(groupName);                
                                }
                                continue;
                            }
                        }

                        string insertQuery = "INSERT INTO credentials_groups (name, user_id) VALUES (@name, @user_id)";
                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@name", groupName);
                            insertCmd.Parameters.AddWithValue("@user_id", user_id);
                            insertCmd.ExecuteNonQuery();
                        }
                        updated = true;

                        string deleteQuery = "DELETE FROM shared_groups WHERE group_id = @group_id AND reciever_id = @receiver_id";
                        using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@group_id", groupId);
                            deleteCmd.Parameters.AddWithValue("@receiver_id", user_id);
                            deleteCmd.ExecuteNonQuery();
                        }

                        rowsToRemove.Add(selectedRow);
                        successfulGroups.Add(groupId);
                    }
                    catch (Exception ex)
                    {
                        Form messagebox = new MyMessageBox("Error:\n" + ex.Message, "Error", MessageBoxIcon.Error);
                        messagebox.ShowDialog();
                    }
                }

                foreach (DataGridViewRow row in rowsToRemove)
                {
                    dataGridView1.Rows.Remove(row);
                }

                if (successfulGroups.Count > 0)
                {
                    if (removedGroups.Count == 0)
                    {
                        Form successBox = new MyMessageBox(
                            $"Successfully accepted {successfulGroups.Count} group(s)!",
                            "Success",
                            MessageBoxIcon.Information);
                        successBox.ShowDialog();
                    }
                    else
                    {
                        Form partialSuccessBox = new MyMessageBox(
                            $"Accepted: {successfulGroups.Count} group(s)\n" +
                            $"Deleted: {removedGroups.Count} group(s)\n\n" +
                            $"Deleted groups: {string.Join(", ", removedGroups)}",
                            "Partial Success",
                            MessageBoxIcon.Warning);
                        partialSuccessBox.ShowDialog();
                    }
                }
                else if (rowsToRemove.Count > 0)
                {
                    Form errorBox = new MyMessageBox(
                        $"Delete groups: {string.Join(", ", removedGroups)}.\n",
                        "Information",
                        MessageBoxIcon.Information);
                    errorBox.ShowDialog();
                }

            

                if (comboBox1.Items.Count > 0 && successfulGroups.Count > 0)
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
                Form messagebox = new MyMessageBox("Please select at least one group to remove from shared.",
                                    "Warning", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
                return;
            }

            List<(int Id, string Name)> selectedGroups = new List<(int, string)>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.IsNewRow) continue;

                int groupId = Convert.ToInt32(row.Cells[0].Value);
                string groupName = row.Cells[2].Value.ToString();
                selectedGroups.Add((groupId, groupName));
            }

            if (selectedGroups.Count == 0)
                return;

            DialogResult result = new MyMessageBox(
                 $"Do you really want to remove {selectedGroups.Count} group(s) from the sharing offer?\n\n" +
                 $"Groups: {string.Join(", ", selectedGroups.Select(g=>$"'{g.Name}'"))}.",
                 "Confirm Removal",
                 MessageBoxIcon.Question,
                 MessageBoxButtons.YesNo).ShowDialog();

            if (result != DialogResult.Yes)
                return;

            List<string> successfullyRemoved = new List<string>();
            List<DataGridViewRow> rowsToRemove = new List<DataGridViewRow>();

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                try
                {
                    foreach (var group in selectedGroups)
                    {
                        string deleteQuery = "DELETE FROM shared_groups WHERE group_id = @group_id " +
                                                            "AND reciever_id = @receiver_id";
                        using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@group_id", group.Id);
                            deleteCmd.Parameters.AddWithValue("@receiver_id", user_id);

                            int rowsAffected = deleteCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                successfullyRemoved.Add(group.Name);
                                DataGridViewRow rowToRemove = dataGridView1.SelectedRows
                                    .Cast<DataGridViewRow>()
                                    .FirstOrDefault(r => Convert.ToInt32(r.Cells[0].Value) == group.Id);

                                if (rowToRemove != null)
                                    rowsToRemove.Add(rowToRemove);
                            }
                        }
                    }
                    
                    foreach (DataGridViewRow row in rowsToRemove)
                    {
                        dataGridView1.Rows.Remove(row);
                    }

                    if (successfullyRemoved.Count > 0)
                    {
                        Form messagebox = new MyMessageBox(
                            $"Successfully removed: {successfullyRemoved.Count} group(s)",
                            "Information",
                            MessageBoxIcon.Information);
                        messagebox.ShowDialog();
                    }

                    if (dataGridView1.Rows.Count == 0 || dataGridView1.Rows.Cast<DataGridViewRow>().All(r => r.IsNewRow))
                    {
                        comboBox_load();
                    }
                }
                catch (Exception ex)
                {
                    Form messagebox = new MyMessageBox(
                        $"Error during removal: {ex.Message}",
                        "Error",
                        MessageBoxIcon.Error);
                    messagebox.ShowDialog();

                    Console.WriteLine($"Error removing groups: {ex.Message}");
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Database error: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            catch (Exception ex)
            {
                Form messagebox = new MyMessageBox("Error: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (updated)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.None;
            }
            this.Close();
        }
    }
}
