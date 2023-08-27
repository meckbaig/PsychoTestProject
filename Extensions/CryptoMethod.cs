﻿using Ionic.Zip;
using Ionic.Zlib;
using Microsoft.Win32;
using PsychoTestProject.View;
using PsychoTestProject.ViewModel;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PsychoTestProject.Extensions
{
    internal class CryptoMethod
    {
        static bool error = false;
        public static byte[] Encrypt(string data)
        {
            return Encrypt(File.ReadAllBytes(data));
        }

        public static byte[] Encrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > 239)
                    data[i] = (byte)(data[i] - 240);
                else
                    data[i] = (byte)(data[i] + 16);
            }
            return data;
        }

        public static byte[] Decrypt(string data)
        {
            if (File.Exists(data))
                return Decrypt(File.ReadAllBytes(data));
            else
            {
                string file = Path.GetFileName(data);
                if (file.Length > 40)
                    file = file.Remove(40) + "...";
                WpfMessageBox.Show($"Файл \"{file}\" в папке \"{Path.GetFileName(Path.GetDirectoryName(data))}\" отсутствует. " +
                    $"Проверьте правильность введённых данных или обратитесь к администратору.", WpfMessageBox.MessageBoxType.Error);
                return new byte[0];
            }
        }

        public static byte[] Decrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] < 16)
                    data[i] = (byte)(data[i] + 240);
                else
                    data[i] = (byte)(data[i] - 16);
            }
            return data;
        }


        public static void Export()
        {
            try
            {
                SaveFileDialog fileDialog = new SaveFileDialog()
                {
                    Title = "Сохранение файлов",
                    FileName = $"Export_{Assembly.GetExecutingAssembly().GetName().Version}",
                    Filter = "Экспортированные данные программы (*.psychoExp)|*.psychoExp|Все файлы (*.*)|*.*"
                };
                if (fileDialog.ShowDialog() == true)
                {
                    error = false;
                    string tmpPath = Path.Combine(Environment.CurrentDirectory, "tmp");
                    Directory.CreateDirectory(tmpPath);

                    WpfMessageBox mBox = new WpfMessageBox("", "Экспорт файлов...", WpfMessageBox.MessageBoxType.Information);
                    Task task = new Task(() =>
                    {
                        File.WriteAllBytes(tmpPath + "\\Lections.tmp", CreateCryptedZip(mBox, Path.Combine(Environment.CurrentDirectory, "Lections")));
                        File.WriteAllBytes(tmpPath + "\\Tests.tmp", CreateCryptedZip(mBox, Path.Combine(Environment.CurrentDirectory, "Tests")));
                        File.WriteAllBytes(fileDialog.FileName, CreateCryptedZip(mBox, tmpPath));
                        Directory.Delete(tmpPath, true);
                    });
                    task.Start();

                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(100);
                    timer.Tick += (s, e) =>
                    {
                        if (task.IsCompleted)
                        {
                            mBox.CloseMessage();
                            if (!error)
                                WpfMessageBox.Show("Успешно экспортировано", "Операция выполнена", MessageBoxButton.OK, MessageBoxImage.Information);
                            else
                            {
                                WpfMessageBox.Show("Импорт прерван", WpfMessageBox.MessageBoxType.Error);
                                Directory.Delete(fileDialog.FileName);
                            }
                            timer.Stop();
                        }
                    };
                    timer.Start();
                }
            }
            catch (UnauthorizedAccessException)
            {
                MainViewModel.FilesEditPermission = false;
                if (WpfMessageBox.Show($"Ошибка доступа к файлам\n" +
                    $"Хотите перезапустить программу от имени администратора?", "Серьёзная ошибка!",
                    MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    MainViewModel.RunAsAdmin();
                }
                else
                    error = true;
            }
            catch (Exception)
            {
                error = true;
            }
        }

        public static void Import(string fileName)
        {
            try
            {
                if (Path.GetExtension(fileName) == ".psychoExp")
                {
                    error = false;
                    Exception exception = null;
                    WpfMessageBox mBox = new WpfMessageBox("", "Импорт файлов...", WpfMessageBox.MessageBoxType.Information);
                    Task task = new Task(() =>
                    {
                        try
                        {
                            DecodeExtract(mBox, fileName, ref error, ref exception);
                        }
                        catch (Exception)
                        {

                        }
                    });
                    task.Start();

                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(100);
                    timer.Tick += (s, e) =>
                    {
                        if (task.IsCompleted)
                        {
                            mBox.CloseMessage();
                            if (!error && task.IsCompletedSuccessfully)
                                WpfMessageBox.Show("Успешно импортировано", "Операция выполнена", MessageBoxButton.OK, MessageBoxImage.Information);
                            else if (exception != null)
                            {
                                if (MainViewModel.IsAdmin())
                                    DecodeExtract(mBox, fileName, ref error, ref exception);
                                else if (WpfMessageBox.Show($"Ошибка доступа к файлам программы\n" +
                                    $"Хотите перезапустить программу от имени администратора?", "Серьёзная ошибка!",
                                    MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                                {
                                    MainViewModel.RunAsAdmin();
                                }
                                else
                                    error = true;
                            }
                            else
                            {
                                WpfMessageBox.Show("Импорт прерван.", WpfMessageBox.MessageBoxType.Error);
                            }
                            timer.Stop();
                        }
                    };
                    timer.Start();
                }
                else
                    WpfMessageBox.Show($"Выбран файл с неверным форматом ({Path.GetExtension(fileName)} вместо .psychoExp)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnauthorizedAccessException)
            {
                MainViewModel.FilesEditPermission = false;
                if (WpfMessageBox.Show($"Ошибка доступа к файлам программы\n" +
                    $"Хотите перезапустить программу от имени администратора?", "Серьёзная ошибка!",
                    MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    MainViewModel.RunAsAdmin();
                }
                else
                    error = true;
            }
            catch (Exception)
            {
                error = true;
            }
        }

        private static void DecodeExtract(WpfMessageBox mBox, string fileName, ref bool error, ref Exception exception)
        {
            try
            {
                string tmpPath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
                if (Path.GetExtension(fileName) == ".psychoExp")
                    tmpPath = Environment.CurrentDirectory;
                else
                    Directory.CreateDirectory(tmpPath);
                string zipPath = Path.Combine(tmpPath, $"{Path.GetFileNameWithoutExtension(fileName)}.zip");

                mBox.ChangeMessage($"Импортирую {Path.GetFileName(fileName)}");
                File.WriteAllBytes(zipPath, Decrypt(fileName));

                ExtractFiles(zipPath, tmpPath);
                File.Delete(zipPath);

                foreach (string file in Directory.GetFiles(tmpPath))
                {
                    if (Path.GetExtension(file) == ".tmp")
                    {
                        DecodeExtract(mBox, file, ref error, ref exception);
                        File.Delete(file);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MainViewModel.FilesEditPermission = false;
                error = true;
                exception = ex;
            }
        }

        private static void ExtractFiles(string zipPath, string outFolder)
        {
            if (error)
                return;
            try
            {
                using (var zip = ZipFile.Read(zipPath))
                {
                    zip.AlternateEncodingUsage = ZipOption.Always;
                    zip.AlternateEncoding = Encoding.UTF8;
                    foreach (ZipEntry entry in zip)
                    {
                        entry.Extract(outFolder, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(ex.Message, WpfMessageBox.MessageBoxType.Error);
                error = true;
            }
        }

        private static byte[] CreateCryptedZip(WpfMessageBox mBox, string folderPath, bool skipFolders = false)
        {
            string zipPath = folderPath + "_tmp.zip";
            using (var zipFile = new ZipFile())
            {
                zipFile.CompressionLevel = CompressionLevel.Default;
                zipFile.Save(zipPath);
            }
            foreach (string file in Directory.GetFiles(folderPath))
            {
                mBox.ChangeMessage($"Экспортирую {Path.GetFileName(file)}");
                AppendFilesToZip(file, zipPath);
            }
            if (!skipFolders)
            {
                foreach (string subFolder in Directory.GetDirectories(folderPath))
                {
                    if (Directory.GetFiles(subFolder).Length > 0 || Directory.GetDirectories(subFolder).Length > 0)
                    {
                        string tmpSubFolderPath = subFolder + ".tmp";
                        File.WriteAllBytes(tmpSubFolderPath, CreateCryptedZip(mBox, subFolder));
                        AppendFilesToZip(tmpSubFolderPath, zipPath);
                        File.Delete(tmpSubFolderPath);
                    }
                }
            }
            byte[] result = Encrypt(zipPath);
            File.Delete(zipPath);
            return result;
        }

        private async static void AppendFilesToZip(string filePath, string zipPath)
        {
            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                zip.AlternateEncodingUsage = ZipOption.Always;
                zip.AlternateEncoding = Encoding.UTF8;
                zip.CompressionLevel = CompressionLevel.Default;
                zip.AddFile(filePath, "");
                zip.Save();
            }
        }
    }
}
