using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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
                conn.Open();

                List<Item> items = new List<Item>();


                string query = String.Format("select  credentials_groups.id , credentials_groups.name ,credentials_groups.user_id from shared_groups left join credentials_groups on shared_groups.group_id =credentials_groups.id where shared_groups.user_id ={0}", user_id);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();
                items.Add(new Item(-1, "all", -1));

                for (int i = 0; reader.Read(); i++)
                {
                    items.Add(new Item(Convert.ToInt32(reader["id"]), reader["name"].ToString(), Convert.ToInt32(reader["user_id"])));
                }

                comboBox1.DataSource = items;
                comboBox1.DisplayMember = "Name";
                comboBox1.SelectedIndex = 0;

                reader.Close();



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
                conn.Close();
                string query;
                conn.Open();
                if (selectedItem.Id == -1 && selectedItem.User_Id == -1)
                {
                    query = String.Format("SELECT credentials.id, users.username, credentials.username AS credentials_username, credentials.password, credentials.url, credentials_groups.name FROM shared_groups LEFT JOIN credentials ON shared_groups.group_id = credentials.group_id LEFT JOIN credentials_groups ON shared_groups.group_id = credentials_groups.id LEFT JOIN users ON credentials_groups.user_id = users.id WHERE shared_groups.user_id = {0}", user_id);

                }
                else
                {


                    query = String.Format("SELECT credentials.id, users.username, credentials.username AS credentials_username, credentials.password, credentials.url, credentials_groups.name FROM shared_groups LEFT JOIN credentials ON shared_groups.group_id = credentials.group_id LEFT JOIN credentials_groups ON shared_groups.group_id = credentials_groups.id LEFT JOIN users ON credentials_groups.user_id = users.id WHERE shared_groups.user_id = {0} AND credentials_groups.id = {1} AND credentials_groups.user_id = {2}", user_id, selectedItem.Id, selectedItem.User_Id);
                }



                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Encryptor encryptor = new Encryptor();


                while (reader.Read())
                {

                    byte[] bytes = (byte[])reader["password"];


                    string password = encryptor.Decrypt(bytes);


                    dataGridView1.Rows.Add(reader["id"], reader["username"], reader["credentials_username"], password, reader["url"], reader["name"]);
                }


                reader.Close();
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
        public shared(int user_id)
        {
            InitializeComponent();


            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            this.user_id = user_id;

            pictureBox1.SendToBack();

            pictureBox1.Image = Properties.Resources.cross_square_svgrepo_com__3_;


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
