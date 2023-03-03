using Microsoft.Web.WebView2.Core;
using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public static string LectionSource;
        public Lections(bool admin)
        {
            InitializeComponent();
            InitializeBrowser();
            MainViewModel.MainWindow.Title = "Лекции";
            Web.Margin = new Thickness(0, TopStackPanelScroll.ActualHeight, 0, 0);
        }

        private async void InitializeBrowser()
        {
            var userDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PsychoTest";
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

        private async void DevButton_Click(object sender, RoutedEventArgs e)
        {
            await Web.ExecuteScriptAsync("document.designMode = \"on\"");
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string html = File.ReadAllText(LectionSource, Encoding.UTF8);
            html = html.Remove(html.IndexOf("<body"));

            string outerHTML = await Web.ExecuteScriptAsync("document.body.outerHTML.toString()");
            outerHTML = Regex.Unescape(outerHTML);
            outerHTML = outerHTML.Remove(0, 1);
            outerHTML = outerHTML.Remove(outerHTML.Length - 1, 1);

            string fullHtml = html+outerHTML;
            File.WriteAllText(Environment.CurrentDirectory + $"\\Lections\\{LectionTitleTB.Text}.html", fullHtml);
            (DataContext as LectionsViewModel).UpdateLectionList();
        }

        private void DeleteLectionBT_Click(object sender, RoutedEventArgs e)
        {
            Web.Dispose();
            File.Delete(LectionSource);
            (DataContext as LectionsViewModel).UpdateLectionList();
            (DataContext as LectionsViewModel).LectionTitle = null;
        }
    }
}
