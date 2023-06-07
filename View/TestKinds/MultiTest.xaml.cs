using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
using ScottPlot;
using ScottPlot.Drawing;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using static ScottPlot.Plottable.PopulationPlot;

namespace PsychoTestProject.View.TestKinds
{
    /// <summary>
    /// Логика взаимодействия для AizenkTest.xaml
    /// </summary>
    public partial class MultiTest : Page
    {
        MultiTestViewModel viewModel;

        public MultiTest(TestType testType, TestClass test = null)
        {
            switch (testType)
            {
                case TestType.AizenkTest:      
                    viewModel = new AizenkTestViewModel(); break;
                case TestType.LeongardTest:    
                    viewModel = new LeongardTestViewModel(); break;
                case TestType.ProTest:         
                    viewModel = new ProTestViewModel(); break;
                case TestType.OrientationTest: 
                    viewModel = new OrientationTestViewModel(test);
                    viewModel.BackButtonVisibility = Visibility.Hidden; break;
            }
            if (MainViewModel.CurrentTest?.Questions?.Count > 0)
            {
                viewModel.AnswersArray = new int[MainViewModel.CurrentTest.Questions.Count];
                if (MainViewModel.CurrentTest.Questions[0].YesNo)
                    viewModel.NegativeAnswersArray = new int[MainViewModel.CurrentTest.Questions.Count];
                DataContext = viewModel;
                InitializeComponent();
                MainViewModel.MouseHover(BackButton);
            }
        }

        private void Question_Loaded(object sender, RoutedEventArgs e)
        {
            Question.Initialize(true, false, MainViewModel.CurrentTest.Questions[0].YesNo);
        }

        private void NextQuestion(object sender, EventArgs e)
        {
            viewModel.AnswersArray[viewModel.CurrentQuestion.Id - 1] = (int)Question.CheckAnswer();
            if (viewModel.NegativeAnswersArray?.Count() > 0)
                viewModel.NegativeAnswersArray[viewModel.CurrentQuestion.Id - 1] = (int)Question.CheckAnswerYesNo(false);
            var nextQuestion = viewModel.NextQuestion();

            if (nextQuestion != null)
            {
                Question.Initialize(nextQuestion, startTimer: false, yesNo: nextQuestion.YesNo);
            }
            else
            {
                viewModel.PrintResults(ThisGrid, Scroll);
            }
        }

        private void PreviousQuestion(object sender, EventArgs e)
        {
            var previousQuestion = viewModel.PreviousQuestion();
            if (previousQuestion != null)
            {
                Question.Initialize(previousQuestion, startTimer: false, enableBackButton: MainViewModel.CurrentQuestionNumber != 1);
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            viewModel.ChangeMargin(Scroll.ActualWidth, Scroll.ActualHeight, Question);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (WpfMessageBox.Show("Вы точно хотите покинуть тест?", "Выход из теста", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Exit();
            }
        }

        private void Exit()
        {
            MainViewModel.Back();
        }
    }
}
