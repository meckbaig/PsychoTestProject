using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using PsychoTestProject.View.TestKinds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PsychoTestProject.ViewModel
{
    class ProTestViewModel : MultiTestViewModel
    {
        int TestNumber = 0;

        List<TestClass> Tests { get; set; } = new List<TestClass>();
        List<int[]> TestsAnswers { get; set; } = new List<int[]>();
        int Predisposition = 0;
        List<int> Styles = new List<int>() { 0, 0, 0 };
        int Empathy = 0;
        int StopSum = 0;


        public ProTestViewModel() : base()
        {
            Title = "Тест профессионализма";
            LoadTest();
        }

        private void LoadTest()
        {
            string testPath = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, $"Tests\\{Title}"), "*.xml")[TestNumber];
            MainViewModel.CurrentTest = new TestClass(true)
            {
                Name = Path.GetFileNameWithoutExtension(testPath),
                Filename = testPath
            };
            MainViewModel.CurrentQuestionNumber = 1;
        }

        public override void PrintResults(Grid thisGrid, ScrollViewer scroll)
        {
            Tests.Add(MainViewModel.CurrentTest);
            TestsAnswers.Add(AnswersArray);
            if (TestNumber < 2)
            {
                TestNumber++;
                LoadTest();
                AnswersArray = new int[MainViewModel.CurrentTest.Questions.Count];
                (scroll.Content as Question).Initialize(MainViewModel.CurrentTest.Questions[MainViewModel.CurrentQuestionNumber-1], false);
            }
            else
            {
                Calculate();
                StackPanel stackPanel = new StackPanel();
                thisGrid.SizeChanged += (s, e) =>
                {
                    stackPanel.Margin = new Thickness(thisGrid.ActualWidth / 20, thisGrid.ActualHeight / 20, thisGrid.ActualWidth / 20, thisGrid.ActualHeight / 20);
                    stackPanel.Width = thisGrid.ActualWidth * 0.8;
                };
                stackPanel.Children.Add(PredispositionProcessing(thisGrid));
                stackPanel.Children.Add(StylesProcessing());
                stackPanel.Children.Add(EmpathyProcessing(thisGrid));
                scroll.Content = stackPanel;

                SizeChangedInfo sifo = new SizeChangedInfo(thisGrid, new Size(Double.NaN, Double.NaN), true, true);
                SizeChangedEventArgs ea = typeof(System.Windows.SizeChangedEventArgs).GetConstructors(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).FirstOrDefault().Invoke(
                    new object[] { (thisGrid as FrameworkElement), sifo }) as SizeChangedEventArgs;
                ea.RoutedEvent = Panel.SizeChangedEvent;
                thisGrid.RaiseEvent(ea);
            }

        }

        private StackPanel PredispositionProcessing(Grid thisGrid)
        {
            StackPanel stackPanel = new StackPanel() { Margin = new Thickness(10) };
            string level;
            switch (Predisposition)
            {
                case > 20: level = "высокий"; break;
                case > 10: level = "средний"; break;
                default: level = "низкий"; break;
            };
            string levelText = $"   У Вас {level} уровень предрасположенности к педагогической деятельности";

            CompletedTestPreview preview = new CompletedTestPreview(Tests[0], TestsAnswers[0]);
            thisGrid.SizeChanged += (s, e) =>
            {
                preview.PageMaxHeight = thisGrid.ActualHeight * 0.6;
            };

            (TextBlock title, TextBlock text) = CreateText("Предрасположенность к педагогической работе", levelText);
            stackPanel.Children.Add(title);
            stackPanel.Children.Add(text);
            stackPanel.Children.Add(preview);
            return stackPanel;
        }

        private StackPanel StylesProcessing()
        {
            StackPanel stackPanel = new StackPanel() { Margin = new Thickness(10) };
            string style;
            int index = Styles.IndexOf(Styles.Max());
            style = ExecuteStyleIndex(index);
            string levelText = $"   Доминирующий стиль взаимодействия с обучаемыми: {style}";
            for (int i = 0; i<3; i++)
            {
                levelText += $"\n{ExecuteStyleIndex(i)}: {Styles[i]*2} ";
                switch ((Styles[i] * 2) % 10)
                {
                    case 1: levelText += "балл"; break;
                    case >4: levelText += "баллов"; break;
                    case 0: levelText += "баллов"; break;
                    default: levelText += "балла"; break;
                }
            }
            (TextBlock title, TextBlock text) = CreateText("Стиль взаимодействия с обучаемыми", levelText); 
            stackPanel.Children.Add(title);
            stackPanel.Children.Add(text);
            return stackPanel;
        }

        private StackPanel EmpathyProcessing(Grid thisGrid)
        {
            StackPanel stackPanel = new StackPanel() { Margin = new Thickness(10) };
            string level, levelText; int levelId;
            if (StopSum < 4)
            {
                switch (Empathy)
                {
                    case >= 82: level = "очень высокий"; levelId = 0; break;
                    case >= 63: level = "высокий"; levelId = 1; break;
                    case >= 37: level = "средний"; levelId = 2; break;
                    case >= 12: level = "низкий"; levelId = 3; break;
                    default: level = "очень низкий"; levelId = 4; break;
                };
                levelText = $"   У Вас {level} уровень эмпатийности.\n";
                levelText += Encoding.UTF8.GetString(CryptoMethod.Decrypt(Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, $"Tests\\Тест профессионализма\\Empathy"), "*.txt")[levelId]));
            }
            else
                levelText = "   Вы не были достаточно честны в своих ответах";

            CompletedTestPreview preview = new CompletedTestPreview(Tests[2], TestsAnswers[2]);
            thisGrid.SizeChanged += (s, e) =>
            {
                preview.PageMaxHeight = thisGrid.ActualHeight * 0.6;
            };

            (TextBlock title, TextBlock text) = CreateText("Способность педагога к эмпатии", levelText);
            stackPanel.Children.Add(title);
            stackPanel.Children.Add(text);
            stackPanel.Children.Add(preview);
            return stackPanel;
        }

        private static string ExecuteStyleIndex(int index)
        {
            string style;
            switch (index)
            {
                case 1: style = "Демократический"; break;
                case 2: style = "Либеральный"; break;
                default: style = "Авторитарный"; break;
            };
            return style;
        }

        private (TextBlock title, TextBlock text) CreateText(string titleText, string textText)
        {
            TextBlock title = new TextBlock()
            {
                Text = titleText,
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                TextAlignment = System.Windows.TextAlignment.Center,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 10, 0, 10)
            };
            TextBlock text = new TextBlock()
            {
                Text = textText,
                FontSize = 16,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 10, 0, 10)
            };

            return (title, text);
        }

        private void Calculate()
        {
            foreach (int i in TestsAnswers[0])
            {
                Predisposition += i;
            }

            foreach (int i in TestsAnswers[1])
            {
                if (i!=0)
                    Styles[i-1]++;
            }

            int[] stopIndexPos = new int[]
            {
                11, 13, 15, 27
            };
            int[] stopIndexNeg = new int[]
            {
                3, 9, 11, 13, 28, 36
            };
            int[] empathyIndex = new int[]
            {
               2, 5, 8, 9, 10, 12, 13, 15, 16, 19, 21, 22, 24, 25, 26, 27, 29, 32
            };
            foreach (int i in stopIndexPos)
            {
                if (TestsAnswers[2][i - 1] == 5)
                    StopSum++;
            }
            foreach (int i in stopIndexNeg)
            {
                if (TestsAnswers[2][i - 1] == 0)
                    StopSum++;
            }
            foreach (int i in empathyIndex)
            {
                Empathy += TestsAnswers[2][i - 1];
            }
        }
    }
}
