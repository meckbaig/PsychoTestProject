using PsychoTestCourseProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using static System.Formats.Asn1.AsnWriter;

namespace PsychoTestCourseProject.View
{
    /// <summary>
    /// Логика взаимодействия для ScorePage.xaml
    /// </summary>
    public partial class ScorePage : Page
    {
        public ScorePage(double totalScore, double currentScore)
        {
            InitializeComponent();
            DataContext = new ScorePageViewModel(totalScore, currentScore);
        }

    }
}
