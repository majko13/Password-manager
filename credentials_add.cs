using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Security;

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

            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);
            user_id = id;

            pictureBox1.SendToBack();

            pictureBox1.Image = Properties.Resources.Blue;


            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(327, -2);
            pictureBox1.Size = new System.Drawing.Size(35, 35);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text;
            string url = textBox3.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                Form messagebox = new MyMessageBox("Vyplňte uživatelské jméno.", "Chyba", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                Form messagebox = new MyMessageBox("Vyplňte heslo.", "Chyba", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
                textBox2.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                Form messagebox = new MyMessageBox("Vyplňte URL.", "Chyba", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
                textBox3.Focus();
                return;
            }

            string masterPassword = null;
            byte[] userSalt = null;

            try
            {
                masterPassword = SecurePasswordManager.GetMasterPasswordAsString();
                userSalt = SecurePasswordManager.UserSalt;

                if (string.IsNullOrEmpty(masterPassword) || userSalt == null)
                {
                    Form messagebox = new MyMessageBox("Chyba: Neplatné přihlašovací údaje", "Chyba", MessageBoxIcon.Error);
                    messagebox.ShowDialog();
                    return;
                }

                byte[] key = SecureEncryptor.DeriveKeyFromPassword(masterPassword, userSalt);
                byte[] iv = SecureEncryptor.GenerateRandomIV();
                byte[] encryptedBytes = SecureEncryptor.Encrypt(password, key, iv);

                conn.Open();

                string insertQuery = @"INSERT INTO credentials(username, password, url, user_id, iv) 
                           VALUES(@username, @password, @url, @user_id, @iv)";

                using (MySqlCommand command = new MySqlCommand(insertQuery, conn))
                {
                    command.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;
                    command.Parameters.Add("@password", MySqlDbType.VarBinary).Value = encryptedBytes;
                    command.Parameters.Add("@url", MySqlDbType.VarChar).Value = url;
                    command.Parameters.Add("@user_id", MySqlDbType.Int32).Value = user_id;
                    command.Parameters.Add("@iv", MySqlDbType.VarBinary, 16).Value = iv;

                    command.ExecuteNonQuery();

                    Form messagebox = new MyMessageBox("Údaje uloženy.", "Úspěch", MessageBoxIcon.Information);
                    messagebox.ShowDialog();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Chyba při ukládání do databáze", "Chyba", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            catch (Exception ex)
            {
                Form messagebox = new MyMessageBox("Došlo k neočekávané chybě", "Chyba", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();

                if (masterPassword != null)
                {
                    SecurePasswordManager.ClearString(ref masterPassword);
                }
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