using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main()
    {
        string exePath = ConfigurationManager.AppSettings["TargetExePath"] ?? "";

        if (string.IsNullOrWhiteSpace(exePath) || !System.IO.File.Exists(exePath))
        {
            MessageBox.Show("Executable path is missing or invalid.", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                WindowStyle = ProcessWindowStyle.Minimized,
                UseShellExecute = true
            };

            Process.Start(psi);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to start process:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
