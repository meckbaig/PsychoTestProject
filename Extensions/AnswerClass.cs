using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoTestProject.Extensions
{
    public class AnswerClass
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public double Value { get; set; }
        public AnswerClass()
        {
            Id = 1;  
            Text = "";
            IsCorrect = false;
            Value = 1;
        }
    }
}
