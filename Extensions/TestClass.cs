using PsychoTestCourseProject.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace PsychoTestCourseProject.Extensions
{
    public class TestClass
    {
        public string Name { get; set; }
        private List<QuestionClass> questions;
        public List<QuestionClass> Questions { get => questions ?? (questions = ParseFile(Filename)); }
        public string Filename { get; set; }
        //public Array AnswersValues { get; set; }
        //public Array AnswersPoints { get; set; }

        public TestClass()
        {

        }

        public static List<QuestionClass> ParseFile(string fileName)
        {
            try
            {
                var questionList = new List<QuestionClass>();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(File.ReadAllText(fileName));
                var root = doc.DocumentElement;
                var items = root.ChildNodes;
                foreach (XmlElement item in items)
                {
                    QuestionClass question = new QuestionClass();
                    question.Text = item.Attributes["Text"].Value;
                    question.Type = (QuestionType)Enum.Parse(typeof(QuestionType), item.Attributes["Type"]?.Value);
                    if (question.Type == QuestionType.String)
                    {
                        if (Convert.ToBoolean(item.Attributes["IsExact"]?.Value == null))
                            throw new Exception();
                        question.IsExact = Convert.ToBoolean(item.Attributes["IsExact"].Value);
                    }
                    foreach (XmlElement answ in item.ChildNodes)
                    {
                        AnswerClass answer = new AnswerClass();
                        answer.Text = answ.InnerText;
                        answer.IsCorrect = answ.HasAttribute("IsCorrect");
                        question.Answers.Add(answer);
                    }
                    questionList.Add(question);
                }
                questionList = Supporting.Shuffle(questionList).Take(Int32.Parse(root.Attributes["Take"]?.Value ?? questionList.Count.ToString())).ToList();
                return questionList;
            }
            catch (Exception)
            {
                MessageBox.Show("Выбранный файл повреждён или не совместим с текущей версией программы", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
