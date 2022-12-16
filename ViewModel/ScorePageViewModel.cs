using PsychoTestCourseProject.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoTestCourseProject.ViewModel
{
    public class ScorePageViewModel
    {
        private int result;
        public string Score { get; set; }
        public int Result { get => result; }

        public ScorePageViewModel(double totalScore, double currentScore)
        {
            result = Calculate(totalScore, currentScore);
            Score = Math.Round(currentScore, 1) + "/" + totalScore;
            MainViewModel.TestStarted = false;
        }

        public int Calculate(double totalScore, double currentScore)
        {
            double percentage = currentScore/totalScore*100;
            if (percentage > 85)
                return 5;
            if (percentage > 75)
                return 4;
            if (percentage > 60)
                return 3;
            return 2;
        }
    }
}
