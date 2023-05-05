using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using static PsychoTestProject.View.WpfMessageBox;

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
        private bool yesNo { get; set; }

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

        public void Initialize(bool showBackButton = false, bool startTimer = true, bool yesNo = false)
        {
            this.yesNo = yesNo;
            this.startTimer = startTimer;
            if (!yesNo) 
                PrintAnswers(QuestionClass);
            else
                PrintAnswersYesNo(QuestionClass);
            if (startTimer)
                Timer(QuestionClass);
            if (!showBackButton)
                ButtonsStackPanel.Children.RemoveAt(0);
        }
        public void Initialize(QuestionClass question, bool startTimer = true, bool yesNo = false)
        {
            this.yesNo = yesNo;
            this.startTimer = startTimer;
            QuestionClass = question;
            if (!yesNo)
                PrintAnswers(QuestionClass);
            else
                PrintAnswersYesNo(QuestionClass);
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
            if (question.YesNo)
            {
                PrintAnswersYesNo(question);
                return;
            }

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
                        textBox.TextWrapping = TextWrapping.Wrap;
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

        public void PrintAnswersYesNo(QuestionClass question)
        {
            QuestionCount = (MainViewModel.CurrentQuestionNumber) + " из " + MainViewModel.CurrentTest.Questions.Count;
            OnPropertyChanged("QuestionCount");
            ShowedAnswers.Children.Clear();
            foreach (var answer in question.Answers)
            {
                StackPanel stackPanel = new StackPanel();
                StackPanel answersStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                switch (question.Type)
                {
                    case QuestionType.Single:
                        answersStackPanel.Children.Add(rb("Да", answer));
                        answersStackPanel.Children.Add(rb("Нет", answer));
                        break;
                    case QuestionType.Multiple:
                        if (!MainViewModel.CurrentTest.Error)
                        {
                            MainViewModel.CurrentTest.Error = true;
                            WpfMessageBox.Show("Внимание!", "Данный тип теста на данный момент не поддерживается.", MessageBoxType.Error);
                        }
                        ExtNextButton_Click?.Invoke(this, new RoutedEventArgs());
                        return;
                    default:
                        if (!MainViewModel.CurrentTest.Error)
                        {
                            MainViewModel.CurrentTest.Error = true;
                            WpfMessageBox.Show("Внимание!", "Данный тип теста на данный момент не поддерживается.", MessageBoxType.Error);
                        }
                        ExtNextButton_Click?.Invoke(this, new RoutedEventArgs());
                        return;
                }
                TextBlock textBlock = new TextBlock();
                textBlock.Text = answer.Text;
                textBlock.FontSize = 16;
                textBlock.FontFamily = new System.Windows.Media.FontFamily("Microsoft YaHei UI");
                textBlock.Margin = new Thickness(5, 3, 0, 3);
                textBlock.TextWrapping = TextWrapping.Wrap;
                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(answersStackPanel);
                ShowedAnswers.Children.Add(stackPanel);

                if (question.Type == QuestionType.String)
                    break;
            }
        }

        private RadioButton rb(string text, AnswerClass answer)
        {
            var radioButton = new RadioButton() 
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                GroupName = text,
                Content = text,
                FontSize = 13,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                Tag = answer,
                Margin = new Thickness(0, 3, 5, 3)
            };
            radioButton.KeyDown += NextKeyDown;
            radioButton.Checked += (s, e) =>
            {
                foreach(RadioButton rb in (radioButton.Parent as StackPanel).Children)
                {
                    if (rb.Content != text)
                        rb.IsChecked = false;
                }
            };
            
            return radioButton;
        }

        public double CheckAnswer()
        {
            if (yesNo)
                return CheckAnswerYesNo();
            double maxPoint = QuestionClass.AnswersTarget;
            double point = 0;
            var answ = ShowedAnswers.Children[0];
            int answersChecked = 0;
            switch (answ)
            {
                case (RadioButton):
                    foreach (var answer in ShowedAnswers.Children)
                    {
                        RadioButton radioButton = (RadioButton)answer;
                        if (((AnswerClass)radioButton.Tag).IsCorrect && (radioButton.IsChecked ?? false))
                            point = MainViewModel.CurrentQuestion.Answers[answersChecked].Value;
                        answersChecked++;
                    }
                    break;
                case (CheckBox):
                    foreach (var answer in ShowedAnswers.Children)
                    {
                        CheckBox checkBox = (CheckBox)answer;
                        if (((AnswerClass)checkBox.Tag).IsCorrect == true)
                        {
                            if (checkBox.IsChecked ?? false)
                                point+= MainViewModel.CurrentQuestion.Answers[answersChecked].Value;
                        }
                        else if (checkBox.IsChecked ?? false)
                            point -= MainViewModel.CurrentQuestion.Answers[answersChecked].Value;
                        answersChecked++;
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
                                        point += MainViewModel.CurrentQuestion.Answers[answersChecked].Value;
                                        answersChecked++;
                                        goto Outer;
                                    }
                                }
                            }
                            else if (!QuestionClass.IsExact && textBox.Text.ToLower().Contains(variableAnswer)) // ищет требуемые ответы в полученном
                            {
                                point += MainViewModel.CurrentQuestion.Answers[answersChecked].Value;
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
            //if (point < 0)
            //    point = 0;
            if (maxPoint != 0)
            {
                if (point > maxPoint)
                    point = maxPoint;
                point /= maxPoint;
                point *= QuestionClass.Value;
            }
            
            return point;
        }

        public double CheckAnswerYesNo(bool positive = true)
        {
            int i = positive ? 0 : 1;
            double maxPoint = QuestionClass.AnswersTarget;
            double point = 0;
            var answ = ((ShowedAnswers.Children[0] as StackPanel).Children[1] as StackPanel).Children[0];
            int answersChecked = 0;

            switch (answ)
            {
                case (RadioButton):
                    foreach (StackPanel answerStack in ShowedAnswers.Children)
                    {
                        RadioButton radioButton = (RadioButton)(answerStack.Children[1] as StackPanel).Children[i];
                        if (((AnswerClass)radioButton.Tag).IsCorrect && (radioButton.IsChecked ?? false))
                            point = MainViewModel.CurrentQuestion.Answers[answersChecked].Value;
                        answersChecked++;
                    }
                    break;
                case (CheckBox):
                    if (!MainViewModel.CurrentTest.Error)
                    {
                        MainViewModel.CurrentTest.Error = true;
                        WpfMessageBox.Show("Внимание!", "Данный тип теста на данный момент не поддерживается.", MessageBoxType.Error);
                    }
                    return 0;
                case (TextBox):
                    if (!MainViewModel.CurrentTest.Error)
                    {
                        MainViewModel.CurrentTest.Error = true;
                        WpfMessageBox.Show("Внимание!", "Данный тип теста на данный момент не поддерживается.", MessageBoxType.Error);
                    }
                    return 0;

            }
            //if (point < 0)
            //    point = 0;
            if (maxPoint != 0)
            {
                if (point > maxPoint)
                    point = maxPoint;
                point /= maxPoint;
                point *= QuestionClass.Value;
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
