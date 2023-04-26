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

        public MultiTest(int testId)
        {
            InitializeComponent();
            MainViewModel.MouseHover(BackButton);
            switch (testId)
            {
                case 0: viewModel = new AizenkTestViewModel(); break;
                case 1: viewModel = new LeongardTestViewModel(); break;
            }
            MainViewModel.MainWindow.Title = viewModel.Title;
            DataContext = viewModel;
            viewModel.AnswersArray = new int[MainViewModel.CurrentTest.Questions.Count];
        }

        private void Question_Loaded(object sender, RoutedEventArgs e)
        {
            Question.Initialize(false, true);
        }

        private void NextQuestion(object sender, EventArgs e)
        {
            //viewModel.PrintResults(ThisGrid, Scroll);
            viewModel.AnswersArray[viewModel.CurrentQuestion.Id - 1] = (int)Question.CheckAnswer();
            var nextQuestion = viewModel.NextQuestion();

            if (nextQuestion != null)
            {
                Question.Initialize(nextQuestion, false);
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
                Question.Initialize(previousQuestion, false);
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            viewModel.ChangeMargin(Question.ActualWidth, Question.ActualHeight);
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
