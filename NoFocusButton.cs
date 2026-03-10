using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Password_manager
{
    internal class NoFocusButton : Button
    {
        public NoFocusButton()
        {
            // Nastavíme vlastné vykreslenie
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;

            // Zapneme vyhladzovanie pre krajšie okraje
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Vymažeme pozadie
            g.Clear(this.BackColor);

            // Vždy vykreslíme biele ohraničenie
            using (Pen whitePen = new Pen(Color.White, 2))
            {
                g.DrawRectangle(whitePen, 1, 1, this.Width - 3, this.Height - 3);
            }

            // Vykreslíme text
            TextRenderer.DrawText(g, this.Text, this.Font,
                this.ClientRectangle, this.Enabled ? this.ForeColor : Color.Gray,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // Ak má tlačidlo focus, pridáme červený rámček OKOLO bieleho
            if (this.Focused)
            {
                using (Pen redPen = new Pen(Color.Red, 4))
                {
                    // Červený rámček OKOLO bieleho (posunutý von)
                    g.DrawRectangle(redPen, -1, -1, this.Width + 1, this.Height + 1);
                }
            }
        }

        // Zabráni default focus kresleniu
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.Invalidate(); // Prekreslíme s focusom
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.Invalidate(); // Prekreslíme bez focusu
        }

    }
}
