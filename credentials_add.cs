using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Password_manager
{
    public partial class credentials_add : Form
    {
        private MySqlConnection conn;
        private string connectionString;
        private int user_id;
        private bool mouseDown;
        private Point lastLocation;

        public credentials_add(int id)
        {
            InitializeComponent();

            // Lepšie: použite App.config
            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            user_id = id;

            // Načítanie obrázku
            string path = Path.Combine(Environment.CurrentDirectory, @"cross-square-svgrepo-com (3).png");
            if (File.Exists(path))
            {
                pictureBox1.Image = new Bitmap(path);
            }

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(435, 0);
            pictureBox1.Size = new System.Drawing.Size(35, 35);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text;
            string url = textBox3.Text.Trim();

            // VALIDÁCIA
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vyplňte uživatelské jméno.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vyplňte heslo.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Vyplňte URL.", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            try
            {
                // ŠIFROVANIE
                Encryptor encryptor = new Encryptor();
                byte[] encryptedBytes = encryptor.Encrypt(password);

                conn.Open();

                string insertQuery = @"INSERT INTO credentials(username, password, url, user_id) 
                               VALUES(@username, @password, @url, @user_id)";

                using (MySqlCommand command = new MySqlCommand(insertQuery, conn))
                {
                    command.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
                    command.Parameters.Add("@password", MySqlDbType.VarBinary).Value = encryptedBytes;
                    command.Parameters.Add("@url", MySqlDbType.VarChar).Value = url;
                    command.Parameters.Add("@user_id", MySqlDbType.Int32).Value = user_id;

                    command.ExecuteNonQuery();

                    MessageBox.Show("Údaje uloženy.", "Úspěch",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Chyba databáze: {ex.Message}", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba: {ex.Message}", "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void credentials_add_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void credentials_add_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && e.Button == MouseButtons.Left)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X,
                    (this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();
            }
        }

        private void credentials_add_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}