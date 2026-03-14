using System;
using System.Configuration;
using System.Drawing;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using MySql.Data.MySqlClient;

namespace Password_manager
{
    public partial class main : Form
    {
        private MySqlConnection conn;
        private string connectionString;

        private bool mouseDown;
        private Point lastLocation;


        private byte[] GenerateRandomSalt(int size = 32)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }


        public main()
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);

            this.Height = 600;
            this.Width = 470;
            this.AcceptButton = loginButton;

            LoginPicturebox.Image = Properties.Resources.Blue;
            LoginPicturebox.SizeMode = PictureBoxSizeMode.Zoom;
            LoginPicturebox.Size = new System.Drawing.Size(35, 35);
            LoginPicturebox.Location = new System.Drawing.Point(441, 11);

            RegisterPicturebox.Image = Properties.Resources.Blue;
            RegisterPicturebox.SizeMode = PictureBoxSizeMode.Zoom;
            RegisterPicturebox.Location = new System.Drawing.Point(444, 11);
            RegisterPicturebox.Size = new System.Drawing.Size(35, 35);


            AddMouseEventsToAllControls(this);
        }
        private void AddMouseEventsToAllControls(Control parent)
        {
            if (parent is Button ||
                parent is PictureBox ||
                parent is DataGridView)
                return;

            parent.MouseDown += main_MouseDown;
            parent.MouseMove += main_MouseMove;
            parent.MouseUp += main_MouseUp;

            foreach (Control ctrl in parent.Controls)
            {
                AddMouseEventsToAllControls(ctrl);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string user = usernameLoginTextbox.Text.Trim();
            string pass = passwordLoginTextbox.Text;

            try
            {
                conn.Open();

                string query = "SELECT id, password, user_salt FROM users WHERE username = @username";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", user);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string hash = reader["password"].ToString();
                        string id = reader["id"].ToString();
                        byte[] userSalt = (byte[])reader["user_salt"];
                        reader.Close();

                        if (BCrypt.Net.BCrypt.EnhancedVerify(pass, hash))
                        {
                            SecurePasswordManager.SetCredentials(pass, userSalt, Convert.ToInt32(id));
                            Form credentials = new credentials(Convert.ToInt32(id), user);
                            this.Hide();
                            credentials.ShowDialog();



                            this.Show();
                            usernameLoginTextbox.Text = "";
                            usernameLoginTextbox.Focus();
                            passwordLoginTextbox.Text = "";

                            SecurePasswordManager.ClearCredentials();
                        }
                        else
                        {
                            Form messagebox = new MyMessageBox("Wrong password or username", "Warning", MessageBoxIcon.Warning);
                            messagebox.ShowDialog();
                        }
                    }
                    else
                    {
                        Form messagebox = new MyMessageBox("Wrong password or username", "Warning", MessageBoxIcon.Warning);
                        messagebox.ShowDialog();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Database error:\n" + ex.Message, "Database Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            catch (Exception ex)
            {
                Form messagebox = new MyMessageBox("Error:\n" + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string user = usernameRegisterTextbox.Text.Trim();
            string pass = passwordRegisterTextbox.Text;
            string passRe = rPasswordRegisterTextbox.Text;

            try
            {
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    Form messagebox = new MyMessageBox("Please fill in all fields!", "Warning", MessageBoxIcon.Warning);
                    messagebox.ShowDialog();
                    return;
                }

                if (pass != passRe)
                {
                    Form messagebox = new MyMessageBox("Passwords do not match!", "Warning", MessageBoxIcon.Warning);
                    messagebox.ShowDialog();
                    return;
                }


                conn.Open();



                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@username", user);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    Form messagebox = new MyMessageBox("This user is already registered.", "Warning", MessageBoxIcon.Warning);
                    messagebox.ShowDialog();
                    return;
                }





                byte[] userSalt = GenerateRandomSalt();
                string hashedPass = BCrypt.Net.BCrypt.EnhancedHashPassword(pass, 15);






                string insertQuery = "INSERT INTO users (username, password, role_id, user_salt) " +
                                                "VALUES (@username, @password, 2, @user_salt)";

                MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@username", user);
                insertCmd.Parameters.AddWithValue("@password", hashedPass);
                insertCmd.Parameters.AddWithValue("@user_salt", userSalt);

                int rowsAffected = insertCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Form messagebox = new MyMessageBox("You have successfully registered,\n" +
                                                    "you can now proceed to login.", "Success", MessageBoxIcon.Information);
                    messagebox.ShowDialog();

                    button4_Click(sender, e);
                    usernameRegisterTextbox.Clear();
                    passwordRegisterTextbox.Clear();
                    rPasswordRegisterTextbox.Clear();
                }


            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Database error:\n" + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            catch (Exception ex)
            {
                Form messagebox = new MyMessageBox("Error:\n" + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            loginGroupbox.Visible = false;
            registerGroupbox.Visible = true;
            this.Height = 780;
            this.Width = 470;
            this.CenterToScreen();
            this.AcceptButton = registerButton;
            registerGroupbox.Location = new Point(-10, -12);

            CaptchaButton.BackColor = Color.Red;
            CaptchaButton.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            registerGroupbox.Visible = false;
            loginGroupbox.Visible = true;
            this.AcceptButton = loginButton;
            this.CenterToScreen();
            this.Height = 600;
            this.Width = 470;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            if (new MyMessageBox("Do you really want to close the application?", "Close", MessageBoxIcon.Question, MessageBoxButtons.YesNo).ShowDialog() == DialogResult.Yes)
            {
                SecurePasswordManager.ClearCredentials();
                Application.Exit();
            }

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (new MyMessageBox("Do you really want to close the application?", "Close", MessageBoxIcon.Question, MessageBoxButtons.YesNo).ShowDialog() == DialogResult.Yes)
            {
                SecurePasswordManager.ClearCredentials();
                Application.Exit();
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            string password = passwordRegisterTextbox.Text;

            string specialCharsPattern = @"[^a-zA-Z0-9\s]";
            string numbersPattern = @"\d+";
            string uppercasePattern = @"[A-Z]+";

            Regex specialChars = new Regex(specialCharsPattern);
            Regex numbers = new Regex(numbersPattern);
            Regex uppercase = new Regex(uppercasePattern);

            if (uppercase.IsMatch(password))
            {
                label9.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                label9.ForeColor = System.Drawing.Color.Red;
            }

            if (specialChars.IsMatch(password))
            {
                label7.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                label7.ForeColor = System.Drawing.Color.Red;
            }

            if (numbers.IsMatch(password))
            {
                label8.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                label8.ForeColor = System.Drawing.Color.Red;
            }

            if (password.Length >= 12)
            {
                label6.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                label6.ForeColor = System.Drawing.Color.Red;
            }

            if (label6.ForeColor == System.Drawing.Color.Green &&
                label7.ForeColor == System.Drawing.Color.Green &&
                label8.ForeColor == System.Drawing.Color.Green &&
                label9.ForeColor == System.Drawing.Color.Green &&
                CaptchaButton.BackColor == System.Drawing.Color.Green)
            {
                registerButton.Enabled = true;
            }
            else
            {
                registerButton.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form form = new CAPTCHA();

            if (form.ShowDialog() == DialogResult.OK)
            {
                CaptchaButton.BackColor = Color.Green;
                CaptchaButton.Enabled = false;

                if (label6.ForeColor == System.Drawing.Color.Green &&
                label7.ForeColor == System.Drawing.Color.Green &&
                label8.ForeColor == System.Drawing.Color.Green &&
                label9.ForeColor == System.Drawing.Color.Green &&
                CaptchaButton.BackColor == System.Drawing.Color.Green)
                {
                    registerButton.Enabled = true;
                }
                else
                {
                    registerButton.Enabled = false;
                }
            }
        }



        private void button6_Click(object sender, EventArgs e)
        {




            new MyMessageBox(
                "TO RECOVER YOUR PASSWORD:\n\n" +
                "1. Contact the admin:\n" +
                "   Email: admin@passwordmanager.com\n" +
                "   Tel: +421 123 456 789\n" +
                "2. In your message include:\n" +
                $"  • Your username\n" +
                "   • Reason for request\n" +
                "3. After verification, the admin will send you a new password,\n" +
                "   which admin has to change manually.",
                "Forgotten Password").ShowDialog();
        }

        private void main_MouseDown(object sender, MouseEventArgs e)
        {

            mouseDown = true;
            lastLocation = Cursor.Position;
        }

        private void main_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point current = Cursor.Position;
                this.Location = new Point(
                    this.Location.X + (current.X - lastLocation.X),
                    this.Location.Y + (current.Y - lastLocation.Y));

                lastLocation = current;
            }
        }

        private void main_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void usernameLoginTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Back || e.Control && e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

            }
        }
    }
}