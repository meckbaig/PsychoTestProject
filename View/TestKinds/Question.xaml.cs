using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PsychoTestProject.View
{
    /// <summary>
    /// Логика взаимодействия для Question.xaml
    /// </summary>
    public partial class Question : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty QuestionProperty = DependencyProperty.Register("QuestionClass", typeof(QuestionClass), typeof(Question));

        public static System.Windows.Threading.DispatcherTimer qTimer { get; set; }
        public string TimerColor { get; set; }
        public int MaxQuestionTime { get; set; }
        public string QuestionTime { get; set; }
        private int questionTime { get; set; }
        public string QuestionCount { get; set; }
        public string QuestionText { get => QuestionClass?.Text; }
        private bool startTimer { get; set; }

        public Question()
        {
            InitializeComponent();
            MainViewModel.MouseHover(NextButton);
            MainViewModel.MouseHover(BackButton);
            DataContext = this;
            TimerColor = "Green";
            qTimer = new();
            qTimer.Interval = new TimeSpan(0, 0, 1);
            qTimer.Tick += Timer_Tick;
        }

        public void Initialize(bool startTimer = true, bool showBackButton = false)
        {
            this.startTimer = startTimer;
            PrintAnswers(QuestionClass);
            if (startTimer)
                Timer(QuestionClass);
            if (!showBackButton)
                ButtonsStackPanel.Children.RemoveAt(0);
        }
        public void Initialize(QuestionClass question, bool startTimer = true)
        {
            this.startTimer = startTimer;
            QuestionClass = question;
            PrintAnswers(QuestionClass);
            if (startTimer)
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
            if (questionTime == 0)
            {
                qTimer.Stop();
                TimeOut?.Invoke(this, EventArgs.Empty);
            }
            questionTime--;
            TimerColorChange();
            QuestionTime = $"Осталось: {questionTime}с.";
            OnPropertyChanged("QuestionTime");
        }

        private void TimerColorChange()
        {
            double percentage = (double)questionTime / (double)MaxQuestionTime;
            if (percentage <= 0.2)
                TimerColor = "Red";
            else if (percentage <= 0.4)
                TimerColor = "Orange";
            else
                TimerColor = "Green";
            OnPropertyChanged("TimerColor");
        }

        public void Timer(QuestionClass question)
        {
            switch (question.Type)
            {
                case QuestionType.Single:
                    questionTime = 30;
                    break;
                case QuestionType.Multiple:
                    questionTime = 60;
                    break;
                case QuestionType.String:
                    questionTime = 90;
                    break;
            }
            MaxQuestionTime = questionTime;
            QuestionTime = $"Осталось: {questionTime}с.";
            OnPropertyChanged("QuestionTime");
            qTimer.Start();
        }

        public void PrintAnswers(QuestionClass question)
        {
            QuestionCount = (MainViewModel.CurrentQuestionNumber) + " из " + MainViewModel.CurrentTest.Questions.Count;
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
                if (question.Type == QuestionType.String)
                    break;
            }
        }

        public double CheckAnswer()
        {
            double maxPoint = QuestionClass.AnswersTarget;
            double point = 0;
            var answ = ShowedAnswers.Children[0];
            switch (answ)
            {
                case (RadioButton):
                    foreach (var answer in ShowedAnswers.Children)
                    {
                        RadioButton radioButton = (RadioButton)answer;
                        if (((AnswerClass)radioButton.Tag).IsCorrect && (radioButton.IsChecked ?? false))
                            point = 1;
                    }
                    break;
                case (CheckBox):
                    foreach (var answer in ShowedAnswers.Children)
                    {
                        CheckBox checkBox = (CheckBox)answer;
                        if (((AnswerClass)checkBox.Tag).IsCorrect == true)
                        {
                            if (checkBox.IsChecked ?? false)
                                point++;
                        }
                        else if (checkBox.IsChecked ?? false)
                            point--;
                    }
                    break;
                case (TextBox):
                    TextBox textBox = (TextBox)answ;
                    int correctAnswersCount = MainViewModel.CurrentQuestion.Answers.Count;
                    string[] correctAnswers = new string[correctAnswersCount];
                    for (int i = 0; i < correctAnswersCount; i++)
                    {
                        correctAnswers[i] = MainViewModel.CurrentQuestion.Answers[i].Text;
                    }
                    var answers = textBox.Text.ToLower().Split(", ");
                    int answersChecked = 0;
                    foreach (var correctAnswer in correctAnswers) // крутит все варианты ответов
                    {
                        foreach (var variableAnswer in correctAnswer.Split("(/)")) // крутит все варианты одного ответа
                        {
                            if (QuestionClass.IsExact)
                            {
                                foreach (var answer in answers) // крутит все ответы под текущий вариант
                                {
                                    if (variableAnswer.Equals(answer)) // сравнивает требуемые ответы с полученными
                                    {
                                        point++;
                                        answersChecked++;
                                        goto Outer;
                                    }
                                }
                            }
                            else if (!QuestionClass.IsExact && textBox.Text.ToLower().Contains(variableAnswer)) // ищет требуемые ответы в полученном
                            {
                                point++;
                                answersChecked++;
                                goto Outer;
                            }
                        }
                        //point--;
                        answersChecked++;
                    Outer:;
                    }
                    if (answers.Length >= answersChecked)
                        point -= (answers.Length - answersChecked);
                    break;
            }
            if (point < 0)
                point = 0;
            if (point > maxPoint)
                point = maxPoint;
            point /= maxPoint;
            point *= QuestionClass.Value;
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
            if (startTimer)
                TimerColorChange();
        }
        public event EventHandler ExtBackButton_Click;
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ExtBackButton_Click?.Invoke(sender, e);
            if (startTimer)
                TimerColorChange();
        }

        private void NextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ExtNextButton_Click?.Invoke(sender, e);
        }

    }
}
