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
            this.AcceptButton = button1;

            pictureBox1.Image = Properties.Resources.Blue;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Size = new System.Drawing.Size(35, 35);
            pictureBox1.Location = new System.Drawing.Point(445, 10);

            pictureBox2.Image = Properties.Resources.Blue;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Location = new System.Drawing.Point(445, 10);
            pictureBox2.Size = new System.Drawing.Size(35, 35);

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
            string user = textBox1.Text.Trim();
            string pass = textBox2.Text;

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
                            textBox1.Text = "";
                            textBox2.Text = "";

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
            string user = textBox3.Text.Trim();
            string pass = textBox4.Text;
            string passRe = textBox5.Text;

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
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
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
            groupBox1.Visible = false;
            groupBox2.Visible = true;
            this.Height = 800;
            this.Width = 470;
            this.CenterToScreen();
            this.AcceptButton = button2;
            groupBox2.Location = new Point(-10, -12);

            button5.BackColor = Color.Red;
            button5.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
            groupBox1.Visible = true;
            this.AcceptButton = button1;
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
            string password = textBox4.Text;

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
                button5.BackColor == System.Drawing.Color.Green)
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form form = new CAPTCHA();

            if (form.ShowDialog() == DialogResult.OK)
            {
                button5.BackColor = Color.Green;
                button5.Enabled = false;

                if (label6.ForeColor == System.Drawing.Color.Green &&
                label7.ForeColor == System.Drawing.Color.Green &&
                label8.ForeColor == System.Drawing.Color.Green &&
                label9.ForeColor == System.Drawing.Color.Green &&
                button5.BackColor == System.Drawing.Color.Green)
                {
                    button2.Enabled = true;
                }
                else
                {
                    button2.Enabled = false;
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
    }
}