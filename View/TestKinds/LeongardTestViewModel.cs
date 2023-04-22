using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PsychoTestProject.View.TestKinds
{
    class LeongardTestViewModel : MultiTestViewModel
    {
        private List<int[]> types = new List<int[]>()
        {
            new int[] {1,14,27,40,53,66,79,92},   // параноик.
            new int[] {2,15,28,41,54,57,80,93},   // эпилептоид.
            new int[] {3,19,29,42,55,68,81,94},   // гипертим.
            new int[] {4,17,30,43,56,69,82,95},   // истероид.
            new int[] {5,18,31,44,57,70,83,96},   // шизоид.
            new int[] {6,19,32,45,58,71,84,97},   // психастеноид.
            new int[] {7,20,33,46,59,72,85,98},   // сензитив.
            new int[] {8,21,34,47,60,73,86,99},   // гипотим.
            new int[] {9,22,35,48,61,74,87,100},  // конформный тип.
            new int[] {10,23,36,49,62,75,88,101}, // неустойчивый тип.
            new int[] {11,24,37,50,63,76,89,102}, // астеник.
            new int[] {12,25,38,51,64,77,90,103}, // лабильный тип.
            new int[] {13,26,39,52,65,78,91,104}  // циклоид.
        };



        public LeongardTestViewModel()
        {
            Title = "Тест «Акцентуации характера К. Леонгард»";
            string testPath = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, $"Tests\\{Title}"), "*.xml")[0];
            MainViewModel.CurrentTest = new TestClass(true) 
            { 
                Name = Path.GetFileNameWithoutExtension(testPath), 
                Filename = testPath 
            };
            MainViewModel.CurrentQuestionNumber = 1;

        }

        public override void PrintResults(Grid thisGrid, ScrollViewer scroll)
        {
            throw new NotImplementedException();
        }
        private int[] CalculateResults()
        {
            int[] results = new int[13];
            for (int i = 0; i < results.Length; i++)
            {
                foreach (int n in types[i]) 
                {
                    results[i] += AnswersArray[n - 1];
                }
            }
            return results;
        }
    }
}
