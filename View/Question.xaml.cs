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

        public string QuestionText { get => QuestionClass?.Text; }

        public Question()
        {
            InitializeComponent();
            DataContext = this;
        }
        public void Initialize()
        {
            ShowAnswers(QuestionClass);
        }
        public void Initialize(QuestionClass question)
        {
            QuestionClass = question;
            ShowAnswers(question);
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

        public void ShowAnswers(QuestionClass question)
        {
            ShowedAnswers.Children.Clear();
            foreach (var answer in question.Answers)
            {
                Control item;
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
                item.Tag = answer;
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
    }
}
