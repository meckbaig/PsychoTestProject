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
        TestClass test;
        int idQuestion;

        public TestViewModel(TestClass test)
        {
            this.test = test;
        }

        public QuestionClass CurrentQuestion
        {
            get => test.Questions[idQuestion];
        }

        public QuestionClass NextQuestion()
        {
            if (idQuestion == (test.Questions.Count - 1))
            {
                return null;
            }
            return test.Questions[++idQuestion];
        }
    }
}
