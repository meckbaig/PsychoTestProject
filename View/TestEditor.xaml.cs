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
        public int PreviousSelected = -2;
        public TestEditor()
        {
            InitializeComponent();
            MainViewModel.MainWindow.Title = this.Title;
            TestEditorViewModel context = new TestEditorViewModel(EditFrame);
            DataContext = context;
            context.PropertyChanged += Context_PropertyChanged; 
            MainViewModel.AllButtonsHover(this.Content);
        }

        private void Context_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TestList")
            {
                ListBox.SelectedItem = null;
                PreviousSelected = -2;
            }
        }

        private void TestList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ListBox.SelectedItem = null;
            PreviousSelected = -2;
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
            if (ListBox.SelectedIndex == PreviousSelected)
            {
                ListBox.SelectedItem = null;
                PreviousSelected = -2;
            }
            else
                PreviousSelected = ListBox.SelectedIndex;
        }
    }
}
