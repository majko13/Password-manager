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

        // Referencie na dynamicky vytvorené tlačidlá
        private Button modreTlacitko;
        private NoFocusButton cerveneTlacitko;
        private Button vedlajsieTlacitko;
        private NoFocusButton vedlajsieCTlacitko;

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
                    pictureBox1.Image = Properties.Resources.Blue;
                    break;

                case MessageBoxIcon.Warning:
                    pictureBox2.Image = SystemIcons.Warning.ToBitmap();
                    panel1.BackColor = Color.Red;
                    panel2.BackColor = Color.Red;
                    panel3.BackColor = Color.Red;
                    panel4.BackColor = Color.Red;
                    pictureBox1.Image = Properties.Resources.Red;
                    break;

                case MessageBoxIcon.Error:
                    pictureBox2.Image = SystemIcons.Error.ToBitmap();
                    panel1.BackColor = Color.Red;
                    panel2.BackColor = Color.Red;
                    panel3.BackColor = Color.Red;
                    panel4.BackColor = Color.Red;
                    pictureBox1.Image = Properties.Resources.Red;
                    break;

                case MessageBoxIcon.Question:
                    pictureBox2.Image = SystemIcons.Question.ToBitmap();
                    panel1.BackColor = SystemColors.Highlight;
                    panel2.BackColor = SystemColors.Highlight;
                    panel3.BackColor = SystemColors.Highlight;
                    panel4.BackColor = SystemColors.Highlight;
                    pictureBox1.Image = Properties.Resources.Blue;
                    break;

                default:
                    pictureBox2.Visible = false;
                    break;
            }
        }

        // Metóda na nastavenie spoločných properties pre tlačidlá
        private void NastavPropertiesTlacidla(Button button, string text, Color backColor, int x, int y)
        {
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.Text = text;
            button.Font = new Font("Bahnschrift", 14, FontStyle.Bold);
            button.Size = new Size(80,40);
            button.Location = new Point(x, y);
            button.Cursor = Cursors.Hand;
        }

        private void NastavPropertiesTlacidla(NoFocusButton button, string text, Color backColor, int x, int y)
        {
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.Text = text;
            button.Font = new Font("Bahnschrift", 14, FontStyle.Bold);
            button.Size = new Size(75, 35);
            button.Location = new Point(x, y);
            button.Cursor = Cursors.Hand;
        }

        // Konštruktor s ikonou - jedno tlačidlo OK
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

                // Vytvorenie tlačidla podľa ikony až po načítaní formu
                int buttonX =261;
                int buttonY = 85;

                if (icon == MessageBoxIcon.Information || icon == MessageBoxIcon.Question)
                {
                    // Modré tlačidlo
                    modreTlacitko = new Button();
                    NastavPropertiesTlacidla(modreTlacitko, "OK", SystemColors.Highlight, buttonX, buttonY);
                    modreTlacitko.DialogResult = DialogResult.OK;

                    this.Controls.Add(modreTlacitko);
                    AcceptButton = modreTlacitko;
                }
                else
                {
                    // Červené tlačidlo (NoFocusButton)
                    cerveneTlacitko = new NoFocusButton();
                    // Pre NoFocusButton musíme nastaviť properties ručne
                    NastavPropertiesTlacidla(cerveneTlacitko, "OK", Color.Red, buttonX+2, buttonY+2);
                    cerveneTlacitko .DialogResult = DialogResult.OK;

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

        // Konštruktor bez ikony
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

                // Vytvoríme modré tlačidlo
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

        // Konštruktor s ikonou a tlačidlami (Yes/No)
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
            pictureBox2.Location = new Point(272, 30);
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

                int textWidth = Math.Max(label1.Width, label2.Width) + 80;
                int textHeight = label1.Height + label2.Height + 100;

                this.Size = new Size(
                    Math.Max(textWidth, minWidth),
                    Math.Max(textHeight, minHeight)
                );

                // Konstanty pre umiestnenie - 20px od pravého okraja a 20px od spodku
                int marginRight = 20;
                int marginBottom = 20;
                int medzeraMedziTlacitkami = 10;

                // Vytvorenie tlačidiel podľa ikony
                if (icon == MessageBoxIcon.Information || icon == MessageBoxIcon.Question)
                {
                    // Vedľajšie tlačidlo - modré (No) - bude viac vpravo
                    vedlajsieTlacitko = new Button();
                    NastavPropertiesTlacidla(vedlajsieTlacitko, "NO", SystemColors.Highlight, 0, 0);
                    vedlajsieTlacitko.DialogResult = DialogResult.No;

                    // Umiestnenie - od pravého okraja
                    vedlajsieTlacitko.Location = new Point(
                        this.ClientSize.Width - vedlajsieTlacitko.Width - marginRight,
                        this.ClientSize.Height - vedlajsieTlacitko.Height - marginBottom
                    );

                    vedlajsieTlacitko.BringToFront();
                    this.Controls.Add(vedlajsieTlacitko);

                    // Hlavné tlačidlo - modré (Yes) - bude naľavo od vedľajšieho
                    modreTlacitko = new Button();
                    NastavPropertiesTlacidla(modreTlacitko, "YES", SystemColors.Highlight, 0, 0);
                    modreTlacitko.DialogResult = DialogResult.Yes;

                    // Umiestnenie - vedľa No tlačidla s medzerou
                    modreTlacitko.Location = new Point(
                        vedlajsieTlacitko.Location.X - modreTlacitko.Width - medzeraMedziTlacitkami,
                        this.ClientSize.Height - modreTlacitko.Height - marginBottom
                    );

                    modreTlacitko.BringToFront();
                    this.Controls.Add(modreTlacitko);
                    AcceptButton = modreTlacitko;
                }
                else // Červená
                {
                    vedlajsieCTlacitko = new NoFocusButton();
                    NastavPropertiesTlacidla(vedlajsieCTlacitko, "NO", Color.Red, 0, 0);
                    vedlajsieCTlacitko.DialogResult = DialogResult.No;

                    vedlajsieCTlacitko.Location = new Point(
                        this.ClientSize.Width - vedlajsieCTlacitko.Width - marginRight,
                        this.ClientSize.Height - vedlajsieCTlacitko.Height - marginBottom
                    );

                    vedlajsieCTlacitko.BringToFront();
                    this.Controls.Add(vedlajsieCTlacitko);

                    cerveneTlacitko = new NoFocusButton();
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