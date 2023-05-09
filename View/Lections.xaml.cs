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
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
            Admin = admin;
            InitializeComponent();
            InitializeBrowser();
            TopStackPanelScroll.IsEnabled = Admin;
            TopStackPanelScroll.Visibility = (Visibility)(!Admin ? 1 : 0);
            TopStackPanelScroll.Height = Admin ? Double.NaN : 0;
            Web.Margin = new Thickness(0, TopStackPanelScroll.ActualHeight, 0, 0);
            MainViewModel.MainWindow.Title = this.Title;
        }

        private async void InitializeBrowser()
        {
            try
            {
                var env = await CoreWebView2Environment.CreateAsync(null, MainViewModel.UserDataFolder);
                await Web.EnsureCoreWebView2Async(env);
                Thread.Sleep(50);
                DataContext = new LectionsViewModel(Web, TopStackPanelScroll);
                Thread.Sleep(50);
                if (Admin)
                    Web.ExecuteScriptAsync("document.designMode = \"on\"");
                MainViewModel.AllButtonsHover(this.Content);

            }
            catch (WebView2RuntimeNotFoundException)
            {
                switch (RuntimeInformation.OSArchitecture)
                {
                    case Architecture.X86:
                        Process.Start(new ProcessStartInfo("https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/0b8bc328-68ba-4cef-bcbc-5814ebe9e775/Microsoft.WebView2.FixedVersionRuntime.113.0.1774.35.x86.cab") { UseShellExecute = true} );
                        break;
                    case Architecture.X64:
                        Process.Start(new ProcessStartInfo("https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/aa9b4217-8ead-42f3-837d-9d42bab92d80/Microsoft.WebView2.FixedVersionRuntime.113.0.1774.35.x64.cab") { UseShellExecute = true} );
                        break;
                }
                WpfMessageBox.Show("Для работы с данным модулем программы требуется установить программную среду Microsoft Edge WebView2.", WpfMessageBox.MessageBoxType.Error);
                MainViewModel.Back();
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(ex.Message, WpfMessageBox.MessageBoxType.Error);
            }

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
            if (LectionSource != null)
            {
                if (WpfMessageBox.Show($"Вы точно хотите удалить лекцию \"{(DataContext as LectionsViewModel).LectionTitle}\"?", 
                    "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    File.Delete(LectionSource);
                    DataContext = new LectionsViewModel(Web, TopStackPanelScroll);
                    Thread.Sleep(50);
                    if (Admin)
                        Web.ExecuteScriptAsync("document.designMode = \"on\"");
                    (DataContext as LectionsViewModel).UpdateLectionList();
                }
            }
            else
                WpfMessageBox.Show("Выберите лекцию", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void BoldBT_Click(object sender, RoutedEventArgs e)
        {
            //string sdgfsdg = await Web.ExecuteScriptAsync("window.getSelection().toString()");
            AddModifierToText("bold");
        }
        private async void ItalicBT_Click(object sender, RoutedEventArgs e)
        {
            AddModifierToText("italic");
        }
        private async void UnderlineBT_Click(object sender, RoutedEventArgs e)
        {
            AddModifierToText("underline");
        }
        private async void StrikethroughBT_Click(object sender, RoutedEventArgs e)
        {
            AddModifierToText("strikethrough");
        }

        private async void AddModifierToText(string modifier)
        {
            List<string> modifiers = new List<string> { "h1", "h2", "h3", "h4", "italic", "bold", "underline", "strikethrough" }; // список модификаторов
            modifiers.Remove(modifier);
            string tag = ReturnTag(modifier);
            string newNode = await Web.ExecuteScriptAsync("window.getSelection().toString()");

            foreach (string m in modifiers) // устанавливаем все потерянные теги у текста
            {
                string t = ReturnTag(m);
                if (await Web.ExecuteScriptAsync($"document.queryCommandState('{m}');") == "true")
                    newNode = $"\"<{t}>\"+{newNode}+\"</{t}>\"";
            }

            if (await Web.ExecuteScriptAsync($"document.queryCommandState('{modifier}');") != "true")
                newNode = $"\"<{tag}>\"+{newNode}+\"</{tag}>\"";
            //    await Web.ExecuteScriptAsync($"document.execCommand('removeFormat', false, '{tag}')");
            //else

            await Web.ExecuteScriptAsync(
                "var sel = window.getSelection();" +
                "if (sel.rangeCount)" +
                "{" +
                    "var range = sel.getRangeAt(0);" +
                    "var newNode = document.createElement(\"span\");" +
                    $"newNode.innerHTML = {newNode};" +
                    "range.deleteContents();" +
                    "range.insertNode(newNode);" +
                    "var parentElement = range.commonAncestorContainer;" +
                    "while (parentElement.nodeName != 'SPAN') " +
                    "{" +
                        "parentElement = parentElement.parentNode;" +
                    "}" +
                    "var textInside = parentElement.textContent;" +
                    "parentElement.outerHTML = textInside;" +
                "};");

        }

        private static string ReturnTag(string modifier)
        {
            string tag;
            switch (modifier)
            {
                case "italic": tag = "em"; break;
                case "bold": tag = "strong"; break;
                case "underline": tag = "u"; break;
                case "strikethrough": tag = "s"; break;
                default: tag = modifier; break;
            }
            return tag;
        }

        private async void GetFontSize()
        {
            //FontSizeCB.SelectedItem = await Web.ExecuteScriptAsync("window.getComputedStyle(window.getSelection().anchorNode.parentElement).fontSize;");
        }

        private async void LeftBT_Click(object sender, RoutedEventArgs e)
        {
            await Web.ExecuteScriptAsync("document.execCommand('justifyLeft', false, null);");
        }

        private async void CenterBT_Click(object sender, RoutedEventArgs e)
        {
            await Web.ExecuteScriptAsync("document.execCommand('justifyCenter', false, null);");
        }

        private async void RightBT_Click(object sender, RoutedEventArgs e)
        {
            await Web.ExecuteScriptAsync("document.execCommand('justifyRight', false, null);");
        }

        private async void JustifyBT_Click(object sender, RoutedEventArgs e)
        {
            await Web.ExecuteScriptAsync("document.execCommand('justifyFull', false, null);");
        }
    }
}
