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
    public partial class credentials : Form
    {

        private MySqlConnection conn;
        private string connectionString;
        private int user_id;


        private void load()
        {
            MessageBox.Show("kokot");
            dataGridView1.Rows.Clear();
            try
            {
                //Item selectedItem = comboBox1.SelectedItem as Item;
                conn.Close();
                string query;
                conn.Open();
                //if (selectedItem.Id == -1 && selectedItem.User_Id == -1)
                //{
                //    query = String.Format("SELECT * FROM credentials LEFT JOIN credentials_groups ON credentials.group_id = credentials_groups.id WHERE credentials.user_id = '{0}'", user_id);

                //}
                //else if (selectedItem.Id == 0 && selectedItem.User_Id == 0)
                //{

                //    query = String.Format("SELECT * FROM credentials LEFT JOIN credentials_groups ON credentials.group_id = credentials_groups.id WHERE credentials.user_id = '{0}' AND group_id is null", user_id);

                //}
                //else
                //{

                //    query = String.Format("SELECT * FROM credentials LEFT JOIN credentials_groups ON credentials.group_id = credentials_groups.id WHERE credentials.user_id = '{0}' AND group_id = {1} AND credentials_groups.user_id = {2}", user_id, selectedItem.Id, selectedItem.User_Id);
                //}


                query = String.Format("SELECT * FROM credentials WHERE user_id = '{0}'", user_id);  

                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Encryptor encryptor = new Encryptor();
                MessageBox.Show("kokot");
                while (reader.Read())
                {
                    byte[] bytes = (byte[])reader["password"];


                    string password = encryptor.Decrypt(bytes);


                    dataGridView1.Rows.Add(reader["id"], reader["username"], password, reader["url"], reader["group_id"]);

                }
                reader.Close();

                query = String.Format("select * FROM users where role_id =1 AND id = {0}", user_id);
                MySqlCommand cmd1 = new MySqlCommand(query, conn);
                MySqlDataReader reader1 = cmd1.ExecuteReader();


                if (reader1.Read())
                {
                    button6.Visible = true;
                }
                else
                {
                    button6.Visible = false;
                }
                reader1.Close();

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("credenials load error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

        }




        public credentials(int id, string username)
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            user_id = id;
            load();







            label3.Text = "Acount: "+username;
            pictureBox1.SendToBack();

            pictureBox1.Image = Properties.Resources.cross_square_svgrepo_com__3_;


            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(992, 0);
            pictureBox1.Size = new System.Drawing.Size(35, 35);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form credentials_add = new credentials_add(user_id);
            credentials_add.Show();
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
    }

}
