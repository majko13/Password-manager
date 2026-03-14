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

        private Button modreTlacitko;
        private RedButton cerveneTlacitko;
        private Button vedlajsieTlacitko;
        private RedButton vedlajsieCTlacitko;

        private void SetIcon(MessageBoxIcon icon)
        {
            if (icon == MessageBoxIcon.Information || icon == MessageBoxIcon.Question)
            {
                pictureBox2.Image = SystemIcons.Information.ToBitmap();
                panel1.BackColor = SystemColors.Highlight;
                panel2.BackColor = SystemColors.Highlight;
                panel3.BackColor = SystemColors.Highlight;
                panel4.BackColor = SystemColors.Highlight;
                pictureBox1.Image = Properties.Resources.Blue;
            }
            else if (icon == MessageBoxIcon.Error || icon == MessageBoxIcon.Warning)
            {
                pictureBox2.Image = SystemIcons.Warning.ToBitmap();
                panel1.BackColor = Color.Red;
                panel2.BackColor = Color.Red;
                panel3.BackColor = Color.Red;
                panel4.BackColor = Color.Red;
                pictureBox1.Image = Properties.Resources.Red;
            }
        }

        private void NastavPropertiesTlacidla(Button button, string text, Color backColor, int x, int y)
        {
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.Text = text;
            button.Font = new Font("Bahnschrift", 14, FontStyle.Bold);
            button.Size = new Size(80, 40);
            button.Location = new Point(x, y);
            button.Cursor = Cursors.Hand;
        }

        private void NastavPropertiesTlacidla(RedButton button, string text, Color backColor, int x, int y)
        {
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.Text = text;
            button.Font = new Font("Bahnschrift", 14, FontStyle.Bold);
            button.Size = new Size(75, 35);
            button.Location = new Point(x, y);
            button.Cursor = Cursors.Hand;
        }

        public MyMessageBox(string description, string header, MessageBoxIcon icon)
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

            this.Load += (s, e) =>
            {
                int minWidth = 400;
                int minHeight = 150;

                int textWidth = Math.Max(label1.Width, label2.Width) + 100;
                int textHeight = label1.Height + label2.Height + 60;

                this.Size = new Size(
                    Math.Max(textWidth, minWidth),
                    Math.Max(textHeight, minHeight)
                );

                int buttonX = 261;
                int buttonY = 85;

                if (icon == MessageBoxIcon.Information || icon == MessageBoxIcon.Question)
                {
                    modreTlacitko = new Button();
                    NastavPropertiesTlacidla(modreTlacitko, "OK", SystemColors.Highlight, buttonX, buttonY);
                    modreTlacitko.DialogResult = DialogResult.OK;

                    this.Controls.Add(modreTlacitko);
                    AcceptButton = modreTlacitko;
                }
                else
                {
                    cerveneTlacitko = new RedButton();
                    NastavPropertiesTlacidla(cerveneTlacitko, "OK", Color.Red, buttonX + 15, buttonY + 5);
                    cerveneTlacitko.DialogResult = DialogResult.OK;
                    pictureBox2.Location = new Point(295, 30);
                    this.Controls.Add(cerveneTlacitko);
                    AcceptButton = cerveneTlacitko;
                }
            };

            AddMouseEventsToAllControls(this);
        }

        private void AddMouseEventsToAllControls(Control parent)
        {
            if (parent is Button || parent is PictureBox || parent is DataGridView)
                return;

            parent.MouseDown += MyMessageBox_MouseDown;
            parent.MouseMove += MyMessageBox_MouseMove;
            parent.MouseUp += MyMessageBox_MouseUp;

            foreach (Control ctrl in parent.Controls)
            {
                AddMouseEventsToAllControls(ctrl);
            }
        }

        public MyMessageBox(string description, string header)
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

            pictureBox1.Image = Properties.Resources.Blue;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new Point(315, -2);
            pictureBox1.Size = new Size(35, 35);
            pictureBox1.SendToBack();

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

                int buttonX = 240;
                int buttonY = 270;

                modreTlacitko = new Button();
                NastavPropertiesTlacidla(modreTlacitko, "OK", SystemColors.Highlight, buttonX, buttonY);
                modreTlacitko.DialogResult = DialogResult.OK;
                this.Controls.Add(modreTlacitko);
                AcceptButton = modreTlacitko;
                modreTlacitko.BringToFront();

                AddMouseEventsToAllControls(this);
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
            pictureBox2.Location = new Point(322, 30);
            pictureBox2.Size = new Size(45, 45);
            pictureBox2.SendToBack();
            


            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new Point(415, -2);
            pictureBox1.Size = new Size(35, 35);
            pictureBox1.SendToBack();
            pictureBox1.Click += delegate
            {
                this.DialogResult = DialogResult.No;
            };

            this.Load += (s, e) =>
            {
                int minWidth = 450;
                int minHeight = 150;

                int textWidth = Math.Max(label1.Width, label2.Width) + 80;
                int textHeight = label1.Height + label2.Height + 100;

                this.Size = new Size(
                    Math.Max(textWidth, minWidth),
                    Math.Max(textHeight, minHeight)
                );

                pictureBox2.Location = new Point(pictureBox2.Location.X, ((this.ClientSize.Height - pictureBox2.Height) / 2)-15);

                int marginRight = 20;
                int marginBottom = 20;
                int medzeraMedziTlacitkami = 10;

                if (icon == MessageBoxIcon.Information || icon == MessageBoxIcon.Question)
                {
                    vedlajsieTlacitko = new Button();
                    NastavPropertiesTlacidla(vedlajsieTlacitko, "NO", SystemColors.Highlight, 0, 0);
                    vedlajsieTlacitko.DialogResult = DialogResult.No;
                  
                    vedlajsieTlacitko.Location = new Point(
                        this.ClientSize.Width - vedlajsieTlacitko.Width - marginRight,
                        this.ClientSize.Height - vedlajsieTlacitko.Height - marginBottom
                    );

                    vedlajsieTlacitko.BringToFront();
                    this.Controls.Add(vedlajsieTlacitko);

                    modreTlacitko = new Button();
                    NastavPropertiesTlacidla(modreTlacitko, "YES", SystemColors.Highlight, 0, 0);
                    modreTlacitko.DialogResult = DialogResult.Yes;

                    modreTlacitko.Location = new Point(
                        vedlajsieTlacitko.Location.X - modreTlacitko.Width - medzeraMedziTlacitkami,
                        this.ClientSize.Height - modreTlacitko.Height - marginBottom
                    );

                    modreTlacitko.BringToFront();
                    this.Controls.Add(modreTlacitko);
                    AcceptButton = modreTlacitko;
                }
                else 
                {
                    vedlajsieCTlacitko = new RedButton();
                    NastavPropertiesTlacidla(vedlajsieCTlacitko, "NO", Color.Red, 0, 0);
                    vedlajsieCTlacitko.DialogResult = DialogResult.No;


                    vedlajsieCTlacitko.Location = new Point(
                        this.ClientSize.Width - vedlajsieCTlacitko.Width - marginRight,
                        this.ClientSize.Height - vedlajsieCTlacitko.Height - marginBottom
                    );

                    vedlajsieCTlacitko.BringToFront();
                    this.Controls.Add(vedlajsieCTlacitko);

                    cerveneTlacitko = new RedButton();
                    NastavPropertiesTlacidla(cerveneTlacitko, "YES", Color.Red, 0, 0);


                    cerveneTlacitko.DialogResult = DialogResult.Yes;

                    cerveneTlacitko.Location = new Point(
                        vedlajsieCTlacitko.Location.X - cerveneTlacitko.Width - medzeraMedziTlacitkami,
                        this.ClientSize.Height - cerveneTlacitko.Height - marginBottom
                    );

                    cerveneTlacitko.BringToFront();
                    this.Controls.Add(cerveneTlacitko);
                    AcceptButton = cerveneTlacitko;
                }
                AddMouseEventsToAllControls(this);
            };
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MyMessageBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = Cursor.Position;
        }

        private void MyMessageBox_MouseMove(object sender, MouseEventArgs e)
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

        private void MyMessageBox_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}