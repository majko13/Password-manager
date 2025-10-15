using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;



namespace Password_manager
{
    public partial class main : Form
    {

        private MySqlConnection conn;
        private string server;
        private string database;
        private string user;
        private string pass;
        private string connectionString;
        public main()
        {
            InitializeComponent();
            server = "localhost";
            database = "password_manager";
            user = "root";
            pass = "root";
            connectionString = String.Format("server={0};user id={1}; password={2}; database={3}", server, user, pass, database);
            conn = new MySqlConnection(connectionString);
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
    }
}
