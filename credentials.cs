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
    public partial class credentials : Form
    {

        private MySqlConnection conn;
        private string connectionString;
        private int user_id;
        private int group_id;
        private bool mouseDown;
        private Point lastLocation;
        private bool closeButtonClicked = false;

        private void comboBox_load()
        {

            try
            {
                conn.Open();

                List<Item> items = new List<Item>();


                string query = String.Format("SELECT * FROM credentials_groups WHERE user_id = {0}", user_id);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();
                items.Add(new Item(-1, "all", -1));
                items.Add(new Item(0, "without group", 0));
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
                    query = String.Format("SELECT * FROM credentials LEFT JOIN credentials_groups ON credentials.group_id = credentials_groups.id WHERE credentials.user_id = '{0}'", user_id);

                }
                else if (selectedItem.Id == 0 && selectedItem.User_Id == 0)
                {

                    query = String.Format("SELECT * FROM credentials LEFT JOIN credentials_groups ON credentials.group_id = credentials_groups.id WHERE credentials.user_id = '{0}' AND group_id is null", user_id);

                }
                else
                {

                    query = String.Format("SELECT * FROM credentials LEFT JOIN credentials_groups ON credentials.group_id = credentials_groups.id WHERE credentials.user_id = '{0}' AND group_id = {1} AND credentials_groups.user_id = {2}", user_id, selectedItem.Id, selectedItem.User_Id);
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Encryptor encryptor = new Encryptor();
                while (reader.Read())
                {
                    byte[] bytes = (byte[])reader["password"];


                    string password = encryptor.Decrypt(bytes);


                    dataGridView1.Rows.Add(reader["id"], reader["username"], password, reader["url"], reader["name"]);

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
            comboBox_load();
            load();







            label3.Text = "Acount: " + username;
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
            lastLocation = e.Location;
        }

        private void credentials_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
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
                int column = dataGridView1.CurrentCell.ColumnIndex;
                string value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                string str = dataGridView1.Rows[row].Cells[0].Value.ToString();
                string columnName = this.dataGridView1.Columns[e.ColumnIndex].Name;


                conn.Open();

                if (int.TryParse(str, out int id))
                {
                    switch (columnName)
                    {
                        case "Column1": columnName = "username"; break;
                        case "Column2":
                            columnName = "password";
                            {

                                Encryptor encryptor = new Encryptor();

                                byte[] bytes = encryptor.Encrypt(value);



                                string insertQuery = "update credentials set password = @value where id = @id";
                                using (MySqlCommand command = new MySqlCommand(insertQuery, conn))
                                {
                                    command.Parameters.Add("@value", MySqlDbType.VarBinary, -1).Value = bytes;
                                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                                    command.ExecuteNonQuery();
                                }

                                break;
                            }
                        case "Column4": columnName = "url"; break;
                        default: throw new Exception("Vybrali jste špatný sloupec.");
                    }
                    if (columnName != "password")
                    {
                        string query = String.Format("update credentials set {0} ='{1}' where id ={2}", columnName, value, id);
                        MySqlCommand cmd = new MySqlCommand(query, conn);

                        cmd.ExecuteNonQuery();

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

        private void button7_Click(object sender, EventArgs e)
        {


            if (DialogResult.Yes == MessageBox.Show("Opravdu se chcete odhlásit?", "Odhlášení", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                closeButtonClicked = true;
                this.Close();
                Form loginForm = new main();
                loginForm.Show();
            }
        }

        private void credentials_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!closeButtonClicked)
            {
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (DialogResult.Yes == MessageBox.Show("Opravdu chcete smazat vybrané záznamy?", "Smazat záznamy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
            {

                int[] indexs = new int[dataGridView1.SelectedRows.Count];
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                    {
                        indexs[i] = dataGridView1.SelectedRows[i].Index;

                    }

                    Array.Sort(indexs);



                    if (indexs[0] > indexs[indexs.Count() - 1])
                    {
                        Array.Reverse(indexs);
                    }

                    for (int index = dataGridView1.SelectedRows.Count - 1; index >= 0; index--)
                    {




                        int count = 0, group_id = 0;


                        string str = dataGridView1.Rows[indexs[index]].Cells[0].Value.ToString();

                        if (int.TryParse(str, out int id))
                        {

                            try
                            {
                                conn.Open();

                                string query = String.Format("SELECT group_id FROM credentials WHERE id = {0} AND group_id IS NOT NULL", id);
                                MySqlCommand cmd1 = new MySqlCommand(query, conn);
                                MySqlDataReader reader = cmd1.ExecuteReader();

                                if (reader.Read())
                                {
                                    group_id = Convert.ToInt32(reader["group_id"]);
                                    reader.Close();
                                    query = String.Format("SELECT (SELECT COUNT(group_id) FROM credentials WHERE group_id = {0}) AS count", group_id);

                                    MySqlCommand cmd2 = new MySqlCommand(query, conn);
                                    MySqlDataReader reader1 = cmd2.ExecuteReader();

                                    reader1.Read();
                                    count = Convert.ToInt32(reader1["count"]);
                                    reader1.Close();
                                }
                                if (count == 1)
                                {
                                    query = String.Format("DELETE FROM credentials_groups WHERE id = {0}", group_id);
                                    MySqlCommand cmd3 = new MySqlCommand(query, conn);
                                    cmd3.ExecuteNonQuery();
                                    conn.Close();
                                    comboBox_load();
                                    conn.Open();
                                }
                                reader.Close();

                                query = String.Format("DELETE FROM credentials WHERE id={0}", id);
                                MySqlCommand cmd = new MySqlCommand(query, conn);

                                cmd.ExecuteNonQuery();

                                dataGridView1.Rows.RemoveAt(indexs[index]);

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
                    }
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
                credentials_groups.Show();
                credentials_groups.FormClosed += delegate
                {

                    comboBox_load();
                    load();

                };
            }
            else
            {
                MessageBox.Show("Nemáš označený žádný řádek.");
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
                try
                {
                    conn.Open();

                    string query = String.Format("SELECT group_id FROM credentials WHERE user_id = {0}", user_id);
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader1 = cmd.ExecuteReader();

                    reader1.Read();
                    group_id = Convert.ToInt32(reader1["group_id"]);
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




                Form share = new share(user_id);

                share.Show();
                share.FormClosed += delegate
                {

                };

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form shared = new shared(user_id);
            shared.Show();
        }
    }
}
