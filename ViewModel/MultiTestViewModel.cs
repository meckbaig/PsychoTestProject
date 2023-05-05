using PsychoTestProject.Extensions;
using PsychoTestProject.View.TestKinds;
using ScottPlot.Plottable;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace PsychoTestProject.ViewModel
{
    abstract class MultiTestViewModel : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public int[] AnswersArray { get; set; }
        public int[] NegativeAnswersArray { get; set; }
        public Thickness Margin { get; set; }
        public Visibility BackButtonVisibility { get; set; } = Visibility.Visible;


        public QuestionClass CurrentQuestion
        {
            get => MainViewModel.CurrentTest?.Questions?[MainViewModel.CurrentQuestionNumber - 1];
        }

        public QuestionClass NextQuestion()
        {
            if (MainViewModel.CurrentQuestionNumber == (MainViewModel.CurrentTest.Questions.Count))
            {
                return null;
            }
            return MainViewModel.CurrentTest.Questions[MainViewModel.CurrentQuestionNumber++];
        }
        public QuestionClass PreviousQuestion()
        {
            if (MainViewModel.CurrentQuestionNumber == 1)
            {
                return null;
            }
            return MainViewModel.CurrentTest.Questions[--MainViewModel.CurrentQuestionNumber-1];
        }

        public void ChangeMargin(double width, double height)
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

        abstract public void PrintResults(Grid thisGrid, ScrollViewer scroll);
    }
}
