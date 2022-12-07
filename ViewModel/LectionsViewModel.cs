using PsychoTestCourseProject.Extensions;
using PsychoTestCourseProject.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PsychoTestCourseProject.ViewModel
{
    internal class LectionsViewModel
    {
        WebBrowser webContainer;
        public List<LectionModel> lectionList { get; set; }
        public LectionsViewModel(WebBrowser web)
        {
            webContainer = web;
            lectionList = new List<LectionModel>();
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Lections")))
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
    }
}
