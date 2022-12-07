using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoTestCourseProject.Extensions
{
    public class AnswerClass
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public AnswerClass()
        {
            Text = "";
            IsCorrect = false;
        }
    }
}
