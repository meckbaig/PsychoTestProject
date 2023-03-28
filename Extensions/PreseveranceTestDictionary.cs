using PsychoTestProject.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace PsychoTestProject.Extensions
{
    class PreseveranceTestDictionary
    {
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
        private List<NumberImageClass> numberImages1 = new List<NumberImageClass>(){
            new NumberImageClass(0,0.095,0.171111,0.7025,0.591111),
            new NumberImageClass(0,0.0825,0.066667,0.88,0.922222),
            new NumberImageClass(0,0.095,0.186667,0.595,0.442222),
            new NumberImageClass(0,0.11,0.177778,0.36125,0.455556),
            new NumberImageClass(0,0.055,0.162222,0.30875,0.04),
            new NumberImageClass(0,0.06375,0.551111,0.225,0),
            new NumberImageClass(0,0.1675,0.097778,0.03125,0.026667),
            new NumberImageClass(0,0.09875,0.268889,0.49,0.164444),
            new NumberImageClass(0,0.095,0.086667,0.4875,0.471111),
            new NumberImageClass(0,0.04625,0.133333,0.55875,0.624444),
            new NumberImageClass(0.444,0.04875,0.133333,0.46125,0.628889),
            new NumberImageClass(0,0.05125,0.071111,0.51,0.566667),
            new NumberImageClass(0,0.06125,0.115556,0.50375,0.782222),
            new NumberImageClass(0,0.04125,0.104444,0.5825,0.786667),
            new NumberImageClass(0,0.04125,0.104444,0.4375,0.786667),
            new NumberImageClass(0,0.035,0.244444,0.63,0.646667),
            new NumberImageClass(0,0.05,0.22,0.3725,0.671111),
            new NumberImageClass(0,0.055,0.093333,0.67125,0.797778),
            new NumberImageClass(0,0.065,0.082222,0.75625,0.777778),
            new NumberImageClass(0,0.0475,0.133333,0.89125,0.553333),
            new NumberImageClass(0,0.03625,0.048889,0.80625,0.717778),
            new NumberImageClass(0,0.08875,0.06,0.875,0.708889),
            new NumberImageClass(0,0.07375,0.111111,0.80125,0.877778),
            new NumberImageClass(0,0.34875,0.086667,0.36375,0.902222),
            new NumberImageClass(0,0.0375,0.046667,0.71375,0.9),
            new NumberImageClass(0,0.03375,0.048889,0.755,0.897778),
            new NumberImageClass(0,0.0375,0.031111,0.71375,0.953333),
            new NumberImageClass(0,0.0325,0.035556,0.755,0.951111),
            new NumberImageClass(0,0.075,0.066667,0.92,0.777778),
            new NumberImageClass(0,0.04875,0.082222,0.94625,0.615556),
            new NumberImageClass(0,0.0675,0.073333,0.81625,0.624444),
            new NumberImageClass(0,0.04875,0.073333,0.94625,0.533333),
            new NumberImageClass(0,0.0875,0.06,0.79625,0.557778),
            new NumberImageClass(0,0.0375,0.068889,0.89625,0.473333),
            new NumberImageClass(0,0.0475,0.037778,0.8925,0.413333),
            new NumberImageClass(0,0.04375,0.031111,0.83875,0.415556),
            new NumberImageClass(0,0.04,0.031111,0.95,0.415556),
            new NumberImageClass(46.661,0.02,0.064444,0.8625,0.451111),
            new NumberImageClass(-46.348,0.0225,0.062222,0.945,0.453333),
            new NumberImageClass(0,0.09875,0.057778,0.86375,0.346667),
            new NumberImageClass(0,0.07125,0.066667,0.87875,0.273333),
            new NumberImageClass(0,0.07125,0.057778,0.87875,0.208889),
            new NumberImageClass(0,0.0925,0.051111,0.86875,0.151111),
            new NumberImageClass(0,0.0275,0.195556,0.965,0.188889),
            new NumberImageClass(0,0.025,0.197778,0.83875,0.188889),
            new NumberImageClass(0,0.135,0.097778,0.8525,0.033333),
            new NumberImageClass(0,0.055,0.54,0.765,0.006667),
            new NumberImageClass(0,0.08125,0.237778,0.675,0.011111),
            new NumberImageClass(0,0.13,0.122222,0.54,0.006667),
            new NumberImageClass(0,0.06875,0.064444,0.6875,0.26),
            new NumberImageClass(0,0.06875,0.075556,0.6875,0.335556),
            new NumberImageClass(0,0.03625,0.151111,0.645,0.26),
            new NumberImageClass(0,0.05125,0.2,0.3825,0.224444),
            new NumberImageClass(0,0.0825,0.093333,0.295,0.224444),
            new NumberImageClass(0,0.08375,0.095556,0.29375,0.328889),
            new NumberImageClass(0,0.155,0.133333,0.38,0.006667),
            new NumberImageClass(0,0.04125,0.111111,0.09625,0.417778),
            new NumberImageClass(0,0.12,0.062222,0.06,0.355556),
            new NumberImageClass(0,0.0775,0.071111,0.07875,0.273333),
            new NumberImageClass(0,0.08125,0.057778,0.07875,0.208889),
            new NumberImageClass(0,0.125,0.06,0.05625,0.142222),
            new NumberImageClass(0,0.04625,0.197778,0.17375,0.191111),
            new NumberImageClass(0,0.05125,0.197778,0.00875,0.191111),
            new NumberImageClass(0,0.03875,0.057778,0.0175,0.428889),
            new NumberImageClass(0,0.04,0.051111,0.1725,0.424444),
            new NumberImageClass(-21.307,0.03125,0.062222,0.1225,0.526667),
            new NumberImageClass(-54.73,0.035,0.066667,0.15875,0.48),
            new NumberImageClass(14.238,0.0325,0.06,0.07625,0.531111),
            new NumberImageClass(46.847,0.03125,0.057778,0.04,0.5),
            new NumberImageClass(5.201,0.055,0.082222,0.08375,0.608889),
            new NumberImageClass(0,0.0575,0.062222,0.14375,0.666667),
            new NumberImageClass(0,0.1175,0.188889,0.2175,0.6),
            new NumberImageClass(0,0.1275,0.093333,0.02375,0.724444),
            new NumberImageClass(0,0.0475,0.057778,0.17125,0.757778),
            new NumberImageClass(0,0.065,0.033333,0.00875,0.655556),
            new NumberImageClass(0,0.06,0.057778,0.00875,0.588889),
            new NumberImageClass(0,0.05875,0.064444,0.145,0.597778),
            new NumberImageClass(0,0.08125,0.086667,0.27625,0.9),
            new NumberImageClass(0,0.1,0.095556,0.17,0.891111),
            new NumberImageClass(0,0.03875,0.048889,0.2525,0.817778),
            new NumberImageClass(0,0.11125,0.095556,0.0575,0.822222),
            new NumberImageClass(0,0.04375,0.091111,0.00875,0.897778),
            new NumberImageClass(0,0.04375,0.093333,0.32375,0.802222),
            new NumberImageClass(36.231,0.04375,0.057778,0.295,0.486667),
            new NumberImageClass(0,0.035,0.04,0.3225,0.431111),
            new NumberImageClass(0,0.0425,0.033333,0.6875,0.42),
            new NumberImageClass(-36.595,0.04125,0.075556,0.7125,0.468889),
            new NumberImageClass(0,0.0325,0.042222,0.595,0.273333),
            new NumberImageClass(0,0.035,0.042222,0.44875,0.268889),
            new NumberImageClass(0,0.0225,0.024444,0.42125,0.648889)
        };
        private List<NumberImageClass> numberImages2 = new List<NumberImageClass>()
        {
            new NumberImageClass(0,0.09,0.122222,0.8675,0.333333),
            new NumberImageClass(0,0.0575,0.062222,0.88375,0.573333),
            new NumberImageClass(0,0.05,0.051111,0.8375,0.511111),
            new NumberImageClass(0,0.05,0.051111,0.9375,0.511111),
            new NumberImageClass(0,0.05,0.051111,0.8375,0.646667),
            new NumberImageClass(0,0.05,0.051111,0.9375,0.646667),
            new NumberImageClass(0,0.035,0.033333,0.83875,0.464444),
            new NumberImageClass(0,0.03375,0.028889,0.95375,0.293333),
            new NumberImageClass(0,0.03375,0.028889,0.84,0.293333),
            new NumberImageClass(0,0.04125,0.028889,0.94625,0.466667),
            new NumberImageClass(0,0.07,0.195556,0.9175,0.082222),
            new NumberImageClass(0,0.10375,0.122222,0.76375,0.02),
            new NumberImageClass(0,0.055,0.128889,0.84875,0.153333),
            new NumberImageClass(0,0.1675,0.28,0.66,0.366667),
            new NumberImageClass(0,0.08375,0.08,0.74375,0.257778),
            new NumberImageClass(0,0.11125,0.173333,0.5525,0.18),
            new NumberImageClass(0,0.05125,0.08,0.6925,0.177778),
            new NumberImageClass(0,0.045,0.1,0.5075,0.333333),
            new NumberImageClass(0,0.075,0.062222,0.5725,0.426667),
            new NumberImageClass(0,0.0575,0.08,0.59,0.511111),
            new NumberImageClass(0,0.12125,0.16,0.32375,0.177778),
            new NumberImageClass(0,0.075,0.062222,0.41625,0.348889),
            new NumberImageClass(0,0.065,0.088889,0.2525,0.168889),
            new NumberImageClass(0,0.0475,0.08,0.3475,0.408889),
            new NumberImageClass(0,0.06375,0.08,0.3475,0.511111),
            new NumberImageClass(0,0.04625,0.102222,0.56375,0.608889),
            new NumberImageClass(0,0.06375,0.048889,0.7175,0.782222),
            new NumberImageClass(0,0.0625,0.055556,0.56375,0.102222),
            new NumberImageClass(0,0.06375,0.062222,0.3725,0.624444),
            new NumberImageClass(0,0.0575,0.053333,0.4725,0.575556),
            new NumberImageClass(0,0.03125,0.108889,0.445,0.646667),
            new NumberImageClass(0,0.03,0.088889,0.525,0.653333),
            new NumberImageClass(0,0.04875,0.028889,0.595,0.766667),
            new NumberImageClass(0,0.0525,0.033333,0.65,0.764444),
            new NumberImageClass(0,0.0475,0.028889,0.59625,0.724444),
            new NumberImageClass(0,0.05125,0.033333,0.65,0.722222),
            new NumberImageClass(0,0.04875,0.028889,0.3,0.76),
            new NumberImageClass(0,0.0525,0.033333,0.355,0.757778),
            new NumberImageClass(0,0.04875,0.031111,0.3,0.715556),
            new NumberImageClass(0,0.05125,0.033333,0.355,0.715556),
            new NumberImageClass(0,0.08,0.088889,0.16625,0.244444),
            new NumberImageClass(0,0.08,0.088889,0.16625,0.671111),
            new NumberImageClass(0,0.0775,0.044444,0.21625,0.786667),
            new NumberImageClass(0,0.0375,0.057778,0.45375,0.022222),
            new NumberImageClass(0,0.35625,0.073333,0.145,0.842222),
            new NumberImageClass(0,0.35625,0.073333,0.50125,0.842222),
            new NumberImageClass(0,0.07,0.148889,0.09,0.704444),
            new NumberImageClass(0,0.07,0.233333,0.0125,0.706667),
            new NumberImageClass(0,0.1075,0.066667,0.07125,0.924444),
            new NumberImageClass(0,0.1075,0.066667,0.31625,0.92),
            new NumberImageClass(0,0.11625,0.066667,0.57875,0.92),
            new NumberImageClass(0,0.1375,0.066667,0.435,0.92),
            new NumberImageClass(0,0.10625,0.06,0.8225,0.926667),
            new NumberImageClass(0,0.1075,0.113333,0.03375,0.333333),
            new NumberImageClass(0,0.1075,0.071111,0.72375,0.66),
            new NumberImageClass(0,0.0625,0.064444,0.055,0.571111),
            new NumberImageClass(0,0.125,0.075556,0.50125,0.015556),
            new NumberImageClass(0,0.07875,0.142222,0.36625,0.015556),
            new NumberImageClass(0,0.0575,0.064444,0.6325,0.015556),
            new NumberImageClass(0,0.0575,0.064444,0.69625,0.015556),
            new NumberImageClass(0,0.0575,0.064444,0.6325,0.091111),
            new NumberImageClass(0,0.0575,0.064444,0.69625,0.093333),
            new NumberImageClass(0,0.05,0.064444,0.2525,0.015556),
            new NumberImageClass(0,0.05125,0.064444,0.30875,0.015556),
            new NumberImageClass(0,0.0475,0.064444,0.255,0.091111),
            new NumberImageClass(0,0.05125,0.064444,0.30875,0.093333),
            new NumberImageClass(0,0.10375,0.122222,0.135,0.02),
            new NumberImageClass(0,0.06375,0.122222,0.09125,0.148889),
            new NumberImageClass(0,0.07,0.197778,0.0125,0.082222),
            new NumberImageClass(0,0.07,0.228889,0.9175,0.711111),
            new NumberImageClass(0,0.06875,0.131111,0.84,0.711111),
            new NumberImageClass(0,0.0525,0.033333,0.7625,0.953333),
            new NumberImageClass(0,0.0525,0.033333,0.71,0.92),
            new NumberImageClass(0,0.0525,0.042222,0.2525,0.915556),
            new NumberImageClass(0,0.05625,0.033333,0.1825,0.953333),
            new NumberImageClass(0,0.05125,0.035556,0.4725,0.197778),
            new NumberImageClass(0,0.0525,0.033333,0.47125,0.095556),
            new NumberImageClass(0,0.035,0.06,0.5175,0.137778),
            new NumberImageClass(0,0.035,0.06,0.4475,0.137778),
            new NumberImageClass(0,0.035,0.033333,0.01125,0.455556),
            new NumberImageClass(0,0.03375,0.028889,0.1225,0.288889),
            new NumberImageClass(0,0.03375,0.033333,0.01125,0.288889),
            new NumberImageClass(0,0.04125,0.028889,0.1175,0.46),
            new NumberImageClass(0,0.05,0.051111,0.01125,0.504444),
            new NumberImageClass(0,0.05,0.051111,0.1075,0.502222),
            new NumberImageClass(0,0.05,0.051111,0.0125,0.64),
            new NumberImageClass(0,0.05,0.051111,0.10625,0.642222),
            new NumberImageClass(0,0.0575,0.053333,0.46875,0.773333),
            new NumberImageClass(0,0.1675,0.28,0.1675,0.366667),
            new NumberImageClass(0,0.04,0.055556,0.4775,0.471111)
        };
        private List<NumberImageClass> numberImages3 = new List<NumberImageClass>()
        {
            new NumberImageClass(0,0.09,0.091111,0.4575,0.895556),
            new NumberImageClass(0,0.0575,0.071111,0.47,0.462222),
            new NumberImageClass(0,0.05875,0.133333,0.47375,0.271111),
            new NumberImageClass(0,0.05875,0.133333,0.47375,0.015556),
            new NumberImageClass(0,0.05875,0.133333,0.52625,0.137778),
            new NumberImageClass(0,0.05375,0.133333,0.42,0.137778),
            new NumberImageClass(0,0.075,0.133333,0.27875,0.351111),
            new NumberImageClass(0,0.075,0.133333,0.645,0.351111),
            new NumberImageClass(0,0.075,0.111111,0.59875,0.206667),
            new NumberImageClass(0,0.0725,0.097778,0.33,0.22),
            new NumberImageClass(0,0.24625,0.413333,0.735,0.286667),
            new NumberImageClass(0,0.24875,0.431111,0.0175,0.286667),
            new NumberImageClass(0,0.075,0.117778,0.27875,0.513333),
            new NumberImageClass(0,0.075,0.124444,0.645,0.506667),
            new NumberImageClass(0,0.06625,0.048889,0.2875,0.071111),
            new NumberImageClass(0,0.0675,0.048889,0.645,0.06),
            new NumberImageClass(0,0.0675,0.048889,0.72125,0.06),
            new NumberImageClass(0,0.0675,0.051111,0.64625,0.124444),
            new NumberImageClass(0,0.0675,0.057778,0.72375,0.124444),
            new NumberImageClass(0,0.0675,0.048889,0.21125,0.071111),
            new NumberImageClass(0,0.0675,0.051111,0.21,0.137778),
            new NumberImageClass(0,0.0675,0.057778,0.29,0.133333),
            new NumberImageClass(0,0.1,0.068889,0.0925,0.022222),
            new NumberImageClass(0,0.075,0.068889,0.82625,0.022222),
            new NumberImageClass(0,0.05625,0.051111,0.35625,0.015556),
            new NumberImageClass(0,0.04625,0.044444,0.59625,0.013333),
            new NumberImageClass(0,0.1175,0.117778,0.0125,0.137778),
            new NumberImageClass(0,0.1175,0.117778,0.0125,0.746667),
            new NumberImageClass(0,0.1175,0.117778,0.87,0.137778),
            new NumberImageClass(0,0.1175,0.117778,0.87,0.737778),
            new NumberImageClass(0,0.08125,0.122222,0.40875,0.586667),
            new NumberImageClass(0,0.08,0.124444,0.515,0.586667),
            new NumberImageClass(0,0.11375,0.091111,0.105,0.891111),
            new NumberImageClass(0,0.105,0.091111,0.79625,0.886667),
            new NumberImageClass(0,0.195,0.217778,0.235,0.744444),
            new NumberImageClass(0,0.195,0.217778,0.575,0.746667),
            new NumberImageClass(0,0.05875,0.071111,0.37125,0.504444),
            new NumberImageClass(0,0.05875,0.055556,0.37125,0.428889),
            new NumberImageClass(0,0.05875,0.071111,0.575,0.502222),
            new NumberImageClass(0,0.05875,0.071111,0.575,0.42),
            new NumberImageClass(0,0.0825,0.106667,0.4575,0.726667)
        };


        public PreseveranceTestDictionary()
        {
            DeleteNumbersDirectory();
            foreach (string image in Directory.GetFiles(Environment.CurrentDirectory + "\\Tests\\Тест на усидчивость\\SpreadSheets", "*.jpg"))
            {
                SpreadSheets.Add(image);
            }
            SpreadSheets = Supporting.Shuffle(SpreadSheets);
            var numberImagesList = new List<List<NumberImageClass>>
            {
                numberImages1,
                numberImages2,
                numberImages3
            };
            if (numberImagesList.Count > 0 && SpreadSheets.Count > 0)
            {
                NumberImages = Supporting.Shuffle(numberImagesList[Convert.ToInt32(Path.GetFileNameWithoutExtension(SpreadSheets[0].ToString())) - 1]);
                NumberImages.Add(new NumberImageClass(0, 1, 1, 0, 0));

                for (int i = 1; i < NumberImages.Count; i++)
                {
                    NumberSources.Add(CreateImage(i));
                }
                NumberSources.Add(SpreadSheets[0]);
            }
        }

        private static void DeleteNumbersDirectory()
        {
            try { Directory.Delete(Environment.CurrentDirectory + "\\Tests\\Тест на усидчивость\\Numbers", true); }
            catch (System.IO.DirectoryNotFoundException) {}
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\Tests\\Тест на усидчивость\\Numbers");
        }

        private string CreateImage(int number)
        {
            Bitmap myBitmap;
            if (number<10)
                myBitmap = new Bitmap(Environment.CurrentDirectory + "\\Tests\\Тест на усидчивость\\blank1.png");
            else
                myBitmap = new Bitmap(Environment.CurrentDirectory + "\\Tests\\Тест на усидчивость\\blank2.png");
            Graphics g = Graphics.FromImage(myBitmap);
            Font font = new Font(Fonts[new Random().Next(0, Fonts.Count)], 46, (FontStyle)new Random().Next(1, 2));
            PointF point = new PointF(myBitmap.Width / 2, myBitmap.Height / 2);

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(number.ToString(), font, System.Drawing.Brushes.Black, point, sf);
            string savesource = Environment.CurrentDirectory + $"\\Tests\\Тест на усидчивость\\Numbers\\{number}.png";
            myBitmap.Save(savesource);
            return savesource;
        }
    }
}
