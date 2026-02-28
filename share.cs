using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Password_manager
{
    public partial class share : Form
    {
        private MySqlConnection conn;
        private string connectionString;
        private int user_id;
        private int[] group_ids_array;
        private bool initialLoad = true;

        private bool mouseDown;
        private Point lastLocation;

        private void comboBox_users_Load()
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                comboBox2.DataSource = null;

                // Získaj aktuálny index
                int currentComboBoxIndex = comboBox1.SelectedIndex;

                if (currentComboBoxIndex < 0 || group_ids_array == null || currentComboBoxIndex >= group_ids_array.Length)
                {
                    comboBox2.Enabled = false;
                    button1.Enabled = false;
                    return;
                }

                int selectedGroupId = group_ids_array[currentComboBoxIndex];
                List<Item_2> items = new List<Item_2>();

                // Získaj všetkých používateľov okrem aktuálneho
                string query = "SELECT id, username FROM users WHERE id != @user_id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = Convert.ToInt32(reader["id"]);
                            string username = reader["username"].ToString();
                            items.Add(new Item_2(userId, username));
                        }
                    }
                }

                // ✅ SPRÁVNE: Získaj používateľov, ktorí UŽ DOSTALI TÚTO KONKRÉTNU skupinu
                query = "SELECT reciever_id FROM shared_groups WHERE group_id = @group_id";
                List<int> alreadySharedUsers = new List<int>();

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@group_id", selectedGroupId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // ✅ Opravené: čítame reciever_id, nie user_id
                            alreadySharedUsers.Add(Convert.ToInt32(reader["reciever_id"]));
                        }
                    }
                }

                // Odstráň používateľov, ktorí už DOSTALI TÚTO skupinu
                items.RemoveAll(item => alreadySharedUsers.Contains(item.Id));

                comboBox2.DataSource = items;

                if (comboBox2.Items.Count == 0)
                {
                    button1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox2.Text = "Žiadni používatelia na zdielanie";
                }
                else
                {
                    comboBox2.Enabled = true;
                    button1.Enabled = true;
                    comboBox2.DisplayMember = "Name";
                    comboBox2.ValueMember = "Id";
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Chyba databázy: " + ex.Message);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("Chyba: " + ex.Message);
            }
        }

        private void comboBox_groups_Load()
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                List<Item> items = new List<Item>();

                string query = "SELECT id, name, user_id FROM credentials_groups WHERE user_id = @user_id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
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
                }

                comboBox1.DataSource = items;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "Id";

                // Naplní pole s ID skupín
                group_ids_array = new int[items.Count];
                for (int i = 0; i < items.Count; i++)
                {
                    group_ids_array[i] = items[i].Id;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Chyba databázy: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba: " + ex.Message);
            }
        }

        public share(int user_id)
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            this.user_id = user_id;

            // Nastavenie pictureBox
            pictureBox1.Image = Properties.Resources.Blue;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new Point(325, -2);
            pictureBox1.Size = new Size(35, 35);
            pictureBox1.SendToBack();

            // Načítanie dát
            comboBox_groups_Load();
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            comboBox_users_Load();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Vyberte používateľa");
                return;
            }

            Item_2 selectedItem = comboBox2.SelectedItem as Item_2;
            int currentComboBoxIndex = comboBox1.SelectedIndex;

            if (currentComboBoxIndex < 0 || group_ids_array == null || currentComboBoxIndex >= group_ids_array.Length)
            {
                MessageBox.Show("Nie je vybraná žiadna skupina");
                return;
            }

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                string query = "INSERT INTO shared_groups (reciever_id, group_id) VALUES (@reciever_id, @group_id)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@reciever_id", selectedItem.Id);
                    cmd.Parameters.AddWithValue("@group_id", group_ids_array[currentComboBoxIndex]);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Skupina bola úspešne zdielaná");
                        this.Close();
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Kontrola duplicity
                if (ex.Number == 1062) // MySQL error code for duplicate entry
                {
                    MessageBox.Show("Táto skupina už je zdielaná tomuto používateľovi");
                }
                else
                {
                    MessageBox.Show("Chyba databázy: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Chyba: " + ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initialLoad && comboBox1.SelectedIndex >= 0)
            {
                comboBox_users_Load();
            }
            else
            {
                initialLoad = false;
            }
        }

        private void share_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void share_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X,
                    (this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();
            }
        }

        private void share_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        public class Item
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public int User_Id { get; set; }

            public override string ToString() => Name;

            public Item(int id, string name, int user_id)
            {
                Id = id;
                Name = name;
                User_Id = user_id;
            }
        }

        public class Item_2
        {
            public string Name { get; set; }
            public int Id { get; set; }

            public override string ToString() => Name;

            public Item_2(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }
    }
}