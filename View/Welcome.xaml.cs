using PsychoTestProject.View;
using PsychoTestProject.ViewModel;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Media.Animation;
using System.Drawing.Printing;
using PsychoTestProject.Extensions;
using System.Threading;
using PsychoTestProject.View.TestKinds;
using Microsoft.Win32;
using Microsoft.VisualBasic.FileIO;
using Ionic.Zip;
using Ionic.Zlib;
using System.Xml;
using System.Windows.Shell;
using System.Drawing.Drawing2D;

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
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                Import(args[1]);
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

        private static void Import(string fileName)
        {
            if (Path.GetExtension(fileName) == ".psychoExp")
            {
                DecodeExtract(fileName);
                WpfMessageBox.Show("Успешно импортировано", "Операция выполнена", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                WpfMessageBox.Show($"Выбран файл с неверным форматом ({Path.GetExtension(fileName)} вместо .psychoExp)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void DecodeExtract(string fileName)
        {
            string tmpPath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
            if (Path.GetExtension(fileName) == ".psychoExp")
                tmpPath = Environment.CurrentDirectory;
            else
                Directory.CreateDirectory(tmpPath);
            string zipPath = Path.Combine(tmpPath, $"{Path.GetFileNameWithoutExtension(fileName)}.zip");
            File.WriteAllBytes(zipPath, CryptoMethod.Decrypt(fileName));
            ExtractFiles(zipPath, tmpPath);
            File.Delete(zipPath);

            foreach (string file in Directory.GetFiles(tmpPath))
            {
                if (Path.GetExtension(file) == ".tmp")
                {
                    DecodeExtract(file);
                    File.Delete(file);
                }
            }
        }

        public static void ExtractFiles(string zipPath, string outFolder)
        {
            using (var zip = ZipFile.Read(zipPath))
            {
                zip.AlternateEncodingUsage = ZipOption.Always;
                zip.AlternateEncoding = Encoding.UTF8;
                foreach (ZipEntry e in zip)
                {
                    e.Extract(outFolder, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private static void Export()
        {
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                Title = "Сохранение файлов",
                FileName = $"Export_{Assembly.GetExecutingAssembly().GetName().Version}",
                Filter = "Экспортированные данные программы (*.psychoExp)|*.psychoExp|Все файлы (*.*)|*.*"
            };
            if (fileDialog.ShowDialog() == true)
            {
                string tmpPath = Path.Combine(Environment.CurrentDirectory, "tmp");
                Directory.CreateDirectory(tmpPath);

                File.WriteAllBytes(tmpPath + "\\Lections.tmp", CreateCryptedZip(Path.Combine(Environment.CurrentDirectory, "Lections")));
                File.WriteAllBytes(tmpPath + "\\Tests.tmp", CreateCryptedZip(Path.Combine(Environment.CurrentDirectory, "Tests")));
                File.WriteAllBytes(fileDialog.FileName, CreateCryptedZip(tmpPath));
                Directory.Delete(tmpPath, true);
                WpfMessageBox.Show("Успешно экспортировано", "Операция выполнена", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public static void AppendFilesToZip(string filePath, string zipPath)
        {
            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                zip.AlternateEncodingUsage = ZipOption.Always;
                zip.AlternateEncoding = Encoding.UTF8;
                zip.CompressionLevel = CompressionLevel.Default;
                zip.AddFile(filePath, ""); ;
                zip.Save();
            }
        }
        public static byte[] CreateCryptedZip(string folderPath, bool skipFolders=false)
        {
            string zipPath = folderPath+"_tmp.zip";
            using (var zipFile = new ZipFile())
            {
                zipFile.CompressionLevel = CompressionLevel.Default;
                zipFile.Save(zipPath);
            }
            foreach (string file in Directory.GetFiles(folderPath))
            {
                AppendFilesToZip(file, zipPath);
            }
            if (!skipFolders)
            {
                foreach (string subFolder in Directory.GetDirectories(folderPath))
                {
                    if (Directory.GetFiles(subFolder).Length > 0 || Directory.GetDirectories(subFolder).Length > 0)
                    {
                        string tmpSubFolderPath = subFolder+".tmp";
                        File.WriteAllBytes(tmpSubFolderPath, CreateCryptedZip(subFolder));
                        AppendFilesToZip(tmpSubFolderPath, zipPath);
                        File.Delete(tmpSubFolderPath);
                    }
                }
            }
            byte[] result = CryptoMethod.Encrypt(zipPath);
            File.Delete(zipPath);
            return result;
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
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                Import(files[0]);
            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.PerseveranceTest = null;
            WpfMessageBox.Show($"Продукт предназначен для упрощения процесса обучения и тестирования по дисциплине «Основы психологии личности».\n" +
                            $"Пользователи: студенты и преподаватель дисциплины\n" +
                            $"Версия программы: {Assembly.GetExecutingAssembly().GetName().Version}\n" +
                            $"Год разработки: 2023. Год конечного сопровождения: 2024.\n" +
                            $"По вопросам обращаться на почту: meckbaig@yandex.ru",
                            "Информация о программе PsychoTest", MessageBoxButton.OK);
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new TestEditor());
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Title = "Выберите файл",
                FileName = $"Export_{Assembly.GetExecutingAssembly().GetName().Version}",
                Filter = "Экспортированные данные программы (*.psychoExp)|*.psychoExp|Все файлы (*.*)|*.*"
            };
            if (fileDialog.ShowDialog() == true)
            {
                Import(fileDialog.FileName);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Export();
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LectionsButton_Click(object sender, RoutedEventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 10)
                WpfMessageBox.Show("Внимание!", "Текущая версия операционной системы не совместима с данным модулем программы.", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MainViewModel.MainFrame.Navigate(new Lections(false));
        }

        private void TestsButton_Click(object sender, RoutedEventArgs e)
        {
            TestButtonsAnimation(true, 240);
        }

        private void OpenTestButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.MainFrame.Navigate(new Tests());
        }

        private void PerseveranceTestButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.PerseveranceTest = new PerseveranceTest();
            MainViewModel.MainFrame.Navigate(MainViewModel.PerseveranceTest);
        }

        private void AizenkTestButton_Click(object sender, RoutedEventArgs e)
        {
            MultiTest multiTest = new MultiTest(0);
            if (!MainViewModel.CurrentTest.Error)
                MainViewModel.MainFrame.Navigate(multiTest);
        }

        private void LeongardTestButton_Click(object sender, RoutedEventArgs e)
        {
            MultiTest multiTest = new MultiTest(1);
            if (!MainViewModel.CurrentTest.Error)
                MainViewModel.MainFrame.Navigate(multiTest);
        }

        private void ProTestButton_Click(object sender, RoutedEventArgs e)
        {
            MultiTest multiTest = new MultiTest(2);
            if (!MainViewModel.CurrentTest.Error)
                MainViewModel.MainFrame.Navigate(multiTest);
        }
    }
}