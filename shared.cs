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

        private byte[] userSalt;
        private string masterPassword;
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
                                0 // User_Id nepotrebujeme pre comboBox, len pre Item triedu
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
                MessageBox.Show("Database error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        private void load()
        {
            dataGridView1.Rows.Clear();
            MessageBox.Show("kokot");
            try
            {
                Item selectedUser = comboBox1.SelectedItem as Item;

                if (selectedUser == null)
                    return;

                conn.Close();
                conn.Open();

                string query = @"
                    SELECT sg.group_id, cg.user_id AS sender_id, cg.name
                    FROM shared_groups sg
                    INNER JOIN credentials_groups cg ON sg.group_id = cg.id
                    WHERE sg.reciever_id = @current_user_id
                    AND cg.user_id = @selected_user_id;";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                // PARAMETRE MUSIA MAŤ ROVNAKÉ NÁZVY AKO V QUERY!
                cmd.Parameters.AddWithValue("@current_user_id", user_id);
                cmd.Parameters.AddWithValue("@selected_user_id", selectedUser.Id);


                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dataGridView1.Rows.Add(
                        reader["group_id"],   // ID – skrytý stĺpec
                        reader["sender_id"],  // Sent by
                        reader["name"]        // Group name
                    );
                }
                reader.Close();
            }
            catch (MySqlException)
            {
                Form messagebox = new MyMessageBox("Error while loading shared groups", "Error", MessageBoxIcon.Error);
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
                comboBox1.SelectedIndex = 0; // Toto automaticky spustí SelectedIndexChanged event
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

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 3 && e.RowIndex >= 0)
            {
                e.PaintBackground(e.CellBounds, true);

                if (showPasswords)
                {

                    string password = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
                    e.Graphics.DrawString(password, e.CellStyle.Font, Brushes.Black, e.CellBounds.X + 2, e.CellBounds.Y + 2);
                }
                else
                {

                    string password = new string('*', dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Length);
                    e.Graphics.DrawString(password, e.CellStyle.Font, Brushes.Black, e.CellBounds.X + 2, e.CellBounds.Y + 2);
                }


                Image eyeImage = icons.Images[0];
                int eyeSize = e.CellBounds.Height - 4;
                int eyeX = e.CellBounds.Right - eyeSize - 2;
                int eyeY = e.CellBounds.Y + 2;
                e.Graphics.DrawImage(eyeImage, eyeX, eyeY, eyeSize, eyeSize);

                e.Handled = true;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3 && e.RowIndex >= 0)
            {
                showPasswords = !showPasswords;
                dataGridView1.InvalidateCell(e.ColumnIndex, e.RowIndex);
            }
        }

        private void shared_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void shared_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
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
            // Skontrolujeme, či je vybratý nejaký riadok
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Prosím, vyberte skupinu na prijatie.");
                return;
            }

            // Získame ID skupiny z vybraného riadku (prvý stĺpec je skryté ID)
            int groupId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
            string groupName = dataGridView1.SelectedRows[0].Cells[2].Value.ToString(); // Group name

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                // 1️⃣ Najprv skontrolujeme, či už používateľ nemá skupinu s rovnakým názvom
                string checkQuery = "SELECT COUNT(*) FROM credentials_groups WHERE user_id = @user_id AND name = @name";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@user_id", user_id);
                    checkCmd.Parameters.AddWithValue("@name", groupName);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("Už máte skupinu s týmto názvom.");


                        string delQuery = "DELETE FROM shared_groups WHERE group_id = @group_id AND reciever_id = @receiver_id";
                        using (MySqlCommand deleteCmd = new MySqlCommand(delQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@group_id", groupId);
                            deleteCmd.Parameters.AddWithValue("@receiver_id", user_id);
                            deleteCmd.ExecuteNonQuery();
                        }

                        dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);

                        return;
                    }
                }

                // 2️⃣ Vytvoríme novú skupinu pre aktuálneho používateľa
                string insertQuery = "INSERT INTO credentials_groups (name, user_id) VALUES (@name, @user_id)";

                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@name", groupName);
                    insertCmd.Parameters.AddWithValue("@user_id", user_id);
                    insertCmd.ExecuteNonQuery();
                }

                // 3️⃣ Vymažeme pôvodný záznam zo shared_groups
                string deleteQuery = "DELETE FROM shared_groups WHERE group_id = @group_id AND reciever_id = @receiver_id";
                using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@group_id", groupId);
                    deleteCmd.Parameters.AddWithValue("@receiver_id", user_id);
                    deleteCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Skupina bola úspešne prijatá!");


                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);

                if (comboBox1.Items.Count > 0)
                {
                    comboBox_load();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Chyba databázy: " + ex.Message);
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

        private void button3_Click(object sender, EventArgs e)
        {
            // Skontrolujeme, či je vybratý nejaký riadok v DataGridView
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Prosím, vyberte skupinu na odstránenie zo zdieľaných.");
                return;
            }

            // Získame ID skupiny z vybraného riadku (prvý stĺpec je skryté ID)
            int groupId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value);
            string groupName = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();

            // Opýtame sa používateľa, či si je istý
            DialogResult result = MessageBox.Show(
                $"Naozaj chcete odstrániť skupinu '{groupName}' z ponuky na prijatie?",
                "Potvrdenie odstránenia",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                // Vymažeme záznam zo shared_groups
                string deleteQuery = "DELETE FROM shared_groups WHERE group_id = @group_id AND reciever_id = @receiver_id";

                using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@group_id", groupId);
                    deleteCmd.Parameters.AddWithValue("@receiver_id", user_id);

                    int rowsAffected = deleteCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Skupina bola odstránená z ponuky na prijatie.");

                        dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);

                        if (dataGridView1.Rows.Count == 0)
                        {
                            comboBox_load(); 
                        }
                    }
                    else
                    {
                        MessageBox.Show("Skupinu sa nepodarilo odstrániť.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Chyba databázy: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
