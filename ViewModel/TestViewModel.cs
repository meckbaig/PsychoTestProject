using PsychoTestCourseProject.Extensions;
using PsychoTestCourseProject.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoTestCourseProject.ViewModel
{
    public class TestViewModel
    {
        public string Picture { get; set; } 
        public TestViewModel(TestClass test)
        {
            Supporting.CurrentTest = test;
            Supporting.CurrentQuestion = 0;
            Picture = Path.GetDirectoryName(test.Filename)+"/"+test.Name+".jpg";
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
