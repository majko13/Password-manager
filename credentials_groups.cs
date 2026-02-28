using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                string query = "SELECT * FROM credentials_groups WHERE user_id = @user_id";
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

                if (items.Count > 0)
                {
                    comboBox1.DataSource = items;
                    comboBox1.DisplayMember = "Name";
                    comboBox1.SelectedIndex = 0;
                    button1.Enabled = true;
                    comboBox1.Enabled = true;
                }
                else
                {
                    // Nie sú žiadne skupiny
                    button1.Enabled = false;
                    comboBox1.Enabled = false;
                    comboBox1.Text = "Žiadne skupiny";
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Chyba při načítání skupin: " + ex.Message, "Chyba", MessageBoxIcon.Error);
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
        }




        private void button1_Click(object sender, EventArgs e)
        {
            Item selectedItem = comboBox1.SelectedItem as Item;
            if (selectedItem == null) return;

            try
            {
                conn.Open();

                string query = "UPDATE credentials SET group_id = @group_id WHERE id = @id AND user_id = @user_id";

                foreach (int id in ids)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@group_id", selectedItem.Id);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@user_id", user_id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Chyba při přiřazování skupiny", "Chyba", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                conn.Close();
                this.Close();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string newGroup = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(newGroup))
            {
                Form messagebox = new MyMessageBox("Musíte zadat název nové skupiny", "Upozornění", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
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
                        Form messagebox = new MyMessageBox("Tato skupina už existuje.", "Varování", MessageBoxIcon.Warning);
                        messagebox.ShowDialog();
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
                Form messagebox = new MyMessageBox("Chyba při vytváření skupiny", "Chyba", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            catch (Exception ex)
            {
                Form messagebox = new MyMessageBox("Došlo k chybě", "Chyba", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }

        private void credentials_groups_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void credentials_groups_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void credentials_groups_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
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
    }
}
