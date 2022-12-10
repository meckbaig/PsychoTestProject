using PsychoTestCourseProject.Extensions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
                    QuestionTime = 60;
                    break;
                case QuestionType.String:
                    QuestionTime = 90;
                    break;
            }
            MaxQuestionTime = QuestionTime;
            OnPropertyChanged("QuestionTime");
            qTimer.Start();
        }

        public void PrintAnswers(QuestionClass question)
        {
            QuestionCount = (Supporting.CurrentQuestion + 1) + " из " + Supporting.CurrentTest.Questions.Count;
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

        public double CheckAnswer()
        {
            double maxPoint = 0;
            double point = 0;
            var answ = ShowedAnswers.Children[0];
            switch (answ)
            {
                case (RadioButton):
                    foreach (var answer in ShowedAnswers.Children)
                    {
                        RadioButton radioButton = (RadioButton)answer;
                        if (((AnswerClass)radioButton.Tag).IsCorrect == (radioButton.IsChecked ?? false))
                            point = 1;
                    }
                    break;
                case (CheckBox):
                    foreach (var answer in ShowedAnswers.Children)
                    {
                        CheckBox checkBox = (CheckBox)answer;
                        if (((AnswerClass)checkBox.Tag).IsCorrect == true)
                        {
                            maxPoint++;
                            if (checkBox.IsChecked ?? false)
                                point++;
                        }
                    }
                    point /= maxPoint;
                    break;
                case (TextBox):
                    TextBox textBox = (TextBox)answ;
                    var correctAnswers = ((AnswerClass)textBox.Tag).Text.Split(", ");
                    var answers = textBox.Text.ToLower().Split(", ");
                    maxPoint = correctAnswers.Length;
                    foreach (var correctAnswer in correctAnswers)
                    {
                        foreach (var variableAnswer in correctAnswer.Split("(/)"))
                        {
                            foreach (var answer in answers)
                            {
                                if (variableAnswer.Equals(answer))
                                {
                                    point++;
                                    goto Outer;
                                }
                            }
                        }
                    Outer:;
                    }
                    point /= maxPoint;
                    break;
            }
            return point;
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
