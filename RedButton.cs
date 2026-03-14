using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Password_manager
{
    internal class RedButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.Clear(this.BackColor);

            using (Pen whitePen = new Pen(Color.White, 2))
            {
                g.DrawRectangle(whitePen, 1, 1, this.Width - 3, this.Height - 3);
            }

            TextRenderer.DrawText(g, this.Text, this.Font,
                this.ClientRectangle, this.ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            if (this.Focused)
            {
                using (Pen redPen = new Pen(Color.Red, 4))
                {
                    g.DrawRectangle(redPen, -1, -1, this.Width + 1, this.Height + 1);
                }
            }
        }

    }
}
