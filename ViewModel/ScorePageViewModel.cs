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

        public ScorePageViewModel(int totalScore, int currentScore)
        {
            result = Calculate(totalScore, currentScore);
            Score = currentScore + "/" + totalScore;
            Supporting.testStarted = false;
        }

        public int Calculate(double totalScore, double currentScore)
        {
            double percentage = currentScore/totalScore*100;
            if (percentage > 90)
                return 5;
            if (percentage > 75)
                return 4;
            if (percentage > 60)
                return 3;
            return 2;
        }
    }
}
