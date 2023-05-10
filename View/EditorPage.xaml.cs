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
using Xceed.Wpf.Toolkit;

namespace PsychoTestProject.View
{
    /// <summary>
    /// Логика взаимодействия для EditorPage.xaml
    /// </summary>
    public partial class EditorPage : Page, INotifyPropertyChanged, IEditorView
    {
        EditorPageControls EditorControls;
        #region Variables
        private bool autoSave = true;
        private List<AnswerClass> answerList;
        private ObservableCollection<QuestionClass> questionList;
        private AnswerClass answer;
        private QuestionClass question = new QuestionClass();
        private string testTitle;
        private int answersTarget;
        private Image backgroundImage;
        public byte[] Image { get; 
            set; }
        public string ImageExt { get; 
            set; }

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
        public QuestionClass CurrentQuestion
        {
            get => question;
            set
            {
                int id = question.Id;
                for (int i = CurrentQuestion.Answers.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(CurrentQuestion.Answers[i].Text))
                        CurrentQuestion.Answers.RemoveAt(i);
                }

                if (autoSave && question.Answers.Count != 0 && value != null && !EditorControls.Equal(question, value))
                    SaveQuestion();
                
                if (value != null)
                    question = value;
                OnPropertyChanged(nameof(CurrentQuestion));
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
            this.CurrentQuestion = QuestionList.ToList()[0];
            QuestionTitle.Text = CurrentQuestion.Text;
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
            CurrentQuestion = QuestionList[CurrentQuestion.Id-2];
        }
        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentQuestion = QuestionList[CurrentQuestion.Id];
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            SaveQuestion();
            for (int i = MainViewModel.CurrentTest.Questions.Count - 1; i >= 0; i--)
            {
                if (MainViewModel.CurrentTest.Questions[i].Text == "")
                    MainViewModel.CurrentTest.Questions.RemoveAt(i);
            }
            AddQuestion();
        }

