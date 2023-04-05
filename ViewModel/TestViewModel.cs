using PsychoTestProject.Extensions;
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
using System.Windows.Media.Imaging;

namespace PsychoTestProject.ViewModel
{
    public class TestViewModel : INotifyPropertyChanged
    {
        public string Picture { get; set; } 
        public Thickness Margin { get; set; }
        public TestViewModel(TestClass test)
        {
            MainViewModel.CurrentTest = test;
            MainViewModel.CurrentQuestionNumber = 1;
            LoadImage();
        }

        private void LoadImage()
        {
            string path;
            for (int i = 0; i < 5; i++)
            {
                path = Path.Combine(Path.GetDirectoryName(MainViewModel.CurrentTest.Filename), Path.GetFileNameWithoutExtension(MainViewModel.CurrentTest.Filename) + $".{(ImageExtension)i}");
                if (File.Exists(path))
                {
                    Picture = path;
                    break;
                }
            }
        }

        public QuestionClass CurrentQuestion
        {
            get => MainViewModel.CurrentTest.Questions[MainViewModel.CurrentQuestionNumber-1];
        }


        public QuestionClass NextQuestion()
        {
            if (MainViewModel.CurrentQuestionNumber == (MainViewModel.CurrentTest.Questions.Count))
            {
                return null;
            }
            return MainViewModel.CurrentTest.Questions[MainViewModel.CurrentQuestionNumber++];
        }

        public void ChangeQuestionMargin(double width, double height)
        {
            Margin = new Thickness(width / 10, height / 15, width / 10, height / 15);
            OnPropertyChanged("Margin");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        //public QuestionClass PreviousQuestion()
        //{
        //    if (idQuestion == 0)
        //    {
        //        return null;
        //    }
        //    return Supporting.CurrentTest.Questions[--idQuestion];
        //}
    }
}
