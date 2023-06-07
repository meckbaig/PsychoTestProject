using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace PsychoTestProject.ViewModel
{
    internal class EditorPageControls
    {
        IEditorView view;
        CheckBox IsExactCB;
        CheckBox YesNoCB;
        public EditorPageControls(IEditorView view)
        {
            this.view = view;
            IsExactCB = view.IsExactCB;
            YesNoCB = view.YesNoCB;
        }

        public void AddToGrid(Grid grid, UIElement element, int column)
        {
            if (column == 1)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            else
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            grid.Children.Add(element);
            Grid.SetColumn(element, column);
        }

        public CheckBox AddCheckBox(string name, string text, bool value)
        {
            CheckBox checkBox;
            switch (name)
            {
                case "IsExactCheckBox": checkBox = IsExactCB; break;
                case "YesNoCheckBox": checkBox = YesNoCB; break;
                default: checkBox = new CheckBox(); break;
            };
            checkBox.Width = 75;
            checkBox.Content = new TextBlock() { Text = text, TextWrapping = TextWrapping.Wrap };
            checkBox.IsChecked = value;
            checkBox.Visibility = Visibility.Visible;
            return checkBox;
        }

        public WrapPanel AnswerWrapPanel(int answerCount)
        {
            WrapPanel wrapPanel = new WrapPanel();
            wrapPanel.Name = "stackPanel" + (answerCount - 1);
            wrapPanel.Orientation = Orientation.Horizontal;
            return wrapPanel;
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
            itemText.MinWidth = 50;
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
                                IntegerUpDown updown = ((rb.Parent as Grid).Children[2] as StackPanel).Children[0] as IntegerUpDown;
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
                        IntegerUpDown updown = ((cb.Parent as Grid).Children[2] as StackPanel).Children[0] as IntegerUpDown;
                        if (updown.Value == 0)
                            updown.Value = 1;
                    };
                    item = cb;
                    break;
                default:
                    item = new TextBox(); break;
            }
            view.DeleteButtonThickness = new Thickness(5, 4.35, 5, 2);
            item.Margin = new Thickness(5, 5, 0, 0);
            item = Fonts(item, answer);
            return item;
        }

        public StackPanel AnsverControlButtons(AnswerClass answer, bool isString = false)
        {
            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 2, 0, 2)
            };


            AddValueAnswer(answer, stackPanel);
            if (isString)
                AddVariableAnswer(answer, stackPanel);
            AddDeleteButton(answer, stackPanel);
            return stackPanel;
        }

        private void AddValueAnswer(AnswerClass answer, StackPanel stackPanel)
        {
            IntegerUpDown integerUpDown = new IntegerUpDown()
            {
                Height = 23,
                BorderThickness = new Thickness(0),
                BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                Value = (int)answer.Value,
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = "Баллов за ответ"
            };
            integerUpDown.ValueChanged += (s, e) =>
            {
                var element = ((integerUpDown.Parent as StackPanel).Parent as Grid).Children[0];
                switch (element)
                {
                    case RadioButton:
                        (element as RadioButton).IsChecked = (element as RadioButton).Focusable = integerUpDown.Value != 0; break;
                    case CheckBox:
                        (element as CheckBox).IsChecked = integerUpDown.Value != 0; break;
                    default: break;
                }
            };

            stackPanel.Children.Add(integerUpDown);
        }

        private void AddVariableAnswer(AnswerClass answer, StackPanel stackPanel)
        {
            Button addVariableAnswerButton = new Button()
            {
                Width = 23,
                Height = 23,
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                Margin = new Thickness(5, 0, 0, 0),
                Padding = new Thickness(5, 2, 5, 2),
                BorderBrush = null,
                VerticalAlignment = VerticalAlignment.Center,
                Command = view.AddVariableAnswerCommand,
                CommandParameter = answer,
                ToolTip = "Добавить вариант ответа"
            };
            addVariableAnswerButton.SetResourceReference(Button.TemplateProperty, "AddButtonTemplate");

            Control varAnswerButton = addVariableAnswerButton;
            varAnswerButton = Fonts(varAnswerButton, null, 17);
            stackPanel.Children.Add(varAnswerButton);
        }

        private void AddDeleteButton(AnswerClass answer, StackPanel stackPanel)
        {
            Button deleteButton = new Button()
            {
                Content = "Удалить",
                Background = new SolidColorBrush(Color.FromRgb(255, 143, 108)),
                Margin = view.DeleteButtonThickness,
                Padding = new Thickness(5, 2, 5, 2),
                BorderBrush = null,
                VerticalAlignment = VerticalAlignment.Center,
                Command = view.DeleteAnswerCommand,
                CommandParameter = answer,
                Height = 25.05,
                Width = 70
            };

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
