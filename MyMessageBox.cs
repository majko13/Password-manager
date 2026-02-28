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
                    pictureBox1.Image = Properties.Resources.Blue;

                    break;

                case MessageBoxIcon.Warning:
                    pictureBox2.Image = SystemIcons.Warning.ToBitmap();
                    panel1.BackColor = Color.Red;
                    panel2.BackColor = Color.Red;
                    panel3.BackColor = Color.Red;
                    panel4.BackColor = Color.Red;
                    button1.BackColor = Color.Red;
                    pictureBox1.Image = Properties.Resources.Red;
                    break;

                case MessageBoxIcon.Error:
                    pictureBox2.Image = SystemIcons.Error.ToBitmap();
                    panel1.BackColor = Color.Red;
                    panel2.BackColor = Color.Red;
                    panel3.BackColor = Color.Red;
                    panel4.BackColor = Color.Red;
                    button1.BackColor = Color.Red;
                    pictureBox1.Image = Properties.Resources.Red;
                    break;

                case MessageBoxIcon.Question:
                    pictureBox2.Image = SystemIcons.Question.ToBitmap();
                    panel1.BackColor = SystemColors.Highlight;
                    panel2.BackColor = SystemColors.Highlight;
                    panel3.BackColor = SystemColors.Highlight;
                    panel4.BackColor = SystemColors.Highlight;
                    button1.BackColor = SystemColors.Highlight;
                    pictureBox1.Image = Properties.Resources.Blue;
                    break;

                default:
                    pictureBox2.Visible = false;
                    break;
            }
        }
        public MyMessageBox(string description, string header, MessageBoxIcon icon)
        {
            InitializeComponent();

            label2.Text = header;
            label1.Text = description;
            button2.Visible = false;

            label1.AutoSize = true;
            label1.MaximumSize = new Size(250, 0);
            label1.Location = new Point(20, 40);

            label2.Location = new Point(20, 10);
            label2.AutoSize = true;
            label2.Font = new Font(label2.Font, FontStyle.Bold);

            SetIcon(icon);
            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox2.Location = new Point(280, 30);
            pictureBox2.Size = new Size(45, 45);
            pictureBox2.SendToBack();

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new Point(365, -2);
            pictureBox1.Size = new Size(35, 35);
            pictureBox1.SendToBack();

            button1.FlatAppearance.BorderSize = 2;
            button1.FlatAppearance.BorderColor = Color.Red;
            button1.Text = "OK";
            button1.Size = new Size(75, 30);
            button1.Anchor = AnchorStyles.None;
            AcceptButton = button1;
            button1.DialogResult = DialogResult.OK;

            this.Load += (s, e) =>
            {
                int minWidth = 400;
                int minHeight = 150;

                int textWidth = Math.Max(label1.Width, label2.Width) + 100;
                int textHeight = label1.Height + label2.Height + 80;

                this.Size = new Size(
                    Math.Max(textWidth, minWidth),
                    Math.Max(textHeight, minHeight)
                );

                button1.Location = new Point(
                    pictureBox2.Location.X + (pictureBox2.Width - button1.Width) / 2,
                    pictureBox2.Location.Y + pictureBox2.Height + 15
                );
            };
        }
        public MyMessageBox(string description, string header)
        {
            InitializeComponent();

            label2.Text = header;
            label1.Text = description;
            button2.Visible = false;

            label1.AutoSize = true;
            label1.MaximumSize = new Size(250, 0);
            label1.Location = new Point(20, 40);

            label2.Location = new Point(20, 10);
            label2.AutoSize = true;
            label2.Font = new Font(label2.Font, FontStyle.Bold);

            pictureBox1.Image = Properties.Resources.Blue;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new Point(315, -2);
            pictureBox1.Size = new Size(35, 35);
            pictureBox1.SendToBack();

            button1.FlatAppearance.BorderSize = 2;
            button1.FlatAppearance.BorderColor = Color.Red;
            button1.Text = "OK";
            button1.Size = new Size(90, 40);
            button1.Location = new Point(220, 180);
            button1.Anchor = AnchorStyles.None;
            AcceptButton = button1;
            button1.DialogResult = DialogResult.OK;

            this.Load += (s, e) =>
            {
                int minWidth = 350;
                int minHeight = 120;

                int textWidth = Math.Max(label1.Width, label2.Width) + 100;
                int textHeight = label1.Height + label2.Height + 80;

                this.Size = new Size(
                    Math.Max(textWidth, minWidth),
                    Math.Max(textHeight, minHeight)
                );

            };
        }
        public MyMessageBox(string description, string header, MessageBoxIcon icon, MessageBoxButtons buttons)
        {
            InitializeComponent();

            label2.Text = header;
            label1.Text = description;

            label1.AutoSize = true;
            label1.MaximumSize = new Size(250, 0);
            label1.Location = new Point(20, 40);

            label2.Location = new Point(20, 10);
            label2.AutoSize = true;
            label2.Font = new Font(label2.Font, FontStyle.Bold);

            SetIcon(icon);
            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox2.Location = new Point(280, 30);
            pictureBox2.Size = new Size(45, 45);
            pictureBox2.SendToBack();

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new Point(365, -2);
            pictureBox1.Size = new Size(35, 35);
            pictureBox1.SendToBack();

            button1.Text = "Yes";
            button1.DialogResult = DialogResult.Yes;
            button1.Location = new Point(165, 95); // ľavé tlačidlo

            button2.Text = "No";
            button2.DialogResult = DialogResult.No;
            button2.Location = new Point(250, 95); // ľavé tlačidlo
            

            button1.FlatAppearance.BorderSize = 2;
            button1.FlatAppearance.BorderColor = Color.Red;
            button1.Size = new Size(75, 30);
            button1.Anchor = AnchorStyles.None;

            button2.FlatAppearance.BorderSize = 2;
            button2.FlatAppearance.BorderColor = Color.Red;
            button2.Size = new Size(75, 30);
            button2.Anchor = AnchorStyles.None;

            this.Load += (s, e) =>
            {
                int minWidth = 400;
                int minHeight = 150;

                int textWidth = Math.Max(label1.Width, label2.Width) + 100;
                int textHeight = label1.Height + label2.Height + 80;


                this.Size = new Size(
                    Math.Max(textWidth, minWidth),
                    Math.Max(textHeight, minHeight)
                );

            };
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
