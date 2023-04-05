using PsychoTestProject.Extensions;
using PsychoTestProject.View.TestKinds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PsychoTestProject.ViewModel
{
    class AizenkTestViewModel : INotifyPropertyChanged
    {
        public int[] AnswersArray { get; set; }
        public int[] indexAPos = new int[] { 6, 24, 36 };
        public int[] indexANeg = new int[] { 12, 18, 30, 42, 48, 54 };
        public int[] indexBPos = new int[] { 1, 3, 8, 10, 13, 17, 22, 25, 27, 39, 44, 46, 49, 53, 56 };
        public int[] indexBNeg = new int[] { 5, 15, 20, 29, 32, 34, 37, 41, 51 };
        public int[] indexCPos = new int[] { 2, 4, 7, 9, 11, 14, 16, 19, 21, 23, 26, 28, 31, 33, 35, 38, 40, 43, 45, 47, 50, 52, 55, 57 };

        public AizenkTestViewModel()
        {
            MainViewModel.CurrentQuestionNumber = 1;
        }
        public Thickness Margin { get; set; }


        public QuestionClass CurrentQuestion
        {
            get => MainViewModel.CurrentTest.Questions[MainViewModel.CurrentQuestionNumber - 1];
        }


        public QuestionClass NextQuestion()
        {
            if (MainViewModel.CurrentQuestionNumber == (MainViewModel.CurrentTest.Questions.Count))
            {
                return null;
            }
            return MainViewModel.CurrentTest.Questions[MainViewModel.CurrentQuestionNumber++];
        }

        public int CalculateIndicator(int[] posIndicators, int[] negIndicators = null)
        {
            int result = 0;
            foreach (int i in posIndicators)
            {
                if (AnswersArray[i-1] == 1)
                    result++;
            }
            if (negIndicators != null)
            {
                foreach (int i in negIndicators)
                {
                    if (AnswersArray[i-1] == 1)
                        result--;
                }
            }
            if (result < 0) return 0;
            return result;
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
    }
}
