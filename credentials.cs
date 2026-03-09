using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;

namespace Password_manager
{
    public partial class credentials : Form
    {

        private MySqlConnection conn;
        private string connectionString;
        private int user_id;
        private bool mouseDown;
        private Point lastLocation;
        private bool closeButtonClicked = false;

        private byte[] userSalt;
        private string masterPassword;


        private string DecryptPasswordWithMasterKey(byte[] encryptedBytes, byte[] iv)
        {
            string masterPassword = null;
            try
            {
                masterPassword = SecurePasswordManager.GetMasterPasswordAsString();
                if (string.IsNullOrEmpty(masterPassword) || userSalt == null)
                {
                    return "***NOT LOGGED IN***";
                }

                if (iv == null || iv.Length == 0)
                {
                    return "***INVALID IV - OLD RECORD***";
                }

                byte[] key = SecureEncryptor.DeriveKeyFromPassword(masterPassword, userSalt);

                return SecureEncryptor.Decrypt(encryptedBytes, key, iv);
            }
            catch (CryptographicException)
            {
                return "***WRONG KEY***";

            }
            finally
            {
                if (masterPassword != null)
                {
                    SecurePasswordManager.ClearString(ref masterPassword);
                }
            }

        }
        private void comboBox_load()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    List<Item> items = new List<Item>();

                    string query = "SELECT * FROM credentials_groups WHERE user_id = @user_id";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@user_id", user_id);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            items.Add(new Item(-1, "all", -1));
                            items.Add(new Item(0, "without group", 0));

