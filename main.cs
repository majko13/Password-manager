using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;



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
            string user = textBox1.Text;
            string pass = textBox2.Text;

            try
            {
                conn.Open();
                string query = String.Format("SELECT username FROM users WHERE username = '{0}'", user);
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string username = reader["username"].ToString();
                    reader.Close();
                    query = String.Format("SELECT id, password FROM users WHERE username = '{0}'", user);


                    MySqlCommand cmd2 = new MySqlCommand(query, conn);
                    reader = cmd2.ExecuteReader();

                    if (reader.Read())
                    {
                        string hash = reader["password"].ToString();



                        if (BCrypt.Net.BCrypt.EnhancedVerify(pass, hash))
                        {
                            string id = reader["id"].ToString();

                            reader.Close();




                            Form credentials = new credentials();
                            credentials.Show();

                            this.Hide();
                            
                        }

                        else
                        {
                            reader.Close();
                            throw new Exception("Zadli jste špatné heslo.");
                        }
                    }
                }

                else
                {
                    throw new Exception("Nenašel se učet se stejným jménem.");
                }
            }

            catch (MySqlException ex)
            {
                MessageBox.Show("login error " + ex.Message);
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

        private void button2_Click(object sender, EventArgs e)
        {
            string user = textBox3.Text;
            string pass = textBox4.Text;
            string passRe = textBox5.Text;

            try
            {
                conn.Open();
                string query = String.Format("SELECT username FROM users WHERE username = '{0}'", user);
                MySqlCommand cmd = new MySqlCommand(query, conn);


                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    reader.Close();
                    throw new Exception("Tento uživatel už je zaregistrovaný.");
                }
                else if (pass == null || passRe == null)
                {
                    reader.Close();
                    throw new Exception("Zadali jste pouze jedno heslo.");
                }
                else if (pass != passRe)
                {
                    reader.Close();
                    throw new Exception("Nezadali jste stejné hesla.");
                }
                else
                {
                    reader.Close();
                    string hashedPass = BCrypt.Net.BCrypt.EnhancedHashPassword(pass, 13);
                    query = String.Format("insert into users(username, password, role_id) values('{0}', '{1}', 2 );", user, hashedPass);
                    MySqlCommand cmd2 = new MySqlCommand(query, conn);

                    cmd2.ExecuteNonQuery();
                    MessageBox.Show("Úspěšně si se zaregistroval, můžeš pokračovat na login.");
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
            string nubersPattern = @"\d+";
            string uppercasePattern = @"[A-Z]+";

            Regex specialChars = new Regex(specialCharsPattern);
            Regex nubers = new Regex(nubersPattern);
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

            if (nubers.IsMatch(password))
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

            if (label6.ForeColor == System.Drawing.Color.Green && label7.ForeColor == System.Drawing.Color.Green && label8.ForeColor == System.Drawing.Color.Green && label9.ForeColor == System.Drawing.Color.Green)
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
