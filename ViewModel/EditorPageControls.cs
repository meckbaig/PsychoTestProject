using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace PsychoTestProject.ViewModel
{
    internal class EditorPageControls
    {
        IEditorView view;
        public EditorPageControls(IEditorView view)
        {
            this.view = view;
        }

        public CheckBox AddCheckBox(string name, string text, bool value)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.IsChecked = value;
            checkBox.Name = name;
            checkBox.Content = text;
            checkBox.FontSize = 14;
            checkBox.Margin = new Thickness(5, 2, 5, 2);
            return checkBox;
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
                    rb.IsChecked = answer.IsCorrect;
                    rb.GroupName = "group" + answer.Id;
                    rb.Focusable = rb.IsChecked ?? false;
                    rb.Click += (s, e) =>
                    {
                        if (rb.IsChecked ?? false)
                        {
                            if (rb.Focusable == false)
                            {
                                IntegerUpDown updown = ((((rb.Parent as StackPanel).Parent as StackPanel).Children[2] as StackPanel).Children[answer.Id - 1] as StackPanel).Children[0] as IntegerUpDown;
                                if (updown.Value == 0)
                                    updown.Value = 1;
                                rb.Focusable = true;
                            }
                            else
                                rb.IsChecked = rb.Focusable = false;
                        }
                        else
                            rb.Focusable = false;
                    };
                    item = rb;
                    break;
                case QuestionType.Multiple:
                    CheckBox cb = new CheckBox();
                    cb.IsChecked = answer.IsCorrect;
                    cb.Checked += (s, e) =>
                    {
                        IntegerUpDown updown = ((((cb.Parent as StackPanel).Parent as StackPanel).Children[2] as StackPanel).Children[answer.Id - 1] as StackPanel).Children[0] as IntegerUpDown;
                        if (updown.Value == 0)
                            updown.Value = 1;
                    };
                    item = cb;
                    break;
                default:
                    item = new TextBox(); break;
            }
            view.DeleteButtonThickness = new Thickness(5, 4.35, 5, 2);
            item.Margin = new Thickness(5, 3, 0, 3);
            item = Fonts(item, answer);
            return item;
        }

        public StackPanel AnsverControlButtons(AnswerClass answer, bool isString = false)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            AddValueAnswer(answer, stackPanel);
            if (isString)
                AddVariableAnswer(answer, stackPanel);
            AddDeleteButton(answer, stackPanel);
            return stackPanel;
        }

        private void AddValueAnswer(AnswerClass answer, StackPanel stackPanel)
        {
            IntegerUpDown integerUpDown = new IntegerUpDown();
            integerUpDown.BorderThickness = new Thickness(0);
            integerUpDown.BorderBrush = new SolidColorBrush(Color.FromArgb(0,0,0,0));
            integerUpDown.Value = (int)answer.Value;
            integerUpDown.ValueChanged += (s, e) => 
            {
                var element = ((((integerUpDown.Parent as StackPanel).Parent as StackPanel).Parent as StackPanel).Children[0] as StackPanel).Children[answer.Id-1];
                switch (element)
                {
                    case RadioButton: 
                        (element as RadioButton).IsChecked = (element as RadioButton).Focusable = integerUpDown.Value!=0; break;
                    case CheckBox:
                        (element as CheckBox).IsChecked = integerUpDown.Value != 0; break;
                    default: break;
                }
            };

            stackPanel.Children.Add(integerUpDown);
        }

        private void AddVariableAnswer(AnswerClass answer, StackPanel stackPanel)
        {
            Button addVariableAnswerButton = new Button();
            addVariableAnswerButton.Height = addVariableAnswerButton.Width = 23;
            addVariableAnswerButton.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            addVariableAnswerButton.SetResourceReference(Button.TemplateProperty, "AddButtonTemplate");
            addVariableAnswerButton.Margin = new Thickness(5, 0, 0, 0);
            addVariableAnswerButton.Padding = new Thickness(5, 2, 5, 2);
            addVariableAnswerButton.BorderBrush = null;
            addVariableAnswerButton.Command = view.AddVariableAnswerCommand;
            addVariableAnswerButton.CommandParameter = answer;
            addVariableAnswerButton.Height = 25.05;
            Control varAnswerButton = addVariableAnswerButton;
            varAnswerButton = Fonts(varAnswerButton, null, 17);
            stackPanel.Children.Add(varAnswerButton);
        }

        private void AddDeleteButton(AnswerClass answer, StackPanel stackPanel)
        {
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
