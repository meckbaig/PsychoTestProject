using Newtonsoft.Json.Linq;
using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsychoTestProject.View.TestKinds
{
    /// <summary>
    /// Логика взаимодействия для CompletedTestPreview.xaml
    /// </summary>
    public partial class CompletedTestPreview : UserControl, INotifyPropertyChanged
    {

        public ObservableCollection<QuestionPreview> Questions { get; set; }
        private bool hidden = true;
        private double pageMaxHeight;

        public bool Hidden
        {
            get => hidden;
            set 
            { 
                hidden = value; 
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.Duration = TimeSpan.FromMilliseconds(350);
                doubleAnimation.From = ActualHeight;
                doubleAnimation.To = PageMaxHeight;
                BeginAnimation(MaxHeightProperty, doubleAnimation);
            }
        }
    
        public double PageMaxHeight
        {
            get
            {
                if (Hidden) return 40;
                else return pageMaxHeight;
            }
            set
            {
                pageMaxHeight = value;
            }
        }

        public string SHButtonText 
        { 
            get
            {
                if (Hidden)
                    return "Развернуть";
                else
                    return "Свернуть";
            }
        }


        public CompletedTestPreview(TestClass test, int[] answers)
        {
            InitializeComponent();
            DataContext = this;
            Questions = Load(test, answers);
            MainViewModel.MouseHover(btn);
        }

        
        private ObservableCollection<QuestionPreview> Load(TestClass test, int[] answers)
        {
            ObservableCollection<QuestionPreview> questionPreviews = new ObservableCollection<QuestionPreview>();
            foreach (QuestionClass question in test.Questions) 
            {
                bool isCorrect = answers[question.Id - 1] == 0 ? false : true;
                double value = answers[question.Id - 1];
                if (value == 0 )
                    value = 1;
                QuestionPreview questionPreview = new QuestionPreview()
                {
                    Question = question,
                    AnswerId = question.Id - 1,
                    AnswerText = question.Answers.First(a => (a.Value == value || a.Value==0) && a.IsCorrect == isCorrect).Text
                };
                questionPreviews.Add(questionPreview);
            }
            return questionPreviews;
        }

        public class QuestionPreview
        {
            public QuestionClass Question { get; set; }
            public string AnswerText { get; set; }
            public int AnswerId { get; set; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void ShowHideButton_Click(object sender, RoutedEventArgs e)
        {
            Hidden = !Hidden;
            OnPropertyChanged("SHButtonText");
        }
    }
}
