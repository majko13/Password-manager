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
    public partial class credentials : Form
    {
        public credentials()
        {
            InitializeComponent();

            

            pictureBox1.SendToBack();

            pictureBox1.Image = Properties.Resources.cross_square_svgrepo_com__3_;


            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Location = new System.Drawing.Point(992, 0);
            pictureBox1.Size = new System.Drawing.Size(35, 35);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
