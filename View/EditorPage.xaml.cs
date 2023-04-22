using Microsoft.Win32;
using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PsychoTestProject.View
{
    /// <summary>
    /// Логика взаимодействия для EditorPage.xaml
    /// </summary>
    public partial class EditorPage : Page, INotifyPropertyChanged, IEditorView
    {
        EditorPageControls EditorControls;
        #region Variables
        private List<AnswerClass> answerList;
        private ObservableCollection<QuestionClass> questionList;
        private AnswerClass answer;
        private QuestionClass question = new QuestionClass();
        private string testTitle;
        private int answersTarget;
        private Image backgroundImage;

        public Thickness DeleteButtonThickness { get; set; }
        public List<AnswerClass> AnswerList
        {
            get => answerList;
            set
            {
                answerList = value;
                OnPropertyChanged("AnswerList");
            }
        }
        public ObservableCollection<QuestionClass> QuestionList
        {
            get => questionList;
            set
            {
                questionList = value;
                OnPropertyChanged("QuestionList");
                TestTitle = MainViewModel.CurrentTest.Name;
            }
        }
        public AnswerClass Answer
        {
            get => answer;
            set
            {
                answer = value;
                OnPropertyChanged("Answer");
            }
        }
        public QuestionClass Question
        {
            get => question;
            set
            {
                for (int i = Question.Answers.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(Question.Answers[i].Text))
                        Question.Answers.RemoveAt(i);
                }
                
                if (value!=null)
                    question = value;
                OnPropertyChanged("Question");
                RecalculateValues();
            }
        }
        public string TestTitle
        {
            get => testTitle;
            set
            {
                testTitle = value;
                OnPropertyChanged("TestTitle");
            }
        }
        public int AnswersTarget
        {
            get => answersTarget;
            set
            {
                int correctAnswers = CorrectAnsvers();
                if (value > correctAnswers)
                    answersTarget = correctAnswers;
                else
                    answersTarget = value;
                OnPropertyChanged("AnswersTarget");
            }
        }
        public Image BackgroundImage
        {
            get => backgroundImage;
            set
            {
                backgroundImage = value;
                OnPropertyChanged("BackgroundImage");
            }
        }
        #endregion

        public EditorPage()
        {
            InitializeComponent();

            foreach (DockPanel dockPanel in MainViewModel.GetVisualChilds<DockPanel>(this.Content as DependencyObject))
            {
                foreach (ScrollViewer scrollViewer in MainViewModel.GetVisualChilds<ScrollViewer>(dockPanel as DependencyObject))
                {
                    foreach (StackPanel stackPanel in (scrollViewer.Content as StackPanel).Children)
                    {
                        foreach (Button button in MainViewModel.GetVisualChilds<Button>(stackPanel))
                        {
                            if (button.Background.GetType() != (new ImageBrush()).GetType())
                            {
                                MainViewModel.MouseHover(button);
                            }
                        }
                    }

                }
            }

            EditorControls = new EditorPageControls(this);
            QuestionList = MainViewModel.CurrentTest.Questions;
            this.Question = QuestionList.ToList()[0];
            QuestionTitle.Text = Question.Text;
            LoadImage();
            TakeUpDown.Value = MainViewModel.CurrentTest.Take;
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void PrevQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            Question = QuestionList[Question.Id-2];
        }
        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            Question = QuestionList[Question.Id];
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = MainViewModel.CurrentTest.Questions.Count - 1; i >= 0; i--)
            {
                if (MainViewModel.CurrentTest.Questions[i].Text == "")
                    MainViewModel.CurrentTest.Questions.RemoveAt(i);
            }
            QuestionClass savedQuestion = Question;
            QuestionClass editedQuestion = ReadQuestion();
            if (!EditorControls.Equal(savedQuestion, editedQuestion))
                switch (MessageBox.Show("Вопрос был изменён. Сохранить?", "Внимание!", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                {
                    case MessageBoxResult.Yes:
                        SaveQuestion(); AddQuestion(); break;
                    case MessageBoxResult.No:
                        AddQuestion(); break;
                    default: break;
                }
            else
                AddQuestion();
        }

        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            this.Question = ReadQuestion();
            AnswerClass newAnswer = new AnswerClass();
            newAnswer.Id = this.Question.Answers.Count + 1;
            this.Question.Answers.Add(newAnswer);
            LoadQuestion();
        }

        private void SaveQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            SaveQuestion();
        }

        private void DeleteQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteQuestion();
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            AddImage();
        }

        private void AddImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображение|";
            for (int i = 0; i < 5; i++)
            {
                openFileDialog.Filter += $"*.{(ImageExtension)i};";
            }
            openFileDialog.Filter = openFileDialog.Filter.Remove(openFileDialog.Filter.Length - 1);
            if (openFileDialog.ShowDialog() == true)
            {
                string savepath = Environment.CurrentDirectory + "\\Tests\\" + MainViewModel.CurrentTest.Name + $"{Path.GetExtension(openFileDialog.FileName)}";
                File.WriteAllBytes(savepath, File.ReadAllBytes(openFileDialog.FileName));
                BackgroundImage = new Image();
                BackgroundImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                BackgroundImage.Visibility = Visibility.Visible;
            }
        }

        private void LoadImage()
        {
            BackgroundImage = new Image();
            string path;
            if (MainViewModel.CurrentTest.Filename != null)
            {
                for (int i = 0; i < Enum.GetNames(typeof(ImageExtension)).Length; i++)
                {
                    path = Path.Combine(Path.GetDirectoryName(MainViewModel.CurrentTest.Filename), Path.GetFileNameWithoutExtension(MainViewModel.CurrentTest.Filename) + $".{(ImageExtension)i}");
                    if (File.Exists(path))
                    {
                        BackgroundImage.Source = new BitmapImage(new Uri(path));
                        BackgroundImage.Visibility = Visibility.Visible;
                        break;
                    }
                    else
                        BackgroundImage.Visibility = Visibility.Hidden;
                }
            }
            else
                BackgroundImage.Visibility = Visibility.Hidden;
        }

        private int CorrectAnsvers()
        {
            int correctAnswers = 0;
            if (Question.Type == QuestionType.String)
                foreach (StackPanel answerStackPanel in ShowedAnswers.Children)
                {
                    correctAnswers++;
                }
            else
            {
                int childrenCount = 0;
                foreach (TextBox answer in ShowedAnswers.Children)
                {
                    switch (Question.Type)
                    {
                        case QuestionType.Single:
                            if ((bool)((RadioButton)ShowedFlags.Children[childrenCount]).IsChecked)
                                correctAnswers++;
                            break;
                        case QuestionType.Multiple:
                            if ((bool)((CheckBox)ShowedFlags.Children[childrenCount]).IsChecked)
                                correctAnswers++;
                            break;
                        default:
                            break;
                    }
                    childrenCount++;
                }
            }
            return correctAnswers;
        }

        public QuestionClass ReadQuestion()
        {
            QuestionClass newQuestion = new QuestionClass();
            newQuestion.Type = Question.Type;
            newQuestion.Value = (int)QuestionValueUpDown.Value;
            if (AnswersTarget>0)
                newQuestion.AnswersTarget = AnswersTarget;
            else
                newQuestion.AnswersTarget = 1;
            newQuestion.Id = Question.Id;
            newQuestion.Text = QuestionTitle.Text;

            int answerCount = 1;
            if (newQuestion.Type == QuestionType.String)
            {
                newQuestion.IsExact = (bool)IsExactCheckBox.IsChecked;
                foreach (StackPanel answerStackPanel in ShowedAnswers.Children)
                {
                    string answerString = "";
                    foreach (TextBox answer in answerStackPanel.Children)
                    {
                        if (!string.IsNullOrEmpty(answer.Text))
                        {
                            if (!string.IsNullOrEmpty(answerString))
                                answerString += "(/)" + answer.Text;
                            else
                                answerString = answer.Text;
                        }
                    }
                    AnswerClass newAnswer = new AnswerClass();
                    newAnswer.Id = answerCount++;
                    newAnswer.Text = answerString;
                    newAnswer.IsCorrect = true;
                    newQuestion.Answers.Add(newAnswer);
                }
            }
            else
            {
                int childrenCount = 0;
                foreach (TextBox answer in ShowedAnswers.Children)
                {
                    AnswerClass newAnswer = new AnswerClass();
                    newAnswer.Id = answerCount++;
                    newAnswer.Text = answer.Text;
                    switch (newQuestion.Type)
                    {
                        case QuestionType.Single:
                            newAnswer.IsCorrect = (bool)((RadioButton)ShowedFlags.Children[childrenCount]).IsChecked;
                            break;
                        case QuestionType.Multiple:
                            newAnswer.IsCorrect = (bool)((CheckBox)ShowedFlags.Children[childrenCount]).IsChecked;
                            break;
                        default:
                            break;
                    }
                    newQuestion.Answers.Add(newAnswer);
                    childrenCount++;
                }
            }

            return newQuestion;
        }

        public void SaveQuestion()
        {
            MainViewModel.CurrentTest.Name = TestTitleTB.Text;
            if ((int)TakeUpDown.Value>0)
                MainViewModel.CurrentTest.Take = (int)TakeUpDown.Value;
            else
                MainViewModel.CurrentTest.Take = MainViewModel.CurrentTest.Questions.Count;
            QuestionClass newQuestion = ReadQuestion();
            this.Question = MainViewModel.CurrentTest.Questions[newQuestion.Id - 1] = newQuestion;
        }

        private void AddQuestion()
        {
            QuestionClass newQuestion = new QuestionClass();
            newQuestion.Type = (QuestionType)NewQuestionType.SelectedIndex;
            newQuestion.Id = MainViewModel.CurrentTest.Questions.Count + 1;
            MainViewModel.CurrentTest.Questions.Add(newQuestion);
            QuestionList = MainViewModel.CurrentTest.Questions;
            this.Question = QuestionList.ToList()[QuestionList.Count - 1];
            LoadQuestion();
        }

        private void DeleteQuestion()
        {
            int deletedQuestionId = Question.Id;
            QuestionList.Remove(Question);
            List<QuestionClass> newQuestionList = QuestionList.ToList();
            for (int i = deletedQuestionId - 1; i < newQuestionList.Count; i++)
            {
                newQuestionList[i].Id--;
            }
            QuestionList = new ObservableCollection<QuestionClass>(newQuestionList);
            if (deletedQuestionId > 1)
                Question = QuestionList[deletedQuestionId - 2];
            else
                Question = QuestionList[0];
        }

        private void LoadQuestion()
        {
            ShowedFlags.Children.Clear();
            ShowedAnswers.Children.Clear();
            AnswerButtons.Children.Clear();
            TestSettingsStackPanel.Children.Remove(IsExactCheckBox);

            AnswerList = this.Question.Answers;
            QuestionValueUpDown.Value = Question.Value;
            MovementButtonsAvailability();

            if (Question.Type == QuestionType.String)
            {
                IsExactCheckBox = EditorControls.IsExactCheckBox(Question);
                TestSettingsStackPanel.Children.Add(IsExactCheckBox);

                if (Question.Answers.Count > 0)
                {
                    int answerCount = 1;
                    foreach (var answer in Question.Answers) // строки с ответами
                    {
                        TextBlock textBlock = EditorControls.TitleTextBlock(answerCount);
                        ShowedFlags.Children.Add(textBlock);

                        var variableAnswer = answer.Text.Split("(/)"); // строка с вариациями одного ответа
                        var stackPanel = EditorControls.AnswerStackPanel(answerCount);
                        ShowedAnswers.Children.Add(stackPanel);
                        foreach (string word in variableAnswer) // вариации одного ответа
                        {
                            stackPanel.Children.Add(EditorControls.VariableAnswer(answerCount, word));
                        }
                        DeleteButtonThickness = new Thickness(5, 2.35, 5, 2);

                        AnswerButtons.Children.Add(EditorControls.AnsverControlButtons(answer, true));
                        answerCount++;
                    }
                }
            }
            else
            {
                int answerCount = 1;
                foreach (var answer in question.Answers)
                {
                    Control optionsAnswer = EditorControls.OptionsAnswer(question.Type, answer);
                    Control answerText = EditorControls.OptionsAnswerTextBox(answerCount, answer);
                    StackPanel answerControlButtons = EditorControls.AnsverControlButtons(answer);

                    ShowedFlags.Children.Add(optionsAnswer);
                    ShowedAnswers.Children.Add(answerText);
                    AnswerButtons.Children.Add(answerControlButtons);
                    answerCount++;
                }
            }
            AnswersTarget = Question.AnswersTarget;

            foreach (StackPanel stack in MainViewModel.GetVisualChilds<StackPanel>(ContentViewer.Content as StackPanel))
            {
                MainViewModel.AllButtonsHover(stack);
            }
        }

        private void MovementButtonsAvailability()
        {
            if (Question.Id == 1)
                PrevQuestionButton.IsEnabled = false;
            else 
                PrevQuestionButton.IsEnabled = true;
            if (Question.Id == QuestionList.Count)
                NextQuestionButton.IsEnabled = false;
            else 
                NextQuestionButton.IsEnabled = true;
        }

        public void RecalculateValues()
        {
            QuestionTitle.Text = Question.Text;
            LoadQuestion();
        }

        #region Commands
        Command deleteAnswerCommand;
        public Command DeleteAnswerCommand
        {
            get
            {
                return deleteAnswerCommand ??
                     (deleteAnswerCommand = new Command(obj =>
                     {
                         SaveQuestion();
                         Question.Answers.Remove(Question.Answers.First(a => a.Id == (obj as AnswerClass).Id));
                         LoadQuestion();
                     }));
            }
        }

        Command addVariableAnswerCommand;
        public Command AddVariableAnswerCommand
        {
            get
            {
                return addVariableAnswerCommand ??
                     (addVariableAnswerCommand = new Command(obj =>
                     {
                         SaveQuestion();
                         obj = Question.Answers.FirstOrDefault(a => a.Id == (obj as AnswerClass).Id);
                         (obj as AnswerClass).Text += "(/)";
                         LoadQuestion();
                     }));
            }
        }
        #endregion

        #region Margins
        private void BottomStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeMargin();
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeMargin();
        }

        private void ChangeMargin()
        {
            ContentViewer.Margin = new Thickness(0, TopStackPanelScroll.ActualHeight + 5, 0, BottomStackPanelScroll.ActualHeight);
            StackPanel sp = new StackPanel();
            foreach (var elem in ShowedAnswers.Children)
            {
                if (elem.GetType() == sp.GetType())
                {
                    (elem as StackPanel).MaxWidth = ContentViewer.ActualWidth - 260;
                    foreach (TextBox item in (elem as StackPanel).Children)
                    {
                         item.MaxWidth = ((elem as StackPanel).MaxWidth-(elem as StackPanel).Children.Count*10)/(elem as StackPanel).Children.Count;
                    }
                }
                else
                    (elem as Control).MaxWidth = ContentViewer.ActualWidth - 200;
            }
        }
        #endregion

    }
}
