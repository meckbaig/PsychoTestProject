using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsychoTestProject.View
{
    /// <summary>
    /// Логика взаимодействия для TestEditor.xaml
    /// </summary>
    public partial class TestEditor : Page
    {
        int previousSelected = -2;
        public TestEditor()
        {
            InitializeComponent();
            MainViewModel.MainWindow.Title = this.Title;
            DataContext = new TestEditorViewModel(EditFrame);
            MainViewModel.AllButtonsHover(this.Content);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //if (MainViewModel.CurrentTest != null)
            //{
            //    if (!(DataContext as TestEditorViewModel).CompareTest())
            //        if (WpfMessageBox.Show($"Выбранный тест не сохранён. Вы точно хотите выйти?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            //        {
            MainViewModel.Back();
            //        }
            //}
            //else
            //    MainViewModel.Back();
        }

        private void ListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ListBox.SelectedIndex == previousSelected)
            {
                ListBox.SelectedItem = null;
                previousSelected = -2;
            }
            else
                previousSelected = ListBox.SelectedIndex;
        }

        private void ListBox_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            ListBox.SelectedItem = null;
            previousSelected = -2;
        }
    }
}
