using Microsoft.Web.WebView2.Wpf;
using PsychoTestProject.Extensions;
using PsychoTestProject.Model;
using PsychoTestProject.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PsychoTestProject.ViewModel
{
    internal class LectionsViewModel : INotifyPropertyChanged
    {
        WebView2 webContainer;
        public List<LectionModel> lectionList { get; set; }
        public LectionsViewModel(WebView2 web, ScrollViewer topScroll)
        {
            webContainer = web;
            webContainer.Margin = new Thickness(0, topScroll.ActualHeight, 0, 0);
            lectionList = new List<LectionModel>();
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Lections"), "*.html"))
            {
                lectionList.Add(new LectionModel() { Name = Path.GetFileNameWithoutExtension(file), Url = file });
            }
            webContainer.CoreWebView2.Navigate(lectionList[0].Url);
        }

        Command openLectionCommand;
        public Command OpenLectionCommand
        {
            get
            {
                return openLectionCommand ??
                     (openLectionCommand = new Command(obj =>
                     {
                         webContainer.CoreWebView2.Navigate(obj.ToString());
                     }));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
