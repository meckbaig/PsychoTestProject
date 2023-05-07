using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionPolibiem
{
    internal class Algorythm
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
            return Decrypt(File.ReadAllBytes(data));
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
