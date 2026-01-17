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

                string query = String.Format("SELECT (SELECT COUNT(group_id) FROM credentials WHERE user_id = {0}) AS count", user_id);

                MySqlCommand cmd2 = new MySqlCommand(query, conn);
                MySqlDataReader reader1 = cmd2.ExecuteReader();

                reader1.Read();
                int count = Convert.ToInt32(reader1["count"]);
                reader1.Close();

                if (count > 0)
                {

                    button1.Enabled = true;
                    comboBox1.Enabled = true;

                    List<Item> items = new List<Item>();


                    query = String.Format("SELECT * FROM credentials_groups WHERE user_id = {0}", user_id);
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        items.Add(new Item(Convert.ToInt32(reader["id"]), reader["name"].ToString(), Convert.ToInt32(reader["user_id"])));

                    }
                    comboBox1.DataSource = items;
                    comboBox1.DisplayMember = "Name";
                    comboBox1.SelectedIndex = 0;


                    reader.Close();
                }
                else
                {
                    button1.Enabled = false;
                    comboBox1.Enabled = false;
                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
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


            try
            {

                conn.Open();
                for (int i = 0; i < ids.Length; i++)
                {
                    string query = String.Format("UPDATE credentials SET group_id = '{0}' WHERE id = {1} AND user_id = {2}", selectedItem.Id.ToString(), ids[i], user_id);
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.ExecuteNonQuery();
                }


            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

                conn.Close();
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string newGroup = textBox1.Text;
            if (newGroup == "")
            {
                MessageBox.Show("Musíte zadat název nové skupiny");
            }
            else
            {
                try
                {
                    conn.Open();
                    string query = String.Format("SELECT name FROM credentials_groups WHERE name = '{0}'", newGroup);
                    MySqlCommand cmd = new MySqlCommand(query, conn);


                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        reader.Close();
                        throw new Exception("Tato skupina už existuje.");
                    }
                    else
                    {
                        reader.Close();
                        query = String.Format("insert into credentials_groups(name, user_id) values('{0}', {1});", newGroup, user_id);
                        MySqlCommand cmd2 = new MySqlCommand(query, conn);

                        cmd2.ExecuteNonQuery();


                        query = string.Format("select * from credentials_groups where name = '{0}'", newGroup);
                        MySqlCommand cmd3 = new MySqlCommand(query, conn);


                        MySqlDataReader reader2 = cmd3.ExecuteReader();

                        if (reader2.Read())
                        {
                            int id = Convert.ToInt32(reader2["id"]);
                            reader2.Close();
                            for (int i = 0; i < ids.Length; i++)
                            {
                                query = String.Format("UPDATE credentials SET group_id = '{0}' WHERE id = {1} AND user_id = {2}", id, ids[i], user_id);
                                MySqlCommand cmd4 = new MySqlCommand(query, conn);

                                cmd4.ExecuteNonQuery();
                            }
                            this.Close();
                        }



                    }
                }

                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }



                finally
                {
                    conn.Close();
                }
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
