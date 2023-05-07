using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PsychoTestControlPanel
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public List<SettingsParameter> Parameters { get; set; } = new List<SettingsParameter>();
        public double ListWidth { get => Width-50; }

        public Settings()
        {
            InitializeComponent();
            DataContext = this;
            foreach (SettingsProperty property in Properties.Settings.Default.Properties)
            {
                try
                {
                    bool asdf = (bool)Properties.Settings.Default[property.Name];
                    Parameters.Add(new SettingsParameter(property.Name));
                }
                catch (Exception)
                {
                }
            }
            Parameters = Parameters.OrderBy(p => p.Property).ToList();
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainViewModel.MainFrame.NavigationService.GoBack();
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Grid parentGrid = (sender as CheckBox).Parent as Grid;
            Properties.Settings.Default[(parentGrid.Children[0] as TextBlock).Text] = (sender as CheckBox).IsChecked;
            Properties.Settings.Default.Save();
        }
    }
}
