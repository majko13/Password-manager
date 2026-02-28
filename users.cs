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

namespace Password_manager
{
    public partial class users : Form
    {
        private MySqlConnection conn;
        private string connectionString;

        private bool mouseDown;
        private Point lastLocation;

        private void load()
        {
            try
            {
                conn.Open();

                string query = @"
            SELECT id, username, role_id
            FROM users
            ORDER BY id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                dataGridView1.Rows.Clear();

                while (reader.Read())
                {
                    string role = Convert.ToInt32(reader["role_id"]) == 1 ? "Admin" : "User";

                    dataGridView1.Rows.Add(
                        reader["id"],      // ID - skrytý stĺpec
                        reader["username"], // Používateľské meno
                        role                // Rola ako text
                    );
                }

                reader.Close();

                if (dataGridView1.Columns.Count >= 3)
                {
                    dataGridView1.Columns[0].HeaderText = "ID";
                    dataGridView1.Columns[1].HeaderText = "Používateľské meno";
                    dataGridView1.Columns[2].HeaderText = "Rola";

                    dataGridView1.Columns[0].Visible = false;
                }
            }
            catch (MySqlException ex)
            {
                new MyMessageBox("Chyba pri načítaní používateľov: " + ex.Message, "Chyba", MessageBoxIcon.Error).ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }
        public users()
        {
            InitializeComponent();


            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            conn = new MySqlConnection(connectionString);

            pictureBox1.SendToBack();
            pictureBox1.Image = Properties.Resources.Blue;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Size = new System.Drawing.Size(35, 35);
            pictureBox1.Location = new System.Drawing.Point(370, 2);

            load();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void users_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void users_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void users_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                new MyMessageBox("Nie je vybratý žiadny záznam na vymazanie.",
                    "Žiadny výber", MessageBoxIcon.Warning).ShowDialog();
                return;
            }

            if (DialogResult.Yes != new MyMessageBox(
                "Naozaj chcete vymazať vybrané záznamy?",
                "Potvrdenie vymazania",
                MessageBoxIcon.Question, MessageBoxButtons.YesNo).ShowDialog())
            {
                return;
            }


            try
            {
                conn.Open();

                for (int i = dataGridView1.SelectedRows.Count - 1; i >= 0; i--)
                {
                    DataGridViewRow row = dataGridView1.SelectedRows[i];

                    if (!int.TryParse(row.Cells[0].Value?.ToString(), out int id))
                    {
                        continue;
                    }


                    string deleteQuery = "DELETE FROM users WHERE id = @id";
                    using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@id", id);
                        int rowsAffected = deleteCmd.ExecuteNonQuery();
                    }

                    dataGridView1.Rows.Remove(row);
                }
            }
            catch (MySqlException ex)
            {
                new MyMessageBox("Chyba pri mazaní z databázy: " + ex.Message,
                    "Chyba", MessageBoxIcon.Error).ShowDialog();
            }
            finally
            {
                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1)
            {
                new MyMessageBox(
                    "Prosím, vyberte práve jedného používateľa na úpravu.",
                    "Neplatný výber",
                    MessageBoxIcon.Warning).ShowDialog();
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

            int userId = Convert.ToInt32(selectedRow.Cells[0].Value);  
            string username = selectedRow.Cells[1].Value.ToString();   
            string role = selectedRow.Cells[2].Value.ToString();       
            int roleId = role == "Admin" ? 1 : 2;                      

            // Otvoríme new_password.cs a pošleme mu údaje
            new_password changeForm = new new_password(userId, username, roleId);
            changeForm.ShowDialog();

            // Po zatvorení formulára obnovíme zoznam používateľov
            load();
        }
    }
}
