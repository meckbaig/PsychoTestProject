using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace PsychoTestControlPanel
{
    internal class Encryptions
    {
        IMainView view;

        public Encryptions(IMainView view) 
        {
            this.view = view;
        }

        public void FileCoder(string filePath)
        {
            if (!view.operationAllowed)
                return;
            byte[] file = null;
            file = EncryptWithSettings(filePath);
            if (file == null)
                return;

            try
            {
                File.WriteAllBytes(filePath, file);
                view.atLeastOneSuccessfull = true;
            }
            catch (UnauthorizedAccessException)
            {
                if (WpfMessageBox.Show($"Ошибка доступа к файлу {Path.GetFileName(filePath)}\n" +
                    $"Хотите перезапустить программу от имени администратора?", "Серьёзная ошибка!",
                    MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                {
                    view.operationAllowed = false;
                }
                else
                    MainViewModel.RunAsAdmin();
            }
        }

        public void Cycle(string path)
        {
            if (view.operationAllowed)
            {
                foreach (string folder in Directory.GetDirectories(path))
                {
                    if (view.operationAllowed)
                        Cycle(folder);
                    else break;
                }
                foreach (string file in Directory.GetFiles(path))
                {
                    if (view.operationAllowed)
                        FileCoder(file);
                    else break;
                }
            }
        }

        private byte[] EncryptWithSettings(string filePath)
        {
            if (!FileTypesProperties(filePath))
                return null;
            return EncryptAutoDirection(filePath);
        }

        private byte[] EncryptFoolProtection(string filePath)
        {
            byte[] file = new byte[0];

            if (Properties.Settings.Default.XmlFoolProtection && Path.GetExtension(filePath) == ".xml")
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    if (view.isEncodeChecked)
                    {
                        doc.LoadXml(File.ReadAllText(filePath));
                        file = Algorythm.Encrypt(filePath);
                    }
                    else
                    {
                        file = Algorythm.Decrypt(filePath);
                        doc.LoadXml(Encoding.UTF8.GetString(file));
                    }
                }
                catch (Exception) { return null; }
            }
            else
            {
                if (view.isEncodeChecked)
                    file = Algorythm.Encrypt(filePath);
                else
                    file = Algorythm.Decrypt(filePath);
            }
            return file;
        }

        private byte[] EncryptAutoDirection(string filePath)
        {
            if (Properties.Settings.Default.XmlAutoDirection == true && Path.GetExtension(filePath) == ".xml")
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(File.ReadAllText(filePath));
                    return Algorythm.Encrypt(filePath);
                }
                catch (Exception)
                {
                    return Algorythm.Decrypt(filePath);
                }
            }
            else
                return EncryptFoolProtection(filePath);
        }

        private bool FileTypesProperties(string filePath)
        {
            bool result = true;
            if (Properties.Settings.Default.OnlyXmlEncoding == true)
            {
                if (Path.GetExtension(filePath) == ".xml")
                    return true;
                else 
                    result = false;
            }

            if (Properties.Settings.Default.OnlyTextEncoding == true)
            {
                if (Path.GetExtension(filePath) == ".text")
                    return true;
                else
                    result = false;
            }

            return result;
        }

    }
}
