using PsychoTestProject.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;

namespace PsychoTestProject.View.TestKinds
{
    /// <summary>
    /// Логика взаимодействия для FunFact.xaml
    /// </summary>
    public partial class LeongardFact : Page, INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Picture { get; set; }
        public string TitleText { get; set; }
        public string DiscripitonText { get; set; }
        public double TitleFont { get => (ActualWidth>0) ? ActualWidth / 18.5 : 27; }
        public double DescriptionFont { get => (ActualWidth > 0) ? ActualWidth / 40 : 12; }

        public LeongardFact(int type)
        {
            ID = type;
            InitializeComponent();
            DataContext = this;
            ParceData(type);
        }

        public void ParceData(int type)
        {
            try
            {
                string typeFolder = Path.Combine(Environment.CurrentDirectory, $"Tests\\Тест «Акцентуации характера К. Леонгард»\\{type}");
                Picture = Directory.GetFiles(typeFolder, "*.jpg")[0];

                string file = Directory.GetFiles(typeFolder, "*.txt")[0];
                TitleText = Path.GetFileNameWithoutExtension(file);
                DiscripitonText = Encoding.UTF8.GetString(CryptoMethod.Decrypt(file));
            }
            catch (Exception)
            {
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnPropertyChanged("TitleFont");
            OnPropertyChanged("DescriptionFont");
        }
    }
}
