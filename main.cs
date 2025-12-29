using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using BCrypt.Net;

namespace Password_manager
{
    public partial class main : Form
    {
        private MySqlConnection conn;
        private string connectionString;

        public main()
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);

            this.Height = 600;
            this.Width = 470;

            pictureBox1.Image = Properties.Resources.cross_square_svgrepo_com__3_;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Size = new System.Drawing.Size(35, 35);
            pictureBox1.Location = new System.Drawing.Point(445, 10);

            pictureBox2.Image = Properties.Resources.cross_square_svgrepo_com__3_;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Location = new System.Drawing.Point(445, 10);
            pictureBox2.Size = new System.Drawing.Size(35, 35);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = textBox1.Text.Trim();
            string pass = textBox2.Text;

            try
            {
                conn.Open();

                // BEZPEČNÉ: Použij parametry proti SQL Injection
                string query = "SELECT id, password FROM users WHERE username = @username";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", user);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string hash = reader["password"].ToString();
                        int userId = reader.GetInt32("id");
                        reader.Close();

                        if (BCrypt.Net.BCrypt.EnhancedVerify(pass, hash))
                        {
                            // Ulož ID uživatele pro další použití
                            // (můžeš použít Properties.Settings.Default.UserId = userId)

                            Form credentials = new credentials();
                            credentials.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Zadali jste špatné heslo.", "Chyba",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nenašel se účet se stejným jménem.", "Chyba",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Chyba databáze: " + ex.Message, "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba: " + ex.Message, "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // Validace
                if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    MessageBox.Show("Vyplňte všechna pole!", "Upozornění",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (pass != passRe)
                {
                    MessageBox.Show("Hesla se neshodují!", "Chyba",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                conn.Open();

                // 1. BEZPEČNĚ zkontroluj, zda uživatel již existuje
                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@username", user);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    MessageBox.Show("Tento uživatel už je zaregistrovaný.", "Chyba",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. BEZPEČNĚ vlož nového uživatele
                string hashedPass = BCrypt.Net.BCrypt.EnhancedHashPassword(pass, 13);
                string insertQuery = "INSERT INTO users (username, password, role_id) VALUES (@username, @password, 2)";

                MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@username", user);
                insertCmd.Parameters.AddWithValue("@password", hashedPass);

                int rowsAffected = insertCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Úspěšně si se zaregistroval, můžeš pokračovat na login.", "Úspěch",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Přepni zpět na login a vyčisti pole
                    button4_Click(sender, e);
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Chyba databáze: " + ex.Message, "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba: " + ex.Message, "Chyba",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            this.Height = 700;
            this.Width = 470;
            groupBox2.Location = new Point(-10, -12);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
            groupBox1.Visible = true;
            this.Height = 600;
            this.Width = 470;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
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
                label9.ForeColor == System.Drawing.Color.Green)
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }
    }
}