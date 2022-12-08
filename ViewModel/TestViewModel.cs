using PsychoTestCourseProject.Extensions;
using PsychoTestCourseProject.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoTestCourseProject.ViewModel
{
    public class TestViewModel
    {

        public TestViewModel(TestClass test)
        {
            Supporting.CurrentTest = test;
            Supporting.CurrentQuestion = 0;
        }

        public QuestionClass CurrentQuestion
        {
            get => Supporting.CurrentTest.Questions[Supporting.CurrentQuestion];
        }

        public QuestionClass NextQuestion()
        {
            if (Supporting.CurrentQuestion == (Supporting.CurrentTest.Questions.Count - 1))
            {
                return null;
            }
            return Supporting.CurrentTest.Questions[++Supporting.CurrentQuestion];
        }

        //public QuestionClass PreviousQuestion()
        //{
        //    if (idQuestion == 0)
        //    {
        //        return null;
        //    }
        //    return Supporting.CurrentTest.Questions[--idQuestion];
        //}
    }
}
