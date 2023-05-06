using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
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
    public partial class FunFact : Page, INotifyPropertyChanged
    {
        public BitmapImage Picture { get; set; }
        public string PeopleNameText { get; set; }
        public string PeopleDiscripitonText { get; set; }
        public double TitleFont { get => ActualWidth / 18.5; }
        public double DescriptionFont { get => ActualWidth / 23; }

        public FunFact(List<string> types)
        {
            InitializeComponent();
            DataContext = this;
            types = Supporting.Shuffle(types);
            ParceData(types[0]);
        }

        public void ParceData(string type)
        {
            string typeFolder = Path.Combine(Environment.CurrentDirectory, $"Tests\\Тест айзенка\\{type}");
            var sdf = Supporting.Shuffle(Directory.GetDirectories(typeFolder).ToList());
            string peoplePath = sdf[0];
            Picture = MainViewModel.GetBitmap(Directory.GetFiles(peoplePath, "*.jpg")[0]);

            string information = Directory.GetFiles(peoplePath, "*.xml")[0];
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(Encoding.UTF8.GetString(CryptoMethod.Decrypt(information)));
            PeopleNameText = xml.DocumentElement.Attributes["Name"]?.Value;
            PeopleDiscripitonText = xml.DocumentElement.Attributes["Description"]?.Value;
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
