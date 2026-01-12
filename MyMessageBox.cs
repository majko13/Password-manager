using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Password_manager
{
    public partial class MyMessageBox : Form
    {

        private bool mouseDown;
        private Point lastLocation;


        private void SetIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    pictureBox2.Image = SystemIcons.Information.ToBitmap();
                    panel1.BackColor = SystemColors.Highlight;
                    panel2.BackColor = SystemColors.Highlight;
                    panel3.BackColor = SystemColors.Highlight;
                    panel4.BackColor = SystemColors.Highlight;
                    button1.BackColor = SystemColors.Highlight;
                    pictureBox1.Image = Properties.Resources.cross_square_svgrepo_com__3_1;

                    break;

                case MessageBoxIcon.Warning:
                    pictureBox2.Image = SystemIcons.Warning.ToBitmap();
                    panel1.BackColor = Color.Red;
                    panel2.BackColor = Color.Red;
                    panel3.BackColor = Color.Red;
                    panel4.BackColor = Color.Red;
                    button1.BackColor = Color.Red;
                    pictureBox1.Image = Properties.Resources.Red1;
                    break;

                case MessageBoxIcon.Error:
                    pictureBox2.Image = SystemIcons.Error.ToBitmap();
                    panel1.BackColor = Color.Red;
                    panel2.BackColor = Color.Red;
                    panel3.BackColor = Color.Red;
                    panel4.BackColor = Color.Red;
                    button1.BackColor = Color.Red;
                    pictureBox1.Image = Properties.Resources.Red1;
                    break;

                case MessageBoxIcon.Question:
                    pictureBox2.Image = SystemIcons.Question.ToBitmap();
                    panel1.BackColor = SystemColors.Highlight;
                    panel2.BackColor = SystemColors.Highlight;
                    panel3.BackColor = SystemColors.Highlight;
                    panel4.BackColor = SystemColors.Highlight;
                    button1.BackColor = SystemColors.Highlight;
                    pictureBox1.Image = Properties.Resources.cross_square_svgrepo_com__3_1;
                    break;

                default:
                    pictureBox2.Visible = false;
                    break;
            }
        }
        public MyMessageBox(string description, string header, MessageBoxIcon icon)
        {
            InitializeComponent();

            pictureBox1.SendToBack();


            
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(273, -2);
            pictureBox1.Size = new System.Drawing.Size(35, 35);
            
            SetIcon(icon);
            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox2.Location = new System.Drawing.Point(221, 30);
            pictureBox2.Width = 45;
            pictureBox2.Height = 45;


            button1.FlatAppearance.BorderSize = 2;
            button1.FlatAppearance.BorderColor = Color.Red;

            


            label2.Text = header;
            label1.Text = description;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MyMessageBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void MyMessageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void MyMessageBox_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}
