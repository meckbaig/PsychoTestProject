using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace PsychoTestProject.ViewModel
{
    internal class EditorPageControls
    {
        IEditorView view;
        public EditorPageControls(IEditorView view)
        {
            this.view = view;
        }

        public CheckBox IsExactCheckBox(QuestionClass Question)
        {
            CheckBox newIsExactCheckBox = new CheckBox();
            newIsExactCheckBox.IsChecked = Question.IsExact;
            newIsExactCheckBox.Name = "IsExactCheckBox";
            newIsExactCheckBox.Content = "Точный ответ";
            newIsExactCheckBox.FontSize = 14;
            newIsExactCheckBox.Margin = new Thickness(5, 2, 5, 2);
            return newIsExactCheckBox;
        }

        public StackPanel AnswerStackPanel(int answerCount)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Name = "stackPanel" + (answerCount - 1);
            stackPanel.Orientation = Orientation.Horizontal;
            return stackPanel;
        }

        public TextBlock TitleTextBlock(int answerCount)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Ответ " + answerCount;
            textBlock.FontSize = 20;
            textBlock.Margin = new Thickness(5, 2, 5, 2);
            textBlock.FontFamily = new FontFamily("Microsoft YaHei UI");
            return textBlock;
        }

        public Control VariableAnswer(int answerCount, string word)
        {
            TextBox itemText = new TextBox();
            itemText.Name = "itemText" + (answerCount - 1);
            itemText.Text = word;
            itemText.TextWrapping = TextWrapping.Wrap;
            Control item = itemText;
            item = Fonts(item);
            item.Margin = new Thickness(5, 1, 5, 1);
            return item;
        }
        public Control OptionsAnswerTextBox(int answerCount, AnswerClass answer)
        {
            TextBox textBox = new TextBox(); // текст ответа
            textBox.Name = "textBox" + (answerCount - 1);
            textBox.Text = answer.Text;
            textBox.TextWrapping = TextWrapping.Wrap;
            Control item = textBox;
            item = Fonts(item, answer);
            item.Margin = new Thickness(5, 2, 5, 2);
            return item;
        }

        public Control OptionsAnswer(QuestionType questionType, AnswerClass answer)
        {
            Control item;
            switch (questionType) // перебор ответов с выбором вариантов
            {
                case QuestionType.Single:
                    RadioButton rb = new RadioButton();
                    view.DeleteButtonThickness = new Thickness(5, 4.35, 5, 2);
                    rb.IsChecked = answer.IsCorrect;
                    item = rb;
                    item.Margin = new Thickness(5, 9, 5, 8.4);
                    break;
                case QuestionType.Multiple:
                    CheckBox cb = new CheckBox();
                    view.DeleteButtonThickness = new Thickness(5, 4.35, 5, 2);
                    cb.IsChecked = answer.IsCorrect;
                    item = cb;
                    item.Margin = new Thickness(5, 8, 5, 8.3);
                    break;
                default:
                    item = new TextBox(); break;
            }
            item = Fonts(item, answer);
            return item;
        }

        public StackPanel AnsverControlButtons(AnswerClass answer, bool isString = false)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            if (isString)
                AddVariableAnswer(answer, stackPanel);

            Button deleteButton = new Button();
            deleteButton.Content = "Удалить";
            deleteButton.Background = new SolidColorBrush(Color.FromRgb(255, 143, 108));
            deleteButton.Margin = view.DeleteButtonThickness;
            deleteButton.Padding = new Thickness(5, 2, 5, 2);
            deleteButton.BorderBrush = null;
            deleteButton.Command = view.DeleteAnswerCommand;
            deleteButton.CommandParameter = answer;
            deleteButton.Height = 25.05;
            deleteButton.Width = 70;
            Control delButton = deleteButton;
            delButton = Fonts(delButton, null, 15);
            stackPanel.Children.Add(delButton);
            return stackPanel;
        }

        private void AddVariableAnswer(AnswerClass answer, StackPanel stackPanel)
        {
            Button addVariableAnswerButton = new Button();
            addVariableAnswerButton.Height = addVariableAnswerButton.Width = 23;
            addVariableAnswerButton.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            addVariableAnswerButton.SetResourceReference(Button.TemplateProperty, "AddButtonTemplate");
            addVariableAnswerButton.Padding = new Thickness(5, 0, 5, 0);
            addVariableAnswerButton.BorderBrush = null;
            addVariableAnswerButton.Command = view.AddVariableAnswerCommand;
            addVariableAnswerButton.CommandParameter = answer;
            addVariableAnswerButton.Height = 25.05;
            Control varAnswerButton = addVariableAnswerButton;
            varAnswerButton = Fonts(varAnswerButton, null, 17);
            stackPanel.Children.Add(varAnswerButton);
        }

        public Control Fonts(Control item, AnswerClass answer = null, int fontSize = 20)
        {
            FontFamily fontFamily = new FontFamily("Microsoft YaHei UI");
            item.FontSize = fontSize;
            item.FontFamily = fontFamily;
            item.Tag = answer;
            return item;
        }

        public bool Equal(QuestionClass savedQuestion, QuestionClass editedQuestion)
        {
            bool equal = true;

            equal &= savedQuestion.Type.Equals(editedQuestion.Type);
            if (savedQuestion.Type == QuestionType.String)
                equal &= savedQuestion.IsExact.Equals(editedQuestion.IsExact);
            equal &= savedQuestion.Id.Equals(editedQuestion.Id);
            equal &= savedQuestion.Text.Equals(editedQuestion.Text);
            if (savedQuestion.Answers.Count.Equals(editedQuestion.Answers.Count))
                for (int i = savedQuestion.Answers.Count - 1; i >= 0; i--)
                {
                    equal &= savedQuestion.Answers[i].IsCorrect.Equals(editedQuestion.Answers[i].IsCorrect);
                    equal &= savedQuestion.Answers[i].Text.Equals(editedQuestion.Answers[i].Text);
                }
            else
                equal = false;

            return equal;
        }

    }
}
