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
using PsychoTestProject.View;

namespace PsychoTestProject.ViewModel
{
    abstract class MultiTestViewModel : INotifyPropertyChanged
    {
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

        public void ChangeMargin(double width, double height, Question question)
        {
            Margin = new Thickness(0, height / 15, 0, height / 15);
            question.Width = (height < width * 0.8) ? height : width * 0.8;
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
