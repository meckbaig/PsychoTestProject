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
        double ZoomFactor = Properties.Settings.Default.Scale;

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
                Web.NavigationStarting += (s, e) => ZoomFactor = Web.ZoomFactor;
                Web.NavigationCompleted += (s, e) => Web.ZoomFactor = ZoomFactor;
                Thread.Sleep(50);
                DataContext = new LectionsViewModel(Web, TopStackPanelScroll);
                Web.Loaded += (s,e) => Web.ZoomFactor = ZoomFactor;
                Thread.Sleep(50);
                if (Admin)
                    Web.ExecuteScriptAsync("document.designMode = \"on\"");
                MainViewModel.AllButtonsHover(this.Content);
                Web.ZoomFactor = ZoomFactor;
            }
            catch (WebView2RuntimeNotFoundException)
            {
                switch (RuntimeInformation.OSArchitecture)
                {
                    case Architecture.X86:
                        Process.Start(new ProcessStartInfo("https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/26962093-63b6-4345-b9c5-18689284a972/MicrosoftEdgeWebView2RuntimeInstallerX86.exe") { UseShellExecute = true} );
                        break;
                    case Architecture.X64:
                        Process.Start(new ProcessStartInfo("https://msedge.sf.dl.delivery.mp.microsoft.com/filestreamingservice/files/4453dec4-574c-4c11-9d66-dcd44cb91b9c/MicrosoftEdgeWebView2RuntimeInstallerX64.exe") { UseShellExecute = true} );
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
            if (!MainViewModel.FilesEditPermission)
            {
                WpfMessageBox.Show("Программа запущена не от имени администратора, доступ к редактированию файлов запрещён",
                    "Внимание!", MessageBoxButton.OK);
                return;
            }
            try
            {
                string html = GetHtmlHead(LectionSource);
                string outerHTML = await GetHtmlBody();

                string fullHtml = html + outerHTML;
                string fileName = MainViewModel.ProperFileName(LectionTitleTB.Text);
                fileName = MainViewModel.FileNameNotNull(fileName, "Лекция.html", Environment.CurrentDirectory + $"\\Lections");
                File.WriteAllText(Environment.CurrentDirectory + $"\\Lections\\{fileName}.html", fullHtml);
                (DataContext as LectionsViewModel).UpdateLectionList();
            }
            catch (UnauthorizedAccessException)
            {
                MainViewModel.FilesEditPermission = false;
                if (WpfMessageBox.Show($"Ошибка доступа к файлам\n" +
                    $"Хотите перезапустить программу от имени администратора?", "Серьёзная ошибка!",
                    MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    MainViewModel.RunAsAdmin();
                }
            }
            catch (Exception)
            {
                WpfMessageBox.Show("Произошла непредвиденная ошибка!", WpfMessageBox.MessageBoxType.Error);
            }
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
            try
            {
                string html = File.ReadAllText(source, Encoding.UTF8);
                html = html.Remove(html.IndexOf("<body"));
                return html;
            }
            catch (Exception)
            {
                string html = File.ReadAllText(source, Encoding.UTF8);
                html = html.Remove(html.IndexOf("</style>"+7));
                return html;
            }
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

        private void LectionTitleTB_KeyUp(object sender, KeyEventArgs e)
        {
            int start = LectionTitleTB.SelectionStart;
            int len = LectionTitleTB.Text.Length;
            LectionTitleTB.Text = MainViewModel.ProperFileName(LectionTitleTB.Text);
            LectionTitleTB.SelectionStart = start - (len - LectionTitleTB.Text.Length);
        }
    }
}
