using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PsychoTestProject.Extensions
{
    public class QuestionClass
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<AnswerClass> Answers { get; set; }
        public QuestionType Type { get; set; }
        public bool IsExact { get; set; }  

        public QuestionClass()
        {
            Id = 1;
            Text = "";
            Answers = new List<AnswerClass>();
            Type = QuestionType.String;
            IsExact = false;
        }
    }
}
