using PsychoTestCourseProject.Extensions;
using PsychoTestCourseProject.Model;
using PsychoTestCourseProject.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PsychoTestCourseProject.ViewModel
{
    internal class LectionsViewModel : INotifyPropertyChanged
    {
        WebBrowser webContainer;
        public List<LectionModel> lectionList { get; set; }
        public LectionsViewModel(WebBrowser web)
        {
            webContainer = web;
            lectionList = new List<LectionModel>();
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Lections"), "*.html"))
            {
                lectionList.Add(new LectionModel() { Name = Path.GetFileNameWithoutExtension(file), Url = file });
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
                         webContainer.Navigate(new Uri(obj.ToString()));
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
