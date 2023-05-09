using PsychoTestProject.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PsychoTestProject.ViewModel
{
    class OrientationTestViewModel : MultiTestViewModel
    {

        public OrientationTestViewModel(TestClass test) : base()
        {
            LoadTest(test);
        }

        private void LoadTest(TestClass test)
        {
            //= Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, $"Tests\\{Title}"), "*.xml")[0];
            MainViewModel.CurrentTest = test;
            MainViewModel.CurrentQuestionNumber = 1;
        }

        public override void PrintResults(Grid thisGrid, ScrollViewer scroll)
        {
            string temp = MainViewModel.CurrentTest.Name;

            TextBlock text = new TextBlock()
            {
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                TextAlignment = System.Windows.TextAlignment.Left,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(20)
            };


            if (temp.ToLower().Contains("определение направленности личности"))
                text.Text = one();
            else if (temp.ToLower().Contains("потребность в общении"))
                text.Text = two();
            else if (temp.ToLower().Contains("оценка потребности в одобрении"))
                text.Text = three();
            else if (temp.ToLower().Contains("диагностика потребности в поисках ощущений"))
                text.Text = four();

            scroll.Content = text;
            MainViewModel.TestStarted = false;

        }

        private string one()
        {
            List<int> OrientationResult = new List<int>() { 0, 0, 0 };
            for (int i = 0; i < AnswersArray.Length; i++)
            {
                if (AnswersArray[i] != 0 && NegativeAnswersArray[i] != 0)
                {
                    OrientationResult[AnswersArray[i] - 1] += 2;
                    OrientationResult[5 - AnswersArray[i] - NegativeAnswersArray[i]] += 1;
                }
            }
            int index = OrientationResult.IndexOf(OrientationResult.Max());
            string levelText = $"  Доминирующая направленность: {ExecuteOrientationResult(index)}";
            for (int i = 0; i < 3; i++)
            {
                levelText += $"\n{ExecuteOrientationResult(i)}: {OrientationResult[i]} ";
                switch (OrientationResult[i] % 10)
                {
                    case 1: levelText += "балл"; break;
                    case > 4: levelText += "баллов"; break;
                    case 0: levelText += "баллов"; break;
                    default: levelText += "балла"; break;
                }
                levelText += $"\n{Encoding.UTF8.GetString(CryptoMethod.Decrypt(Path.Combine(Path.GetDirectoryName(MainViewModel.CurrentTest.Filename), "Определение направленности личности", $"{i}.text")))}";
            }
            return levelText;
        }

        private string ExecuteOrientationResult(int index)
        {
            string result;
            switch (index)
            {
                case 2: result = "Направленность на себя"; break;
                case 1: result = "Направленность на общение"; break;
                default: result = "Направленность на дело"; break;
            };
            return result;
        }

        private string two()
        {
            int[] indexPos = new int[] { 1, 2, 7, 8, 11, 12, 13, 14, 17, 18, 19, 20, 21, 22, 23, 24, 26, 28, 30, 31, 32, 33 };
            int[] indexNeg = new int[] { 3, 4, 5, 6, 9, 10, 15, 16, 25, 27, 29 };

            int result = 0;
            foreach (int i in indexPos)
            {
                if (AnswersArray[i - 1] == 1)
                    result++;
            }
            foreach (int i in indexNeg)
            {
                if (AnswersArray[i - 1] == 0)
                    result++;
            }
            double percentage = ((double)result / (double)AnswersArray.Length) * 100;
            string levelText = $"Ваша потребность в общении: {Math.Round(percentage, 1)}%";
            return levelText;
        }

        private string three()
        {
            int[] indexPos = new int[] { 1, 2, 3, 4, 5, 9, 11, 14, 15, 16, 20 };
            int[] indexNeg = new int[] { 6, 7, 9, 10, 12, 13, 17, 18, 19 };

            int result = 0;
            string levelText = "";

            foreach (int i in indexPos)
            {
                if (AnswersArray[i - 1] == 1)
                    result++;
            }
            foreach (int i in indexNeg)
            {
                if (AnswersArray[i - 1] == 0)
                    result++;
            }
            switch (result)
            {
                case >= 13: levelText = "высокий"; break;
                case >= 10: levelText = "средний"; break;
                case < 10: levelText = "низкий"; break;
            }
            levelText = $"У вас {levelText} уровень потребности одобрения";

            return levelText;

        }

        private string four()
        {
            int[] indexPos = new int[] { 1, 2, 4, 7, 9, 11, 13, 15 };
            int[] indexNeg = new int[] { 3, 5, 6, 8, 10, 12, 14, 16 };

            int result = 0;
            string levelText = "";

            foreach (int i in indexPos)
            {
                if (AnswersArray[i - 1] == 1)
                    result++;
            }
            foreach (int i in indexNeg)
            {
                if (AnswersArray[i - 1] == 0)
                    result++;
            }
            int levelIndex;
            switch (result)
            {
                case >= 11: levelText = "высокий"; levelIndex = 2; break;
                case >= 6: levelText = "средний"; levelIndex = 1; break;
                case < 6: levelText = "низкий"; levelIndex = 0; break;
            }
            levelText = $"У вас {levelText} уровень потребности в ощущениях";
           
            levelText += $"\n{Encoding.UTF8.GetString(CryptoMethod.Decrypt(Path.Combine(Path.GetDirectoryName(MainViewModel.CurrentTest.Filename), "Диагностика потребности в поисках ощущений", $"{levelIndex}.text")))}";


            return levelText;
        }
    }
}
