using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace PsychoTestProject.Extensions
{
    class PreseveranceTestDictionary
    {
        bool notAllowed = false;
        private List<string> Fonts = new List<string>()
        {
            "Tahoma",
            "Microsoft YaHei UI",
            "Calibri",
            "Segoe UI Black",
            "Consolas",
            "Times New Roman"
        };
        public List<string> NumberSources = new List<string>();
        public List<string> SpreadSheets = new List<string>();

        public List<NumberImageClass> NumberImages = new List<NumberImageClass>();

        private static string Path = Environment.CurrentDirectory + "\\Tests\\Тест на усидчивость";

        public PreseveranceTestDictionary()
        {
            DeleteNumbersDirectory();
            foreach (string image in Directory.GetFiles($"{Path}\\SpreadSheets", "*.jpg"))
            {
                SpreadSheets.Add(image);
            }
            SpreadSheets = Supporting.Shuffle(SpreadSheets);
            List<List<NumberImageClass>> numberImagesList = ParceNumberImagesList();

            if (numberImagesList.Count > 0 && SpreadSheets.Count > 0)
            {
                NumberImages = Supporting.Shuffle(numberImagesList[Convert.ToInt32(System.IO.Path.GetFileNameWithoutExtension(SpreadSheets[0].ToString())) - 1]);
                NumberImages.Add(new NumberImageClass(0, 1, 1, 0, 0));

                for (int i = 1; i < NumberImages.Count; i++)
                {
                    NumberSources.Add(CreateImage(i));
                }
                NumberSources.Add(SpreadSheets[0]);
            }
        }

        private static List<List<NumberImageClass>> ParceNumberImagesList()
        {
            var numberImagesList = new List<List<NumberImageClass>>();

            foreach (string dictionaryPath in Directory.GetFiles($"{Path}\\Dictionaries", "*.text"))
            {
                List<NumberImageClass> imageClassList = new List<NumberImageClass>();
                string dictionary = Encoding.UTF8.GetString(CryptoMethod.Decrypt(dictionaryPath));
                dictionary = dictionary.Replace("\r\n", "");
                string[] images = dictionary.Split(';');
                foreach (string image in images)
                {
                    string[] imgProp = image.Split('|');
                    imageClassList.Add(new NumberImageClass(Convert.ToDouble(imgProp[0]),
                                                            Convert.ToDouble(imgProp[1]),
                                                            Convert.ToDouble(imgProp[2]),
                                                            Convert.ToDouble(imgProp[3]),
                                                            Convert.ToDouble(imgProp[4])));
                }
                numberImagesList.Add(imageClassList);
            }

            return numberImagesList;
        }

        private void DeleteNumbersDirectory()
        {
            try 
            { 
                Directory.Delete($"{Path}\\Numbers", true);
                Directory.CreateDirectory($"{Path}\\Numbers");
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Directory.CreateDirectory($"{Path}\\Numbers");
            }
            catch (Exception)
            {
                notAllowed = true;
            }
        }

        private string CreateImage(int number)
        {
            string savesource = $"{Path}\\Numbers\\{number}.png";
            if (notAllowed)
                return savesource;
            Bitmap myBitmap;
            if (number < 10)
                myBitmap = new Bitmap($"{Path}\\blank1.png");
            else
                myBitmap = new Bitmap($"{Path}\\blank2.png");
            Graphics g = Graphics.FromImage(myBitmap);
            Font font = new Font(Fonts[new Random().Next(0, Fonts.Count)], 46, (FontStyle)new Random().Next(1, 2));
            PointF point = new PointF(myBitmap.Width / 2, myBitmap.Height / 2);

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(number.ToString(), font, System.Drawing.Brushes.Black, point, sf);
            
            myBitmap.Save(savesource);
            return savesource;
        }
    }
}
