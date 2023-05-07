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

namespace PsychoTestControlPanel
{
    internal class MainViewModel
    {
        public static Frame? MainFrame { get; set; }
        public MainViewModel(Frame mainFrame)
        {
            MainFrame = mainFrame;
        }
    }
}
