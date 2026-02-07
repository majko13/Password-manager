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
    public partial class CAPTCHA : Form
    {
        private string captchaCode;
        private Random rand = new Random();
        private bool mouseDown;
        private Point lastLocation;

        private void GenerateCaptcha()
        {
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
            captchaCode = "";
            for (int i = 0; i < 6; i++)
            {
                captchaCode += chars[rand.Next(chars.Length)];
            }

            // Uvolnění předchozího obrázku
            if (pictureBox2.Image != null)
            {
                pictureBox2.Image.Dispose();
                pictureBox2.Image = null;
            }

            // PŘIDAT USING pro Bitmap!
            using (Bitmap bmp = new Bitmap(265, 104))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // pozadí
                g.Clear(Color.White);

                // šum – tečky
                for (int i = 0; i < 150; i++)
                {
                    int x = rand.Next(bmp.Width);
                    int y = rand.Next(bmp.Height);
                    bmp.SetPixel(x, y, Color.FromArgb(rand.Next(50, 200), rand.Next(50, 200), rand.Next(50, 200)));
                }

                // šum – čáry
                for (int i = 0; i < 10; i++)
                {
                    int x1 = rand.Next(bmp.Width);
                    int y1 = rand.Next(bmp.Height);
                    int x2 = rand.Next(bmp.Width);
                    int y2 = rand.Next(bmp.Height);
                    using (Pen pen = new Pen(Color.Gray))
                    {
                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                }

                // kreslení znaků s rotací, posunem a barvou
                for (int i = 0; i < captchaCode.Length; i++)
                {
                    float angle = rand.Next(-30, 30);
                    g.TranslateTransform(20 * i + 35, 40);
                    g.RotateTransform(angle);

                    Color color = Color.FromArgb(rand.Next(50, 150), rand.Next(50, 150), rand.Next(50, 150));

                    using (Brush brush = new SolidBrush(color))
                    using (Font font = new Font("Arial", rand.Next(20, 35), FontStyle.Bold))
                    {
                        g.DrawString(captchaCode[i].ToString(), font, brush, 0, -15);
                    }

                    g.ResetTransform();
                }

                // Vytvořit nový Bitmap pro pictureBox
                pictureBox2.Image = new Bitmap(bmp);
            } // Zde se bmp automaticky uvolní
        }
        public CAPTCHA()
        {
            InitializeComponent();

            pictureBox1.SendToBack();

            pictureBox1.Image = Properties.Resources.Blue;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(220, -2);
            pictureBox1.Size = new System.Drawing.Size(35, 35);

            GenerateCaptcha();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            // Uvolnění obrázku při zavření formuláře
            if (pictureBox2.Image != null)
            {
                pictureBox2.Image.Dispose();
                pictureBox2.Image = null;
            }

            base.Dispose(disposing);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GenerateCaptcha();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == captchaCode)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                Form messagebox = new MyMessageBox("CAPTCHA code is incorrect.\nPlease try again.", "Error", MessageBoxIcon.Error);
                messagebox.ShowDialog();

                textBox1.Text = "";
                GenerateCaptcha();
            }
        }

        private void CAPTCHA_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void CAPTCHA_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void CAPTCHA_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}