        private void AddAnswerButton_Click(object sender, RoutedEventArgs e) 
        {
            this.CurrentQuestion = ReadQuestion();
            AnswerClass newAnswer = new AnswerClass();
            newAnswer.Id = this.CurrentQuestion.Answers.Count + 1;
            this.CurrentQuestion.Answers.Add(newAnswer);
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
                //string savepath = Environment.CurrentDirectory + "\\Tests\\" + MainViewModel.CurrentTest.Name + $"{Path.GetExtension(openFileDialog.FileName)}";
                //File.WriteAllBytes(savepath, File.ReadAllBytes(openFileDialog.FileName));
                Image = File.ReadAllBytes(openFileDialog.FileName);
                ImageExt = Path.GetExtension(openFileDialog.FileName);


                BackgroundImage = new Image();
                BackgroundImage.Source = MainViewModel.GetBitmap(openFileDialog.FileName);
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
                        Image = File.ReadAllBytes(path);
                        ImageExt = Path.GetExtension(path);
                        BackgroundImage.Source = MainViewModel.GetBitmap(path);
                        BackgroundImage.Visibility = Visibility.Visible;
                        break;
                    }
                    else
                        BackgroundImage.Visibility = Visibility.Collapsed;
                }
            }
            else
                BackgroundImage.Visibility = Visibility.Collapsed;
        }

        private int CorrectAnsvers()
        {
            int correctAnswers = 0;
            if (CurrentQuestion.Type == QuestionType.String)
                foreach (Grid answerGrid in AnswersControl.Items)
                {
                    correctAnswers++;
                }
            else
            {
                int childrenCount = 0;
                foreach (Grid answerGrid in AnswersControl.Items)
                {
                    switch (CurrentQuestion.Type)
                    {
                        case QuestionType.Single:
                            if ((bool)((RadioButton)answerGrid.Children[0]).IsChecked)
                                correctAnswers++;
                            break;
                        case QuestionType.Multiple:
                            if ((bool)((CheckBox)answerGrid.Children[0]).IsChecked)
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
            newQuestion.Type = CurrentQuestion.Type;
            newQuestion.Value = (int)QuestionValueUpDown.Value;
            if (AnswersTarget > 0)
                newQuestion.AnswersTarget = AnswersTarget;
            else
                newQuestion.AnswersTarget = 1;
            newQuestion.Id = CurrentQuestion.Id;
            newQuestion.Text = QuestionTitle.Text;

            int answerCount = 1;
            if (newQuestion.Type == QuestionType.String)
            {
                newQuestion.IsExact = (bool)IsExactCheckBox.IsChecked;
                foreach (Grid answerGrid in AnswersControl.Items)
                {
                    StackPanel answerButtons = answerGrid.Children[2] as StackPanel;
                    WrapPanel answerWrapPanel = answerGrid.Children[1] as WrapPanel;
                    string answerString = "";
                    foreach (TextBox answer in answerWrapPanel.Children)
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
                    newAnswer.Value = ((answerGrid.Children[2] as StackPanel).Children[0] as IntegerUpDown).Value ?? 0;
                    newQuestion.Answers.Add(newAnswer);
                }
            }
            else
            {
                int childrenCount = 0;
                newQuestion.YesNo = (bool)YesNoCheckBox.IsChecked;

                foreach (Grid answerGrid in AnswersControl.Items)
                {
                    AnswerClass newAnswer = new AnswerClass();
                    newAnswer.Id = answerCount++;
                    newAnswer.Text = (answerGrid.Children[1] as TextBox).Text;
                    newAnswer.Value = ((answerGrid.Children[2] as StackPanel).Children[0] as IntegerUpDown).Value ?? 0;
                    switch (newQuestion.Type)
                    {
                        case QuestionType.Single:
                            newAnswer.IsCorrect = (bool)((RadioButton)answerGrid.Children[0]).IsChecked;
                            break;
                        case QuestionType.Multiple:
                            newAnswer.IsCorrect = (bool)((CheckBox)answerGrid.Children[0]).IsChecked;
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
            string picPath = Path.GetFileNameWithoutExtension(MainViewModel.CurrentTest.Filename) + ".jpg";
            byte[] pic;
            if (File.Exists(picPath))
                pic = File.ReadAllBytes(picPath);
            MainViewModel.CurrentTest.Name = TestTitleTB.Text;
            if ((int)TakeUpDown.Value>0)
                MainViewModel.CurrentTest.Take = (int)TakeUpDown.Value;
            else
                MainViewModel.CurrentTest.Take = MainViewModel.CurrentTest.Questions.Count;
            QuestionClass newQuestion = ReadQuestion();
            autoSave = false;
            CurrentQuestion = newQuestion;
            MainViewModel.CurrentTest.Questions[newQuestion.Id - 1] = newQuestion;
            autoSave = true;
        }

        private void AddQuestion()
        {
            QuestionClass newQuestion = new QuestionClass();
            newQuestion.Type = (QuestionType)NewQuestionType.SelectedIndex;
            newQuestion.Id = MainViewModel.CurrentTest.Questions.Count + 1;
            MainViewModel.CurrentTest.Questions.Add(newQuestion);
            QuestionList = MainViewModel.CurrentTest.Questions;
            autoSave = false;
            CurrentQuestion = QuestionList.ToList()[QuestionList.Count - 1];
            autoSave = true;
        }

        private void DeleteQuestion()
        {
            int deletedQuestionId = CurrentQuestion.Id;
            QuestionList.Remove(CurrentQuestion);
            List<QuestionClass> newQuestionList = QuestionList.ToList();
            for (int i = deletedQuestionId - 1; i < newQuestionList.Count; i++)
            {
                newQuestionList[i].Id--;
            }
            QuestionList = new ObservableCollection<QuestionClass>(newQuestionList);
            autoSave = false;
            if (deletedQuestionId > 1)
                CurrentQuestion = QuestionList[deletedQuestionId - 2];
            else
                CurrentQuestion = QuestionList[0];
            autoSave = true;
        }

        private void LoadQuestion()
        {
            AnswersControl.Items.Clear();
            //AnswersStack.Children.Clear();
            //ShownFlags.Children.Clear();
            //ShownAnswers.Children.Clear();
            //AnswerButtons.Children.Clear();
            TestSettingsStackPanel.Children.Remove(IsExactCheckBox);
            TestSettingsStackPanel.Children.Remove(YesNoCheckBox);

            AnswerList = this.CurrentQuestion.Answers;
            QuestionValueUpDown.Value = CurrentQuestion.Value;
            MovementButtonsAvailability();

            if (CurrentQuestion.Type == QuestionType.String)
            {
                IsExactCheckBox = EditorControls.AddCheckBox("IsExactCheckBox", "Точный ответ", CurrentQuestion.IsExact);
                IsExactCheckBox.Loaded += (s, e) => { RaiseSizeChange(); };
                TestSettingsStackPanel.Children.Add(IsExactCheckBox);

                if (CurrentQuestion.Answers.Count > 0)
                {

                    int answerCount = 1;
                    foreach (var answer in CurrentQuestion.Answers) // строки с ответами
                    {
                        Grid answerGrid = new Grid();

                        TextBlock titileTextBlock = EditorControls.TitleTextBlock(answerCount);

                        string[] variableAnswer = answer.Text.Split("(/)"); // строка с вариациями одного ответа
                        var wrapPanel = EditorControls.AnswerWrapPanel(answerCount);
                        foreach (string word in variableAnswer) // вариации одного ответа
                        {
                            wrapPanel.Children.Add(EditorControls.VariableAnswer(answerCount, word));
                        }

                        DeleteButtonThickness = new Thickness(5, 2.35, 5, 2);

                        StackPanel answerControlButtons = EditorControls.AnsverControlButtons(answer, true);

                        EditorControls.AddToGrid(answerGrid, titileTextBlock, 0);
                        EditorControls.AddToGrid(answerGrid, wrapPanel, 1);
                        EditorControls.AddToGrid(answerGrid, answerControlButtons, 2);

                        AnswersControl.Items.Add(answerGrid);
                        answerCount++;
                    }
                }
            }
            else
            {
                YesNoCheckBox = EditorControls.AddCheckBox("YesNoCheckBox", "Выбор \"Да/Нет\"", CurrentQuestion.YesNo);
                YesNoCheckBox.Loaded += (s, e) => { RaiseSizeChange(); };
                TestSettingsStackPanel.Children.Add(YesNoCheckBox);
                int answerCount = 1;
                foreach (var answer in question.Answers)
                {
                    Grid answerGrid = new Grid();
                    StackPanel answerButtons = new StackPanel() { Margin = new Thickness(5, 2, 5, 2) };

                    Control optionsAnswer = EditorControls.OptionsAnswer(question.Type, answer);
                    Control answerText = EditorControls.OptionsAnswerTextBox(answerCount, answer);
                    StackPanel answerControlButtons = EditorControls.AnsverControlButtons(answer);

                    EditorControls.AddToGrid(answerGrid, optionsAnswer, 0);
                    EditorControls.AddToGrid(answerGrid, answerText, 1);
                    EditorControls.AddToGrid(answerGrid, answerControlButtons, 2);

                    AnswersControl.Items.Add(answerGrid);
                    answerCount++;
                }
            }
            AnswersTarget = CurrentQuestion.AnswersTarget;

            foreach (Grid grid in AnswersControl.Items)
            {
                MainViewModel.AllButtonsHover(grid);
            }
            
        }

        private void MovementButtonsAvailability()
        {
            if (CurrentQuestion.Id == 1)
                PrevQuestionButton.IsEnabled = false;
            else 
                PrevQuestionButton.IsEnabled = true;
            if (CurrentQuestion.Id == QuestionList.Count)
                NextQuestionButton.IsEnabled = false;
            else 
                NextQuestionButton.IsEnabled = true;
        }

        public void RecalculateValues()
        {
            QuestionTitle.Text = CurrentQuestion.Text;
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
                         CurrentQuestion.Answers.Remove(CurrentQuestion.Answers.First(a => a.Id == (obj as AnswerClass).Id));
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
                         obj = CurrentQuestion.Answers.FirstOrDefault(a => a.Id == (obj as AnswerClass).Id);
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

        private void RaiseSizeChange()
        {
            SizeChangedInfo sifo = new SizeChangedInfo(PageGrid, new Size(Double.NaN, Double.NaN), true, true);
            SizeChangedEventArgs ea = typeof(System.Windows.SizeChangedEventArgs).GetConstructors(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).FirstOrDefault().Invoke(
                new object[] { (PageGrid as FrameworkElement), sifo }) as SizeChangedEventArgs;
            ea.RoutedEvent = Panel.SizeChangedEvent;
            PageGrid.RaiseEvent(ea);
        }

        private void ChangeMargin()
        {
            ContentViewer.Margin = new Thickness(0, TopStackPanelScroll.ActualHeight + 5, 0, BottomStackPanelScroll.ActualHeight);
        }
        #endregion
    }
}
