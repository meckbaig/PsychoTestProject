using Microsoft.Web.WebView2.Wpf;
using PsychoTestProject.Extensions;
using PsychoTestProject.Model;
using PsychoTestProject.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private  ObservableCollection<LectionModel> lectionList;
        private string lectionTitle;
        public ObservableCollection<LectionModel> LectionList
        {
            get => lectionList;
            set
            {
                lectionList = value;
                OnPropertyChanged("LectionList");
            }
        }
        public string LectionTitle 
        {
            get => lectionTitle;
            set
            {
                lectionTitle = value;
                OnPropertyChanged("LectionTitle");
            }
        }
        public LectionsViewModel(WebView2 web, ScrollViewer topScroll)
        {
            webContainer = web;
            webContainer.Margin = new Thickness(0, topScroll.ActualHeight, 0, 0);
            UpdateLectionList();
            OpenLectionCommand.Execute(LectionList[0].Url);
        }

        public void UpdateLectionList()
        {
            LectionList = new ObservableCollection<LectionModel>();
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Lections"), "*.html"))
            {
                if (Path.GetFileNameWithoutExtension(file)!="temp")
                    LectionList.Add(new LectionModel() { Name = Path.GetFileNameWithoutExtension(file), Url = file });
            }
        }

        Command openLectionCommand;
        public Command OpenLectionCommand
        {
            get
            {
                return openLectionCommand ??
                     (openLectionCommand = new Command(obj =>
                     {
                         Lections.LectionSource = obj.ToString();
                         LectionTitle = Path.GetFileNameWithoutExtension(obj.ToString());
                         webContainer.CoreWebView2.Navigate(obj.ToString());
                         if (Lections.Admin)
                         {
                             //string lection = File.ReadAllText(obj.ToString());
                             //lection = lection.Replace("</style>", "</style><script src=\"https://cdn.ckeditor.com/4.15.1/standard-all/ckeditor.js\"></script>");
                             //File.WriteAllText($"{Path.Combine(Environment.CurrentDirectory, "Lections")}\\temp.html", lection);
                             //webContainer.CoreWebView2.Navigate($"{Path.Combine(Environment.CurrentDirectory, "Lections")}\\temp.html");
                             webContainer.ExecuteScriptAsync("document.designMode = \"on\"");
                         }
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
