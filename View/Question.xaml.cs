using PsychoTestCourseProject.Extensions;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsychoTestCourseProject.View
{
    /// <summary>
    /// Логика взаимодействия для Question.xaml
    /// </summary>
    public partial class Question : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty QuestionProperty = DependencyProperty.Register("QuestionClass", typeof(QuestionClass), typeof(Question));

        public System.Windows.Threading.DispatcherTimer qTimer;
        public string TimerColor { get; set; }
        public int MaxQuestionTime { get; set; }
        public int QuestionTime { get; set; }
        public string QuestionCount { get; set; }
        public string QuestionText { get => QuestionClass?.Text; }

        public Question()
        {
            InitializeComponent();
            DataContext = this;
            TimerColor = "Green";
            qTimer = new();
            qTimer.Interval = new TimeSpan(0, 0, 1);
            qTimer.Tick += Timer_Tick;
        }
        public void Initialize()
        {
            PrintAnswers(QuestionClass);
            Timer(QuestionClass);
        }
        public void Initialize(QuestionClass question)
        {
            QuestionClass = question;
            PrintAnswers(QuestionClass);
            Timer(QuestionClass);
        }
        public QuestionClass QuestionClass 
        { 
            get { return (QuestionClass)GetValue(QuestionProperty); }
            set 
            { 
                SetValue(QuestionProperty, value); 
                OnPropertyChanged("QuestionText");
            }
        }

        public event EventHandler TimeOut;

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (QuestionTime == 0)
            {
                qTimer.Stop();
                TimeOut?.Invoke(this, EventArgs.Empty);
            }
            QuestionTime--;
            TimerColorChange();
            OnPropertyChanged("QuestionTime");
        }

        private void TimerColorChange()
        {
            double percentage = (double)QuestionTime / (double)MaxQuestionTime;
            if (percentage <= 0.2)
            {
                TimerColor = "Red";
                OnPropertyChanged("TimerColor");
            }
            else if (percentage <= 0.4)
            {
                TimerColor = "Orange";
                OnPropertyChanged("TimerColor");
            }
        }

        public void Timer(QuestionClass question)
        {
            switch (question.Type)
            {
                case QuestionType.Single:
                    QuestionTime = 30;
                    break;
                case QuestionType.Multiple:
                    QuestionTime = 45;
                    break;
                case QuestionType.String:
                    QuestionTime = 60;
                    break;
            }
            MaxQuestionTime = QuestionTime;
            OnPropertyChanged("QuestionTime");
            qTimer.Start();
        }

        public void PrintAnswers(QuestionClass question)
        {
            QuestionCount = (Supporting.CurrentQuestion+1)+" из "+Supporting.CurrentTest.Questions.Count;
            OnPropertyChanged("QuestionCount");
            ShowedAnswers.Children.Clear();
            foreach (var answer in question.Answers)
            {
                Control item;
                FontFamily fontFamily = new FontFamily("Microsoft YaHei UI");
                Thickness margin = new Thickness(0, 3, 0, 3);
                switch (question.Type)
                {
                    case QuestionType.Single:
                        var radioButton = new RadioButton();
                        radioButton.Content = answer.Text;
                        item = radioButton;
                        break;
                    case QuestionType.Multiple:
                        var checkBox = new CheckBox();
                        item = checkBox;
                        checkBox.Content = answer.Text;
                        break;
                    default:
                        var textBox = new TextBox();
                        item = textBox;
                        break;
                }
                item.FontSize = 15;
                item.FontFamily = fontFamily;
                item.Tag = answer;
                item.Margin = margin;
                item.KeyDown += NextKeyDown;
                ShowedAnswers.Children.Add(item);
            }
        }

        public bool CheckAnswer()
        {
            bool сorrect = true;
            foreach (var answer in ShowedAnswers.Children)
            {
                if (answer is RadioButton radioButton)
                {
                    сorrect = ((AnswerClass)radioButton.Tag).IsCorrect == (radioButton.IsChecked ?? false);
                }
                else if (answer is CheckBox checkBox)
                {
                    сorrect = ((AnswerClass)checkBox.Tag).IsCorrect == (checkBox.IsChecked ?? false);
                }
                else if (answer is TextBox textBox)
                {
                    сorrect = ((AnswerClass)textBox.Tag).Text == textBox.Text;
                }
                if (!сorrect) return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public event EventHandler ExtNextButton_Click;
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ExtNextButton_Click?.Invoke(sender, e);
        }

        private void NextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ExtNextButton_Click?.Invoke(sender, e);
        }
    }
}
