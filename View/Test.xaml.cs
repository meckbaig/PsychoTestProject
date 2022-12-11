using PsychoTestCourseProject.Extensions;
using PsychoTestCourseProject.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace PsychoTestCourseProject.View
{
    /// <summary>
    /// Логика взаимодействия для Test.xaml
    /// </summary>
    public partial class Test : Page
    {
        double currentScore = 0;
        double totalScore;

        public Test(TestClass newTest)
        {
            InitializeComponent();
            DataContext = new TestViewModel(newTest);
        }
        private void Question_Loaded(object sender, RoutedEventArgs e)
        {
            Question.Initialize();
            totalScore = Supporting.CurrentTest.Questions.Count;
        }

        private void NextQuestion(object sender, EventArgs e)
        {
            var nextQuestion = (DataContext as TestViewModel).NextQuestion();
            currentScore += Question.CheckAnswer();

            if (nextQuestion != null)
            {
                Question.Initialize(nextQuestion);
            }
            else
                (Supporting.testFrame as Frame).Navigate(new ScorePage(totalScore, currentScore));
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            (DataContext as TestViewModel).ChangeQuestionMargin(QuestionGrid.ActualWidth, QuestionGrid.ActualHeight);
        }
    }
}
