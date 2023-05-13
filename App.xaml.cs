using PsychoTestProject.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace PsychoTestProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        public static MainWindow MainWindow { get; private set; }

        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
            {
                ((IntegerUpDown)e.Source).Value = (int)e.OldValue;
            }
        }
    }
}
