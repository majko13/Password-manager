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
                    return "***NENÍ PŘIHLÁŠEN***";
                }

                if (iv == null || iv.Length == 0)
                {
                    return "***NEVALIDNÍ IV - STARÝ ZÁZNAM***";
                }

                byte[] key = SecureEncryptor.DeriveKeyFromPassword(masterPassword, userSalt);

                return SecureEncryptor.Decrypt(encryptedBytes, key, iv);
            }
            catch (CryptographicException)
            {
                return "***ŠPATNÝ KLÍČ***";

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
                conn.Open();

                List<Item> items = new List<Item>();


                string query = "SELECT * FROM credentials_groups WHERE user_id = @user_id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user_id", user_id);

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
                Form messagebox = new MyMessageBox("Chyba při načítání skupin", "Chyba", MessageBoxIcon.Error);
                messagebox.ShowDialog();
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
                conn.Open();

                string query;
                MySqlCommand cmd;
                if (selectedItem.Id == -1 && selectedItem.User_Id == -1)
                {
                    query = "SELECT * FROM credentials LEFT JOIN credentials_groups " +
                         "ON credentials.group_id = credentials_groups.id " +
                         "WHERE credentials.user_id = @user_id";
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                }
                else if (selectedItem.Id == 0 && selectedItem.User_Id == 0)
                {

                    query = "SELECT * FROM credentials LEFT JOIN credentials_groups " +
                         "ON credentials.group_id = credentials_groups.id " +
                         "WHERE credentials.user_id = @user_id AND group_id IS NULL";
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                }
                else
                {
                    query = "SELECT * FROM credentials LEFT JOIN credentials_groups " +
                             "ON credentials.group_id = credentials_groups.id " +
                             "WHERE credentials.user_id = @user_id AND group_id = @group_id " +
                             "AND credentials_groups.user_id = @group_user_id";
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@group_id", selectedItem.Id);
                    cmd.Parameters.AddWithValue("@group_user_id", selectedItem.User_Id);
                }


                MySqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    byte[] encryptedBytes = (byte[])reader["password"];

                    byte[] iv = (byte[])reader["iv"];

                    string password = DecryptPasswordWithMasterKey(encryptedBytes, iv);



                    dataGridView1.Rows.Add(reader["id"], reader["username"], password, reader["url"], reader["name"]);

                }
                reader.Close();

                string roleQuery = "SELECT * FROM users WHERE role_id = 1 AND id = @user_id";
                MySqlCommand roleCmd = new MySqlCommand(roleQuery, conn);
                roleCmd.Parameters.AddWithValue("@user_id", user_id);
                MySqlDataReader reader1 = roleCmd.ExecuteReader();


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
                Form messagebox = new MyMessageBox("Chyba při načítání dat", "Chyba", MessageBoxIcon.Error);
                messagebox.ShowDialog();
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

            masterPassword = SecurePasswordManager.GetMasterPasswordAsString();
            userSalt = SecurePasswordManager.UserSalt;

            if (string.IsNullOrEmpty(masterPassword) || userSalt == null)
            {
                MessageBox.Show("Chyba: Neplatné přihlašovací údaje", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            comboBox_load();
            load();

            label3.Text = "Acount: " + username;
            pictureBox1.SendToBack();

            pictureBox1.Image = Properties.Resources.Blue;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(992, 0);
            pictureBox1.Size = new System.Drawing.Size(35, 35);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Opravdu chcete zavřít aplikaci?", "Zavřít",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }

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
                            Form messagebox = new MyMessageBox("Nejste přihlášeni!", "Chyba", MessageBoxIcon.Error);
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
                Form messagebox = new MyMessageBox("Chyba při ukládání dat", "Chyba", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            catch (Exception ex)
            {
                Form messagebox = new MyMessageBox("Chyba při ukládání dat", "Chyba", MessageBoxIcon.Error);
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


            if (DialogResult.Yes == MessageBox.Show("Opravdu se chcete odhlásit?", "Odhlášení", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
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
            if (DialogResult.Yes != MessageBox.Show("Opravdu chcete smazat vybrané záznamy?",
                "Smazat záznamy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
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
                    Form messagebox = new MyMessageBox("Chyba při mazání záznamu", "Chyba", MessageBoxIcon.Error);
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
                    Form messagebox = new MyMessageBox("Chyba při načítání skupin", "Chyba", MessageBoxIcon.Error);
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
                    share.Show();
                }
            }
            else
            {
                Form messagebox = new MyMessageBox("You have not any groups to share.", "Chyba", MessageBoxIcon.Error);
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
                MessageBox.Show("Prosím, vyberte skupinu na vymazanie.");
                return;
            }

            if (comboBox1.SelectedIndex <= 1)
            {
                MessageBox.Show(
                    "Nemôžete vymazať túto položku.\n\n" +
                    "Vyberte konkrétnu skupinu na vymazanie.",
                    "Neplatná voľba",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Item selectedGroup = comboBox1.SelectedItem as Item;

            if (selectedGroup == null)
            {
                MessageBox.Show("Chyba pri výbere skupiny.");
                return;
            }


            DialogResult result = MessageBox.Show(
                $"Naozaj chcete vymazať skupinu '{selectedGroup.Name}'?\n\n" +
                $"Heslá sa automaticky presunú do 'Bez skupiny' (group_id = NULL).\n\n" +
                "Túto akciu nie je možné vrátiť späť!",
                "Vymazanie skupiny",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

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


                    MessageBox.Show(
                        $"Skupina '{selectedGroup.Name}' bola úspešne vymazaná.\n",
                        "Vymazanie úspešné",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    conn.Close();
                    comboBox_load();
                    if (comboBox1.Items.Count > 1)
                    {
                        comboBox1.SelectedIndex = 1;
                    }
                }
                else
                {
                    MessageBox.Show("Skupinu sa nepodarilo vymazať.");
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