                            while (reader.Read())
                            {
                                items.Add(new Item(
                                    Convert.ToInt32(reader["id"]),
                                    reader["name"].ToString(),
                                    Convert.ToInt32(reader["user_id"])));
                            }
                        }
                    }

                    comboBox1.DataSource = items;
                    comboBox1.DisplayMember = "Name";
                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Error loading groups: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
        }
        private async void load()
        {
            dataGridView1.Rows.Clear();

            try
            {
                Item selectedItem = comboBox1.SelectedItem as Item;
                conn.Close();
                var result = await Task.Run(() =>
                {
                    var rows = new List<DataGridViewRow>();
                    bool isAdmin = false;

                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        string query;
                        MySqlCommand cmd;

                        if (selectedItem.Id == -1 && selectedItem.User_Id == -1)
                        {
                            query = @"SELECT c.id, c.username, c.password, c.iv, c.url, cg.name 
                             FROM credentials c
                             LEFT JOIN credentials_groups cg ON c.group_id = cg.id
                             WHERE c.user_id = @user_id";
                            cmd = new MySqlCommand(query, connection);
                            cmd.Parameters.AddWithValue("@user_id", user_id);
                        }
                        else if (selectedItem.Id == 0 && selectedItem.User_Id == 0)
                        {
                            query = @"SELECT c.id, c.username, c.password, c.iv, c.url, cg.name 
                             FROM credentials c
                             LEFT JOIN credentials_groups cg ON c.group_id = cg.id
                             WHERE c.user_id = @user_id AND c.group_id IS NULL";
                            cmd = new MySqlCommand(query, connection);
                            cmd.Parameters.AddWithValue("@user_id", user_id);
                        }
                        else
                        {
                            query = @"SELECT c.id, c.username, c.password, c.iv, c.url, cg.name 
                             FROM credentials c
                             LEFT JOIN credentials_groups cg ON c.group_id = cg.id
                             WHERE c.user_id = @user_id 
                               AND c.group_id = @group_id 
                               AND cg.user_id = @group_user_id";
                            cmd = new MySqlCommand(query, connection);
                            cmd.Parameters.AddWithValue("@user_id", user_id);
                            cmd.Parameters.AddWithValue("@group_id", selectedItem.Id);
                            cmd.Parameters.AddWithValue("@group_user_id", selectedItem.User_Id);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                byte[] encryptedBytes = (byte[])reader["password"];
                                byte[] iv = (byte[])reader["iv"];

                                string password = DecryptPasswordWithMasterKey(encryptedBytes, iv);

                                var row = new DataGridViewRow();
                                row.CreateCells(dataGridView1,
                                    reader["id"],
                                    reader["username"],
                                    password,
                                    reader["url"],
                                    reader["name"] ?? "");

                                rows.Add(row);
                            }
                        }

                        string roleQuery = "SELECT role_id FROM users WHERE id = @user_id LIMIT 1";
                        using (var roleCmd = new MySqlCommand(roleQuery, connection))
                        {
                            roleCmd.Parameters.AddWithValue("@user_id", user_id);
                            var roleId = roleCmd.ExecuteScalar();
                            isAdmin = (roleId != null && Convert.ToInt32(roleId) == 1);
                        }
                    }

                    return new { Rows = rows, IsAdmin = isAdmin };
                });

                dataGridView1.Rows.AddRange(result.Rows.ToArray());
                button6.Visible = result.IsAdmin;
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Error loading data: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
        }



        public credentials(int id, string username)
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            user_id = id;

            masterPassword = SecurePasswordManager.GetMasterPasswordAsString();
            userSalt = SecurePasswordManager.UserSalt;

            if (string.IsNullOrEmpty(masterPassword) || userSalt == null)
            {
                Form messagebox = new MyMessageBox("Error: Invalid login credentials", "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
                this.Close();
                return;
            }
            comboBox_load();

            label3.Text = "Acount: " + username;
            pictureBox1.SendToBack();

            pictureBox1.Image = Properties.Resources.Blue;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(992, 0);
            pictureBox1.Size = new System.Drawing.Size(35, 35);

            AddMouseEventsToAllControls(this);
        }
        private void AddMouseEventsToAllControls(Control parent)
        {
            if (parent is Button || parent is PictureBox || parent is DataGridView)
                return;

            parent.MouseDown += credentials_MouseDown;
            parent.MouseMove += credentials_MouseMove;
            parent.MouseUp += credentials_MouseUp;

            foreach (Control ctrl in parent.Controls)
            {
                AddMouseEventsToAllControls(ctrl);
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (new MyMessageBox("Do you really want to close the application?", "Close", MessageBoxIcon.Question, MessageBoxButtons.YesNo).ShowDialog() == DialogResult.Yes)
            {
                Application.Exit();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form credentials_add = new credentials_add(user_id);
            credentials_add.ShowDialog();
            credentials_add.FormClosed += delegate
            {
                load();
            };
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

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                e.PaintBackground(e.CellBounds, true);

                var cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string cellValue = cell.Value?.ToString();

                if (string.IsNullOrEmpty(cellValue))
                {
                    e.PaintContent(e.CellBounds);
                    e.Handled = true;
                    return;
                }

                string maskedPassword = new string('•', cellValue.Length);

                TextRenderer.DrawText(e.Graphics, maskedPassword,
                                      e.CellStyle.Font,
                                      new Rectangle(e.CellBounds.X + 2, e.CellBounds.Y,
                                                   e.CellBounds.Width - 20, e.CellBounds.Height),
                                      e.CellStyle.ForeColor,
                                      TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                if (icons != null && icons.Images.Count > 0)
                {
                    int eyeSize = Math.Min(e.CellBounds.Height - 4, 16);
                    int eyeX = e.CellBounds.Right - eyeSize - 2;
                    int eyeY = e.CellBounds.Top + (e.CellBounds.Height - eyeSize) / 2;

                    e.Graphics.DrawImage(icons.Images[0], eyeX, eyeY, eyeSize, eyeSize);
                }

                e.Handled = true;
            }
        }

        private void credentials_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = Cursor.Position;
        }

        private void credentials_MouseMove(object sender, MouseEventArgs e)
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

        private void credentials_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int row = dataGridView1.CurrentCell.RowIndex;
                string value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex]?.Value?.ToString();
                string str = dataGridView1.Rows[row].Cells[0]?.Value?.ToString();
                string columnName = this.dataGridView1.Columns[e.ColumnIndex].Name;

                if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(str)) return;

                conn.Open();

                if (!int.TryParse(str, out int id)) return;

                if (columnName == "Column2")
                {
                    string currentMasterPassword = SecurePasswordManager.GetMasterPasswordAsString();
                    try
                    {
                        if (!string.IsNullOrEmpty(currentMasterPassword) && userSalt != null)
                        {
                            byte[] key = SecureEncryptor.DeriveKeyFromPassword(currentMasterPassword, userSalt);
                            byte[] iv = SecureEncryptor.GenerateRandomIV();
                            byte[] encryptedBytes = SecureEncryptor.Encrypt(value, key, iv);

                            string updateQuery = "UPDATE credentials SET password = @password, iv = @iv WHERE id = @id";
                            using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                            {
                                cmd.Parameters.Add("@password", MySqlDbType.VarBinary, -1).Value = encryptedBytes;
                                cmd.Parameters.Add("@iv", MySqlDbType.VarBinary, 16).Value = iv;
                                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            Form messagebox = new MyMessageBox("You are not logged in!", "Error", MessageBoxIcon.Error);
                            messagebox.ShowDialog();
                        }
                    }
                    finally
                    {
                        SecurePasswordManager.ClearString(ref currentMasterPassword);
                    }
                }
                else
                {
                    string dbColumnName;
                    switch (columnName)
                    {
                        case "Column1":
                            dbColumnName = "username";
                            break;
                        case "Column4":
                            dbColumnName = "url";
                            break;
                        default:
                            dbColumnName = "name";
                            break;
                    }

                    string[] allowedColumns = { "username", "url", "name" };
                    bool isAllowed = false;
                    foreach (string col in allowedColumns)
                    {
                        if (col == dbColumnName)
                        {
                            isAllowed = true;
                            break;
                        }
                    }

                    if (isAllowed)
                    {
                        string updateQuery = $"UPDATE credentials SET {dbColumnName} = @value WHERE id = @id";
                        using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@value", value);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Database error saving data: " + ex.Message, "Database error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            catch (Exception ex)
            {
                Form messagebox = new MyMessageBox("Error saving data: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {

            if (DialogResult.Yes == new MyMessageBox("Do you really want to log out?", "Logout", MessageBoxIcon.Question, MessageBoxButtons.YesNo).ShowDialog())
            {
                if (masterPassword != null)
                {
                    SecurePasswordManager.ClearString(ref masterPassword);
                    masterPassword = null;
                }
                if (userSalt != null)
                {
                    Array.Clear(userSalt, 0, userSalt.Length);
                    userSalt = null;
                }

                closeButtonClicked = true;
                this.Close();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != new MyMessageBox(
        "Do you really want to delete the selected records?\n\n" +
        "This action cannot be undone!",
        "Delete Records",
        MessageBoxIcon.Warning,  // Výkričník pre zdôraznení následkov
        MessageBoxButtons.YesNo).ShowDialog())
            {
                return;
            }
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (!int.TryParse(row.Cells[0].Value?.ToString(), out int id)) continue;

                try
                {
                    conn.Open();

                    string deleteQuery = "DELETE FROM credentials WHERE id = @id";
                    using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@id", id);
                        deleteCmd.ExecuteNonQuery();
                    }

                    dataGridView1.Rows.Remove(row);
                }
                catch (Exception ex)
                {
                    Form messagebox = new MyMessageBox("Error deleting record: " + ex.Message, "Error", MessageBoxIcon.Error);
                    messagebox.ShowDialog();
                }
                finally
                {
                    conn.Close();
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int[] ids = new int[dataGridView1.SelectedRows.Count];
            if (dataGridView1.SelectedRows.Count > 0)
            {
                for (int index = dataGridView1.SelectedRows.Count - 1; index >= 0; index--)
                {
                    int i = dataGridView1.SelectedRows[index].Index;

                    string str = dataGridView1.Rows[i].Cells[0].Value.ToString();

                    if (int.TryParse(str, out int id))
                    {
                        ids[index] = id;
                    }


                }
                Form credentials_groups = new credentials_groups(ids, user_id);
                credentials_groups.ShowDialog();
                credentials_groups.FormClosed += delegate
                {

                    comboBox_load();

                };
            }
            else
            {
                Form messagebox = new MyMessageBox("You don't have any row selected.", "Information", MessageBoxIcon.Information);
                messagebox.ShowDialog();
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            load();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count != 2)
            {
                int foundGroupId = -1;
                try
                {
                    conn.Open();

                    string query = "SELECT group_id FROM credentials WHERE user_id = @user_id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", user_id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                foundGroupId = Convert.ToInt32(reader["group_id"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Form messagebox = new MyMessageBox("Error loading groups: " + ex.Message, "Error", MessageBoxIcon.Error);
                    messagebox.ShowDialog();
                    return;
                }
                finally
                {
                    conn.Close();
                }

                if (foundGroupId != -1)
                {
                    Form share = new share(user_id);
                    share.ShowDialog();
                }
            }
            else
            {
                Form messagebox = new MyMessageBox("You don't have any groups to share.", "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form shared = new shared(user_id);
            shared.ShowDialog();

            comboBox_load();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                new MyMessageBox("Please select a group to delete.", "Information", MessageBoxIcon.Information).ShowDialog();
                return;
            }

            if (comboBox1.SelectedIndex <= 1)
            {
                new MyMessageBox(
                    "You cannot delete this item.\n\n" +
                    "Please select a specific group to delete.",
                    "Invalid Selection",
                    MessageBoxIcon.Warning).ShowDialog();
                return;
            }

            Item selectedGroup = comboBox1.SelectedItem as Item;

            if (selectedGroup == null)
            {
                new MyMessageBox("Error selecting group.", "Error", MessageBoxIcon.Error).ShowDialog();
                return;
            }


            DialogResult result = new MyMessageBox(
                $"Do you really want to delete the group '{selectedGroup.Name}'?\n\n" +
                $"Passwords will be automatically moved to 'Without group' (group_id = NULL).\n\n" +
                "This action cannot be undone!",
                "Delete Group",
                MessageBoxIcon.Question, MessageBoxButtons.YesNo).ShowDialog();// Show confirmation dialog

            if (result != DialogResult.Yes)
                return;

            try
            {
                conn.Open();

                string deleteGroupQuery = "DELETE FROM credentials_groups WHERE id = @id AND user_id = @user_id";
                int deletedGroup;

                using (MySqlCommand deleteGroupCmd = new MySqlCommand(deleteGroupQuery, conn))
                {
                    deleteGroupCmd.Parameters.AddWithValue("@id", selectedGroup.Id);
                    deleteGroupCmd.Parameters.AddWithValue("@user_id", user_id);
                    deletedGroup = deleteGroupCmd.ExecuteNonQuery();
                }

                if (deletedGroup > 0)
                {


                    Form messagebox = new MyMessageBox($"Group '{selectedGroup.Name}' has been successfully deleted.\n", "Deletion Successful", MessageBoxIcon.Information);
                    messagebox.ShowDialog();

                    conn.Close();
                    comboBox_load();
                    if (comboBox1.Items.Count > 1)
                    {
                        comboBox1.SelectedIndex = 1;
                    }
                }
                else
                {
                    Form messagebox = new MyMessageBox("Failed to delete the group.", "Error", MessageBoxIcon.Error);
                    messagebox.ShowDialog();
                }
            }
            catch (Exception) { }
            finally
            {
                conn.Close();

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            users usersForm = new users();
            usersForm.ShowDialog();
        }
    }
}
