using PsychoTestProject.View;
using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PsychoTestProject.Extensions
{
    public class XmlDocumentClass
    {
        public XmlDocument XmlDoc { get; set; } 
        private TestClass CurrentTest { get; set; }
        private XmlElement Array { get; set; }

        public XmlDocumentClass()
        {

        }

        public XmlDocumentClass(TestClass currentTest)
        {
            try
            {
                CurrentTest = currentTest;
                XmlDoc = new XmlDocument();
                Array = XmlDoc.CreateElement("Array");
                XmlAttribute Take = XmlDoc.CreateAttribute("Take");
                Take.Value = CurrentTest.Take.ToString();
                Array.Attributes.Append(Take);
                Array = AddQuestions(Array);
                XmlDoc.AppendChild(Array);
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(ex.Message, WpfMessageBox.MessageBoxType.Error);
            }
        }
        

        public void Save(string savePath = null)
        {
            if (savePath == null)
                savePath = Environment.CurrentDirectory + "\\Tests\\" + CurrentTest.Name + ".xml";
            XmlDoc.Save(savePath);
            File.WriteAllBytes(savePath, CryptoMethod.Encrypt(savePath));
        }

        public XmlElement AddQuestions(XmlElement array)
        {
            foreach (QuestionClass question in CurrentTest.Questions)
            {
                XmlElement questionElement = XmlDoc.CreateElement("Question");
                questionElement = AddAtributes(questionElement, question);
                array.AppendChild(questionElement);
            }
            return array;
        }

        public XmlElement AddAnswers(XmlElement questionElement, QuestionClass question)
        {
            foreach (AnswerClass answer in question.Answers)
            {
                XmlElement answerElement = XmlDoc.CreateElement("Answer");
                answerElement.Attributes.Append(AddAttribute("IsCorrect", answer.IsCorrect.ToString()));
                answerElement.Attributes.Append(AddAttribute("Text", answer.Text));
                questionElement.AppendChild(answerElement);
            }
            return questionElement;
        }

        public XmlAttribute AddAttribute(string name, string value)
        {
            XmlAttribute attribute = XmlDoc.CreateAttribute(name);
            attribute.Value = value;
            return attribute;
        }

        public XmlElement AddAtributes(XmlElement questionElement, QuestionClass question)
        {
            questionElement.Attributes.Append(AddAttribute("Text", question.Text));
            questionElement.Attributes.Append(AddAttribute("Type", question.Type.ToString()));
            questionElement.Attributes.Append(AddAttribute("Value", question.Value.ToString()));
            questionElement.Attributes.Append(AddAttribute("AnswersTarget", question.AnswersTarget.ToString()));
            if (question.Type == QuestionType.String)
                questionElement.Attributes.Append(AddAttribute("IsExact", question.IsExact.ToString()));
            else
                questionElement.Attributes.Append(AddAttribute("YesNo", question.YesNo.ToString()));
            questionElement = AddAnswers(questionElement, question);

            return questionElement;
        }
    }
}
