﻿using Microsoft.Web.WebView2.Wpf;
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
            webContainer.CoreWebView2.Navigate(LectionList[0].Url);
            Lections.LectionSource = LectionList[0].Url;
            LectionTitle = Path.GetFileNameWithoutExtension(LectionList[0].Url);
        }

        public void UpdateLectionList()
        {
            LectionList = new ObservableCollection<LectionModel>();
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Lections"), "*.html"))
            {
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
                         webContainer.CoreWebView2.Navigate(obj.ToString());
                         Lections.LectionSource = obj.ToString();
                         LectionTitle = Path.GetFileNameWithoutExtension(obj.ToString());
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
