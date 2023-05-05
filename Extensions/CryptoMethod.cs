using PsychoTestProject.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
