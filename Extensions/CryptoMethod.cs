using Ionic.Zip;
using Ionic.Zlib;
using Microsoft.Win32;
using PsychoTestProject.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PsychoTestProject.Extensions
{
    internal class CryptoMethod
    {
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


        public async static void AppendFilesToZip(string filePath, string zipPath)
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

        public static void Export()
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
                        WpfMessageBox.Show("Успешно экспортировано", "Операция выполнена", MessageBoxButton.OK, MessageBoxImage.Information);
                        timer.Stop();
                    }
                };
                timer.Start();
            }
        }

        public static void Import(string fileName)
        {
            if (Path.GetExtension(fileName) == ".psychoExp")
            {
                WpfMessageBox mBox = new WpfMessageBox("", "Импорт файлов...", WpfMessageBox.MessageBoxType.Information);
                Task task = new Task(() =>
                {
                    DecodeExtract(mBox, fileName);
                });
                task.Start();

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(100);
                timer.Tick += (s, e) =>
                {
                    if (task.IsCompleted)
                    {
                        mBox.CloseMessage();
                        WpfMessageBox.Show("Успешно импортировано", "Операция выполнена", MessageBoxButton.OK, MessageBoxImage.Information);
                        timer.Stop();
                    }
                };
                timer.Start();
            }
            else
                WpfMessageBox.Show($"Выбран файл с неверным форматом ({Path.GetExtension(fileName)} вместо .psychoExp)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void DecodeExtract(WpfMessageBox mBox, string fileName)
        {
            string tmpPath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
            if (Path.GetExtension(fileName) == ".psychoExp")
                tmpPath = Environment.CurrentDirectory;
            else
                Directory.CreateDirectory(tmpPath);
            string zipPath = Path.Combine(tmpPath, $"{Path.GetFileNameWithoutExtension(fileName)}.zip");

            mBox.ChangeMessage($"Импортирую {Path.GetFileName(fileName)}");
            File.WriteAllBytes(zipPath, CryptoMethod.Decrypt(fileName));

            ExtractFiles(zipPath, tmpPath);
            File.Delete(zipPath);


            foreach (string file in Directory.GetFiles(tmpPath))
            {
                if (Path.GetExtension(file) == ".tmp")
                {
                    DecodeExtract(mBox, file);
                    File.Delete(file);
                }
            }
        }

        private static void ExtractFiles(string zipPath, string outFolder)
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
            byte[] result = CryptoMethod.Encrypt(zipPath);
            File.Delete(zipPath);
            return result;
        }
    }
}
