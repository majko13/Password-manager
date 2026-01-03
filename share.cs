using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Password_manager
{
    public partial class share : Form
    {
        private MySqlConnection conn;
        private string connectionString;
        private int user_id;

        private int[] group_ids_array;
        private int comboBoxIndex;
        private bool initialLoad = true;

        private bool mouseDown;
        private Point lastLocation;














        private void comboBox_users_Load()
        {
            comboBox2.DataSource = null;
            try
            {
                conn.Close();
                conn.Open();
                comboBoxIndex = comboBox1.SelectedIndex;


                List<Item_2> items = new List<Item_2>();
                string query = String.Format("SELECT (SELECT count(*)  FROM users)as pocet");
                MySqlCommand cmd1 = new MySqlCommand(query, conn);



                MySqlDataReader reader1 = cmd1.ExecuteReader();
                reader1.Read();
                int count = Convert.ToInt32(reader1["pocet"]);
                reader1.Close();

                int[] user = new int[count];


                query = String.Format("SELECT id FROM users");
                MySqlCommand cmd5 = new MySqlCommand(query, conn);



                MySqlDataReader reader5 = cmd5.ExecuteReader();

                for (int i = 0; reader5.Read(); i++)
                {

                    user[i] = Convert.ToInt32(reader5["id"]);

                }
                reader5.Close();


                for (int i = 0; i < user.Length; i++)
                {
                    query = String.Format("SELECT * FROM shared_groups WHERE user_id = {0} AND group_id = {1}", user[i], group_ids_array[comboBoxIndex]);
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {

                    }
                    else
                    {
                        reader.Close();
                        query = String.Format("SELECT * FROM users WHERE id = {0}", user[i]);
                        MySqlCommand cmd2 = new MySqlCommand(query, conn);

                        MySqlDataReader reader2 = cmd2.ExecuteReader();


                        if (reader2.Read() && Convert.ToInt32(reader2["id"]) != user_id)
                        {


                            items.Add(new Item_2(Convert.ToInt32(reader2["id"]), reader2["username"].ToString()));

                        }
                        reader2.Close();
                    }
                    reader.Close();


                }



                comboBox2.DataSource = items;

                if (comboBox2.Items.Count == 0)
                {
                    button1.Enabled = false;
                    comboBox2.Enabled = false;
                }
                else
                {
                    comboBox2.Enabled = true;
                    button1.Enabled = true;
                    comboBox2.SelectedIndex = 0;
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

        private void comboBox_groups_Load()
        {

            try
            {

                conn.Open();
                List<Item> items = new List<Item>();

                string query = String.Format("SELECT * FROM credentials_groups WHERE user_id= {0}", user_id);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(new Item(Convert.ToInt32(reader["id"]), reader["name"].ToString(), Convert.ToInt32(reader["user_id"])));
                }
                comboBox1.DataSource = items;
                group_ids_array = new int[comboBox1.Items.Count];


                reader.Close();


                query = String.Format("SELECT * FROM credentials_groups WHERE user_id= {0}", user_id);
                MySqlCommand cmd1 = new MySqlCommand(query, conn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();

                for (int i = 0; reader1.Read(); i++)
                {
                    group_ids_array[i] = Convert.ToInt32(reader1["id"]);

                }




                comboBox1.DisplayMember = "Name";



                reader1.Close();



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

        public share(int user_id)
        {
            InitializeComponent();



            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            this.user_id = user_id;

            pictureBox1.SendToBack();

            pictureBox1.Image = Properties.Resources.cross_square_svgrepo_com__3_;


            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(325, -2);
            pictureBox1.Size = new System.Drawing.Size(35, 35);

            comboBox_groups_Load();
            comboBox_users_Load();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Item_2 selectedItem = comboBox2.SelectedItem as Item_2;
            try
            {
                conn.Open();


                string query = String.Format("insert into shared_groups (user_id, group_id) values ({0}, {1})", Convert.ToInt32(selectedItem.Id), group_ids_array[comboBoxIndex]);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initialLoad)
            {
                comboBoxIndex = comboBox1.SelectedIndex;
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
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

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
        public class Item_2
        {
            public string Name { get; set; }
            public int Id { get; set; }

            public override string ToString()
            {
                return Name;
            }

            public Item_2(int id, string name)
            {
                Name = name;
                Id = id;
            }
        }

    }
}
