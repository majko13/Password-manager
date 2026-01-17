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
                conn.Open();

                List<Item> items = new List<Item>();

                string query = @"SELECT 
            credentials_groups.id, 
            credentials_groups.name,
            credentials_groups.user_id 
            FROM shared_groups 
            LEFT JOIN credentials_groups ON shared_groups.group_id = credentials_groups.id 
            WHERE shared_groups.user_id = @user_id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        items.Add(new Item(-1, "all", -1));

                        while (reader.Read())
                        {
                            items.Add(new Item(
                                Convert.ToInt32(reader["id"]),
                                reader["name"].ToString(),
                                Convert.ToInt32(reader["user_id"])
                            ));
                        }
                    }
                }

                comboBox1.DataSource = items;
                comboBox1.DisplayMember = "Name";
                comboBox1.SelectedIndex = 0;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("credenials_groups load error: " + ex.Message);
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
                Item selectedItem = comboBox1.SelectedItem as Item;

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();

                byte[] userKey = SecureEncryptor.DeriveKeyFromPassword(masterPassword, userSalt);

                // ZMENENÉ: Načítaj z shared_passwords
                string query = @"SELECT 
            shared_passwords.id as shared_id,
            shared_passwords.credential_id,
            credentials.username as cred_username, 
            shared_passwords.password as encrypted_password, 
            credentials.url, 
            credentials_groups.name as group_name,
            shared_passwords.iv,
            credentials.group_id,
            users.username as owner_name,
            credentials_groups.user_id as group_owner_id
            FROM shared_passwords
            LEFT JOIN credentials ON shared_passwords.credential_id = credentials.id
            LEFT JOIN credentials_groups ON credentials.group_id = credentials_groups.id
            LEFT JOIN users ON credentials_groups.user_id = users.id
            WHERE shared_passwords.user_id = @current_user_id";

                if (selectedItem.Id != -1)
                {
                    query += " AND credentials_groups.id = @group_id AND credentials_groups.user_id = @owner_id";
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@current_user_id", user_id);

                    if (selectedItem.Id != -1)
                    {
                        cmd.Parameters.AddWithValue("@group_id", selectedItem.Id);
                        cmd.Parameters.AddWithValue("@owner_id", selectedItem.User_Id);
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            byte[] encryptedPassword = (byte[])reader["encrypted_password"];
                            byte[] iv = (byte[])reader["iv"];
                            int groupId = Convert.ToInt32(reader["group_id"]);
                            int credentialId = Convert.ToInt32(reader["credential_id"]);

                            string decryptedPassword = "";
                            bool canDecrypt = true;

                            try
                            {
                                // Použi NOVÚ metódu na desifrovanie zdieľaného hesla
                                //decryptedPassword = SecureEncryptor.GetSharedPassword(
                                    //user_id, credentialId, groupId, userKey, conn);
                            }
                            catch (Exception ex)
                            {
                                decryptedPassword = $"[CHYBA: {ex.Message}]";
                                canDecrypt = false;
                            }

                            dataGridView1.Rows.Add(
                                reader["credential_id"],
                                reader["owner_name"],
                                reader["cred_username"],
                                decryptedPassword,
                                reader["url"],
                                reader["group_name"],
                                canDecrypt ? "Áno" : "Nie"
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba pri načítaní: " + ex.Message, "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
        public shared(int user_id, byte[] salt, string masterPwd)
        {
            InitializeComponent();


            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            this.user_id = user_id;
            this.userSalt = salt;
            this.masterPassword = masterPwd;

            pictureBox1.SendToBack();

            pictureBox1.Image = Properties.Resources.Blue;


            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(327, -2);
            pictureBox1.Size = new System.Drawing.Size(35, 35);

            comboBox_load();
            load();

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
    }
}
