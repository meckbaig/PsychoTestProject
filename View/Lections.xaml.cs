using Microsoft.Web.WebView2.Core;
using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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

namespace PsychoTestProject
{
    /// <summary>
    /// Логика взаимодействия для Lections.xaml
    /// </summary>
    public partial class Lections : Page
    {
        public static string LectionSource;
        public static bool Admin;
        public Lections(bool admin)
        {
            InitializeComponent();
            InitializeBrowser();
            Admin = admin;
            TopStackPanelScroll.IsEnabled = admin;
            TopStackPanelScroll.Visibility = (Visibility)(!admin ? 1 : 0);
            TopStackPanelScroll.Height = admin ? Double.NaN : 0;
            Web.Margin = new Thickness(0, TopStackPanelScroll.ActualHeight, 0, 0);
            MainViewModel.MainWindow.Title = "Лекции";
        }

        private async void InitializeBrowser()
        {
            var env = await CoreWebView2Environment.CreateAsync(null, MainViewModel.UserDataFolder);
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string html = GetHtmlHead(LectionSource);
            string outerHTML = await GetHtmlBody();

            string fullHtml = html + outerHTML;
            File.WriteAllText(Environment.CurrentDirectory + $"\\Lections\\{LectionTitleTB.Text}.html", fullHtml);
            (DataContext as LectionsViewModel).UpdateLectionList();
        }

        private async Task<string> GetHtmlBody()
        {
            string outerHTML = await Web.ExecuteScriptAsync("document.body.outerHTML.toString()");
            outerHTML = Regex.Unescape(outerHTML);
            outerHTML = outerHTML.Remove(0, 1);
            outerHTML = outerHTML.Remove(outerHTML.Length - 1, 1);
            return outerHTML;
        }

        public static string GetHtmlHead(string source)
        {
            string html = File.ReadAllText(source, Encoding.UTF8);
            html = html.Remove(html.IndexOf("<body"));
            return html;
        }

        private void DeleteLectionBT_Click(object sender, RoutedEventArgs e)
        {
            Web.Dispose();
            File.Delete(LectionSource);
            (DataContext as LectionsViewModel).UpdateLectionList();
            (DataContext as LectionsViewModel).LectionTitle = null;
        }

        private async void BoldBT_Click(object sender, RoutedEventArgs e)
        {
            string sdgfsdg = await Web.ExecuteScriptAsync("window.getSelection()");

        }
    }
}
