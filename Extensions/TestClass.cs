using PsychoTestProject.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace PsychoTestProject.Extensions
{
    public class TestClass
    {
        public string Name { get; set; }
        private ObservableCollection<QuestionClass> questions;
        public ObservableCollection<QuestionClass> Questions { get => questions ?? (questions = ParseFile(Filename)); }
        public string Filename { get; set; }
        public bool UseTake { get; set; }
        public int Take { get; set; }
        public bool Error { get; set; } = false;

        public TestClass(bool useTake)
        {
            UseTake = useTake;
        }

        private ObservableCollection<QuestionClass> ParseFile(string fileName)
        {
            try
            {
                if (Filename != null && !Error)
                {
                    var questionList = new List<QuestionClass>();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(Encoding.UTF8.GetString(CryptoMethod.Decrypt(fileName)));
                    //doc.LoadXml(File.ReadAllText(fileName));
                    var root = doc.DocumentElement;
                    var items = root.ChildNodes;
                    int questionId = 1;
                    Take = Int32.Parse(root.Attributes["Take"]?.Value ?? questionList.Count.ToString());
                    foreach (XmlElement item in items)
                    {
                        QuestionClass question = new QuestionClass();
                        question.Id = questionId++;
                        question.Text = item.Attributes["Text"].Value;
                        question.Type = (QuestionType)Enum.Parse(typeof(QuestionType), item.Attributes["Type"]?.Value);
                        question.Value = Convert.ToInt32(item.Attributes["Value"]?.Value ?? "0");
                        question.AnswersTarget = Convert.ToInt32(item.Attributes["AnswersTarget"]?.Value ?? "0");
                        if (question.Type == QuestionType.String)
                        {
                            if (Convert.ToBoolean(item.Attributes["IsExact"]?.Value == null))
                                throw new Exception();
                            question.IsExact = Convert.ToBoolean(item.Attributes["IsExact"].Value);
                        }
                        int answerCount = 1;
                        foreach (XmlElement answ in item.ChildNodes)
                        {
                            AnswerClass answer = new AnswerClass();
                            answer.Id = answerCount++;
                            answer.Text = answ.Attributes["Text"].Value;
                            answer.IsCorrect = Convert.ToBoolean(answ.Attributes["IsCorrect"].Value);
                            answer.Value = Convert.ToInt32(answ.Attributes["Value"]?.Value ?? "1");
                            question.Answers.Add(answer);
                        }
                        questionList.Add(question);
                    }
                    if (UseTake)
                        questionList = Supporting.Shuffle(questionList).Take(Take).ToList();
                    return new ObservableCollection<QuestionClass>(questionList);
                }
                else { return new ObservableCollection<QuestionClass>(); }
            }
            catch (Exception)
            {
                Error = true;
                WpfMessageBox.Show("Выбранный файл повреждён или не совместим с текущей версией программы", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
