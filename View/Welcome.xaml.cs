using Ionic.Zip;
using Ionic.Zlib;
using Microsoft.Win32;
using PsychoTestProject.Extensions;
using PsychoTestProject.View;
using PsychoTestProject.View.TestKinds;
using PsychoTestProject.ViewModel;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace PsychoTestProject
{
    /// <summary>
    /// Логика взаимодействия для Welcome.xaml
    /// </summary>
    public partial class Welcome : Page
    {
        /// <summary>
        /// Возможность вызова анимации (чтобы не перекрывались)
        /// </summary>
        public static bool OpenedToAnimate { get; set; }
        /// <summary>
        /// Вызываем или скрываем кнопки
        /// </summary>
        public static bool ShowOrHide { get; set; }
        AnimationClass animationClass { get; set; }
        public Welcome()
        {
            MainViewModel.MainWindow.Title = "PsychoTest";
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                CryptoMethod.Import(args[1]);
            }
            MainViewModel.AllButtonsHover(this.Content);
            OpenedToAnimate = ShowOrHide = true;
        }

        /// <summary>
        /// Универсальный метод для анимации
        /// </summary>
        /// <param name="animationOpened">Направленность действия (вызов или скрытие кнопок)</param>
        /// <param name="duration">Длительность анимации в мс</param>
        private void TestButtonsAnimation(bool animationOpened, int duration)
        {
            if (OpenedToAnimate && ShowOrHide == animationOpened)
            {
                OpenedToAnimate = false;
                animationClass = new AnimationClass(this.Content, animationOpened, duration);
                TestsButton.Visibility = animationOpened ? Visibility.Hidden : Visibility.Visible;
                new Thread(() =>
                {
                    Thread.Sleep(duration);
                    OpenedToAnimate = true;
                    ShowOrHide = !animationOpened;
                }).Start();
            }
        }

        public static void CreateAnimation(Button b, int duration, double x, double y, double reverseX = 0, double reverseY = 0)
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = b.Margin;
            animation.To = new Thickness(b.Margin.Left + x, b.Margin.Top + y, b.Margin.Right + reverseX, b.Margin.Bottom + reverseY);
            animation.Duration = TimeSpan.FromMilliseconds(duration);
            b.BeginAnimation(Button.MarginProperty, animation);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TestButtonsAnimation(false, 240);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (EnterPasswordDialog.Show())
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    CryptoMethod.Import(files[0]);
                }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            WpfMessageBox.Show($"Продукт предназначен для упрощения процесса обучения и тестирования по дисциплине «Основы психологии личности».\n" +
                            $"Пользователи: студенты и преподаватель дисциплины\n" +
                            $"Версия программы: {Assembly.GetExecutingAssembly().GetName().Version}\n" +
                            $"Год разработки: 2023. Год конечного сопровождения: 2024.\n" +
                            $"По вопросам обращаться на почту: meckbaig@yandex.ru",
                            "Информация о программе PsychoTest", MessageBoxButton.OK);
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnterPasswordDialog.Show())
                MainViewModel.MainFrame.Navigate(new TestEditor());
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {

            if (EnterPasswordDialog.Show())
            {
                OpenFileDialog fileDialog = new OpenFileDialog()
                {
                    Title = "Выберите файл",
                    FileName = $"Export_{Assembly.GetExecutingAssembly().GetName().Version}",
                    Filter = "Экспортированные данные программы (*.psychoExp)|*.psychoExp|Все файлы (*.*)|*.*"
                };
                if (fileDialog.ShowDialog() == true)
                {
                    CryptoMethod.Import(fileDialog.FileName);
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnterPasswordDialog.Show())
            {
                try
                {
                    CryptoMethod.Export();
                }
                catch (Exception ex)
                {
                    WpfMessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void LectionsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Lections"), "*.html");
                if (Environment.OSVersion.Version.Major < 10)
                    WpfMessageBox.Show("Внимание!", "Текущая версия операционной системы не совместима с данным модулем программы.", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MainViewModel.MainFrame.Navigate(new Lections(false));
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is IndexOutOfRangeException)
            {
                WpfMessageBox.Show("Файлы лекций отсутствуют или повреждены. Для импорта файлов обратитесь к администратору.", WpfMessageBox.MessageBoxType.Error);
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(ex.Message, WpfMessageBox.MessageBoxType.Error);
            }
        }

        private void TestsButton_Click(object sender, RoutedEventArgs e)
        {
            TestButtonsAnimation(true, 240);
        }

        private void OpenTestButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new Transition(TestType.KnowlegeTest));
        }

        private void PerseveranceTestButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new Transition(TestType.PerseveranceTest));
        }

        private void AizenkTestButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new Transition(TestType.AizenkTest));
        }

        private void LeongardTestButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new Transition(TestType.LeongardTest));
        }

        private void ProTestButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new Transition(TestType.ProTest));
        }

        private void OrientationTestButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new Transition(TestType.OrientationTest));
        }
    }
}