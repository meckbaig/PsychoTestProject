using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PsychoTestControlPanel
{
    /// <summary>
    /// Логика взаимодействия для WpfMessageBox.xaml
    /// </summary>
    public partial class WpfMessageBox : Window, INotifyPropertyChanged
    {
        private string messageText;
        public string MessageText
        {
            get => messageText;
            set
            {
                messageText = value;
                OnPropertyChanged("MessageText");
            }
        }

        public enum MessageBoxType
        {
            ConfirmationWithYesNo = 0,
            ConfirmationWithYesNoCancel,
            Information,
            Error,
            Warning,
            Message
        }

        //public enum MessageBoxImage
        //{
        //    Warning = 0,
        //    Question,
        //    Information,
        //    Error,
        //    None
        //}
        private WpfMessageBox()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            CoolLook.AllButtonsHover(this.Content);
            DataContext = this;
        }
        public WpfMessageBox(string message, string title, MessageBoxType type)
        {
            InitializeComponent();
            CoolLook.AllButtonsHover(this.Content);
            DataContext = this;
            ShowMessage(message, title, type);
        }
        static WpfMessageBox _messageBox;
        static MessageBoxResult _result = MessageBoxResult.Cancel;

        public void ChangeMessage(string message)
        {
            _messageBox.MessageText = message;
        }

        public void CloseMessage()
        {
            _messageBox.Close();
            _messageBox = null;
        }

        private static void ShowMessage(string message, string title, MessageBoxType type)
        {
            _messageBox = new WpfMessageBox
            { MessageText = message, MessageTitle = { Text = title } };
            switch (type)
            {
                case MessageBoxType.Error: SetImageOfMessageBox(MessageBoxImage.Error); break;
                case MessageBoxType.Warning: SetImageOfMessageBox(MessageBoxImage.Warning); break;
                case MessageBoxType.Information: SetImageOfMessageBox(MessageBoxImage.Information); break;
                default: SetImageOfMessageBox(MessageBoxImage.None); break;
            };
            _messageBox.btnCancel.Visibility = Visibility.Collapsed;
            _messageBox.btnNo.Visibility = Visibility.Collapsed;
            _messageBox.btnYes.Visibility = Visibility.Collapsed;
            _messageBox.btnOk.Visibility = Visibility.Collapsed;
            _messageBox.closeBtn.Visibility = Visibility.Collapsed;
            _messageBox.Show();
        }

        public static MessageBoxResult Show
        (string message, string title, MessageBoxType type)
        {
            switch (type)
            {
                case MessageBoxType.ConfirmationWithYesNo:
                    return Show(message, title, MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                case MessageBoxType.ConfirmationWithYesNoCancel:
                    return Show(message, title, MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
                case MessageBoxType.Information:
                    return Show(message, title, MessageBoxButton.OK,
                    MessageBoxImage.Information);
                case MessageBoxType.Error:
                    return Show(message, title, MessageBoxButton.OK,
                    MessageBoxImage.Error);
                case MessageBoxType.Warning:
                    return Show(message, title, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                default:
                    return MessageBoxResult.No;
            }
        }
        public static MessageBoxResult Show(string message, MessageBoxType type)
        {
            switch (type)
            {
                case MessageBoxType.Error:
                    return Show(message, "Ошибка!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                case MessageBoxType.Warning:
                    return Show(message, "Внимание!", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                default:
                    return Show(message, string.Empty, type);
            }
        }
        public static MessageBoxResult Show(string message)
        {
            return Show(string.Empty, message,
            MessageBoxButton.OK, MessageBoxImage.None);
        }
        public static MessageBoxResult Show
        (string title, string text)
        {
            return Show(title, text,
            MessageBoxButton.OK, MessageBoxImage.None);
        }
        public static MessageBoxResult Show
        (string title, string text, MessageBoxButton button)
        {
            return Show(title, text, button,
            MessageBoxImage.None);
        }
        public static MessageBoxResult Show
        (string text, string title,
        MessageBoxButton button, MessageBoxImage image)
        {
            _messageBox = new WpfMessageBox
            { MessageText = text, MessageTitle = { Text = title } };
            SetVisibilityOfButtons(button);
            SetImageOfMessageBox(image);
            _messageBox.ShowDialog();
            return _result;
        }
        private static void SetVisibilityOfButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    _messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    _messageBox.btnNo.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Visibility = Visibility.Collapsed;
                    _messageBox.btnOk.Focus();
                    break;
                case MessageBoxButton.OKCancel:
                    _messageBox.btnNo.Visibility = Visibility.Collapsed;
                    _messageBox.btnYes.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Focus();
                    break;
                case MessageBoxButton.YesNo:
                    _messageBox.btnOk.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Visibility = Visibility.Collapsed;
                    _messageBox.btnNo.Focus();
                    break;
                case MessageBoxButton.YesNoCancel:
                    _messageBox.btnOk.Visibility = Visibility.Collapsed;
                    _messageBox.btnCancel.Focus();
                    break;
                default:
                    break;
            }
        }
        private static void SetImageOfMessageBox(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Warning:
                    _messageBox.SetImage("exclamation.png");
                    break;
                case MessageBoxImage.Question:
                    _messageBox.SetImage("question.png");
                    break;
                case MessageBoxImage.Information:
                    _messageBox.SetImage("information.png");
                    break;
                case MessageBoxImage.Error:
                    _messageBox.SetImage("error.png");
                    break;
                default:
                    _messageBox.SetImage("chat.png");
                    break;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnOk)
                _result = MessageBoxResult.OK;
            else if (sender == btnYes)
                _result = MessageBoxResult.Yes;
            else if (sender == btnNo)
                _result = MessageBoxResult.No;
            else if (sender == btnCancel)
                _result = MessageBoxResult.Cancel;
            else
                _result = MessageBoxResult.None;
            _messageBox.Close();
            _messageBox = null;
        }
        private void SetImage(string imageName)
        {
            string uri = string.Format("/Resources/{0}", imageName);
            var uriSource = new Uri(uri, UriKind.RelativeOrAbsolute);
            img.Source = new BitmapImage(uriSource);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
