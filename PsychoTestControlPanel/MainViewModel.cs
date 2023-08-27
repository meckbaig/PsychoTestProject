using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Diagnostics;

namespace PsychoTestControlPanel
{
    internal class MainViewModel
    {
        public static Frame? MainFrame { get; set; }
        public MainViewModel(Frame mainFrame)
        {
            MainFrame = mainFrame;
        }

        public static void RunAsAdmin()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Process.GetCurrentProcess().MainModule.FileName;
            psi.UseShellExecute = true;
            psi.Verb = "runas";
            Process.Start(psi);
            Application.Current.Shutdown();
        }

        public static bool IsAdmin()
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
}
