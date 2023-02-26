using Microsoft.Web.WebView2.Core;
using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace PsychoTestProject
{
    /// <summary>
    /// Логика взаимодействия для Lections.xaml
    /// </summary>
    public partial class Lections : Page
    {

        public Lections(bool admin)
        {
            InitializeComponent();
            InitializeBrowser();
            MainViewModel.MainWindow.Title = "Лекции";
            Web.Margin = new Thickness(0, TopStackPanelScroll.ActualHeight, 0, 0);
        }

        private async void InitializeBrowser()
        {
            var userDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SoftwareName";
            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
            await Web.EnsureCoreWebView2Async(env);
            Thread.Sleep(50);
            DataContext = new LectionsViewModel(Web, TopStackPanelScroll);


        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Back();
            Web.Dispose();
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Web.Margin = new Thickness(0, TopStackPanelScroll.ActualHeight, 0, 0);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Web.ExecuteScriptAsync("document.designMode = \"on\"");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string innerHtml = await Web.ExecuteScriptAsync("document.body.innerHTML.toString()");
            
        }
    }
}
