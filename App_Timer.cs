using Password_manager;
using System;
using System.Windows.Forms;

public class AppTimer : IMessageFilter
{
    private Timer inactivityTimer;

    public AppTimer()
    {
        inactivityTimer = new Timer();
        inactivityTimer.Interval = 300000; 
        inactivityTimer.Tick += Timer_Tick;
        inactivityTimer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        inactivityTimer.Stop();
        foreach (Form form in Application.OpenForms)
        {
            form.Hide();
        }

        Form messagebox = new MyMessageBox("The application has\nbeen inactive for\ntoo long and will now\nclose.",
                "Inactivity Detected",
                MessageBoxIcon.Information);
        messagebox.ShowDialog();
        Application.Exit(); 
    }

    public bool PreFilterMessage(ref Message m)
    {
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_KEYDOWN = 0x0100;

        if (m.Msg == WM_MOUSEMOVE || m.Msg == WM_KEYDOWN)
        {
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }

        return false;
    }
}