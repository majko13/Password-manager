using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Security.Cryptography;


namespace Password_manager
{
    public partial class new_password : Form
    {
        private MySqlConnection conn;
        private string connectionString;

        private bool mouseDown;
        private Point lastLocation;

        private int userId;
        private string currentUsername;  
        private int currentRoleId;
        public new_password(int userId,string username,int roleId)
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);

            this.userId = userId;
            this.currentUsername = username;  
            this.currentRoleId = roleId;


            textBox3.Text = currentUsername;
            if(roleId == 1)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                comboBox1.SelectedIndex = 1;
            }



            pictureBox1.SendToBack();
            pictureBox1.Image = Properties.Resources.Blue;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Size = new System.Drawing.Size(35, 35);
            pictureBox1.Location = new System.Drawing.Point(370, 2);

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string newUsername = textBox3.Text.Trim();
            string newPassword = textBox1.Text.Trim();
            string repeatPassword = textBox2.Text.Trim();
            int newRoleId = comboBox1.SelectedIndex == 0 ? 1 : 2;

            if (string.IsNullOrWhiteSpace(newUsername))
            {
                Form messagebox = new MyMessageBox("Username cannot be empty.", "Error", MessageBoxIcon.Warning);
                messagebox.ShowDialog();
                return;
            }

            if (!string.IsNullOrEmpty(newPassword) || !string.IsNullOrEmpty(repeatPassword))
            {
                if (newPassword != repeatPassword)
                {
                    Form messagebox = new MyMessageBox("Passwords do not match.", "Error", MessageBoxIcon.Warning);
                    messagebox.ShowDialog();
                    return;
                }

                if (newPassword.Length < 6)
                {
                    Form messagebox = new MyMessageBox("Password must be at least 6 characters long.", "Error", MessageBoxIcon.Warning);
                    messagebox.ShowDialog();
                    return;
                }
            }

            try
            {
                conn.Open();

                bool hasChanges = false;
                List<string> changes = new List<string>();

                if (newUsername != currentUsername) 
                {
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username AND id != @userId";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", newUsername);
                        checkCmd.Parameters.AddWithValue("@userId", userId);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            Form messagebox = new MyMessageBox("Username already exists.", "Error", MessageBoxIcon.Warning);
                            messagebox.ShowDialog();
                            return;
                        }
                    }
                    changes.Add("username");
                    hasChanges = true;
                }

                if (newRoleId != currentRoleId) 
                {
                    changes.Add("role");
                    hasChanges = true;
                }

                bool passwordChanged = !string.IsNullOrEmpty(newPassword);

                if (passwordChanged)
                {
                    DialogResult result = new MyMessageBox(
                        "PASSWORD CHANGE:\n\n" +
                        "After changing your password, you **WILL NOT BE ABLE TO USE**\n" +
                        "your old saved passwords in the application!\n\n" +
                        "You will have to:\n" +
                        "• Add all passwords again\n" +
                        "Do you really want to continue?",
                        "WARNING",
                        MessageBoxIcon.Warning,
                        MessageBoxButtons.YesNo).ShowDialog();

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }

                    changes.Add("password");
                    hasChanges = true;
                }

                string updateQuery = "UPDATE users SET ";
                List<string> setClauses = new List<string>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                if (newUsername != currentUsername)
                {
                    setClauses.Add("username = @username");
                    parameters.Add(new MySqlParameter("@username", newUsername));
                }

                if (passwordChanged)
                {
                    setClauses.Add("password = @password");
                    parameters.Add(new MySqlParameter("@password", HashPassword(newPassword)));
                }

                if (newRoleId != currentRoleId)
                {
                    setClauses.Add("role_id = @roleId");
                    parameters.Add(new MySqlParameter("@roleId", newRoleId));
                }

                updateQuery += string.Join(", ", setClauses);
                updateQuery += " WHERE id = @userId";
                parameters.Add(new MySqlParameter("@userId", userId));

                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddRange(parameters.ToArray());
                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        string changesText = changes.Count > 0 ? string.Join(", ", changes) : "";
                        string message = "Changes were successfully saved.";
                        if (!string.IsNullOrEmpty(changesText))
                            message += $"\nChanged: {changesText}";
                        if (passwordChanged)
                            message += $"\n{(changesText.Length > 0 ? "and password changed" : "Password changed")}";

                        Form messagebox = new MyMessageBox(message, "Success", MessageBoxIcon.Information);
                        messagebox.ShowDialog();
                        this.Close();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Form messagebox = new MyMessageBox("Database error: " + ex.Message, "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }
        private void new_password_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void new_password_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void new_password_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, 15);
        }
    }
}
