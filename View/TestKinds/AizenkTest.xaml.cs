using PsychoTestProject.Extensions;
using PsychoTestProject.ViewModel;
using ScottPlot;
using ScottPlot.Drawing;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using static ScottPlot.Plottable.PopulationPlot;

namespace PsychoTestProject.View.TestKinds
{
    /// <summary>
    /// Логика взаимодействия для AizenkTest.xaml
    /// </summary>
    public partial class AizenkTest : Page
    {
        AizenkTestViewModel viewModel;

        public AizenkTest()
        {
            InitializeComponent();
            MainViewModel.MouseHover(BackButton);
            string testPath = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Tests\\Тест айзенка"), "*.xml")[0];
            MainViewModel.CurrentTest = new TestClass(true) { Name = Path.GetFileNameWithoutExtension(testPath), Filename = testPath };
            viewModel = new AizenkTestViewModel();
            DataContext = viewModel;

            viewModel.AnswersArray = new int[MainViewModel.CurrentTest.Questions.Count];
        }

        private void Question_Loaded(object sender, RoutedEventArgs e)
        {
            Question.Initialize(false, true);
        }

        private void NextQuestion(object sender, EventArgs e)
        {
            //viewModel.AnswersArray[viewModel.CurrentQuestion.Id - 1] = (int)Question.CheckAnswer();
            //var nextQuestion = viewModel.NextQuestion();

            //if (nextQuestion != null)
            //{
            //    Question.Initialize(nextQuestion, false);
            //}
            //else
            //{
                if (viewModel.CalculateIndicator(viewModel.indexAPos, viewModel.indexANeg) <= 4)
                {
                //int extraversion = viewModel.CalculateIndicator(viewModel.indexBPos, viewModel.indexBNeg);
                //int neuroticism = viewModel.CalculateIndicator(viewModel.indexCPos);
                int extraversion = 17;
                int neuroticism = 18;

                string type;
                    switch (extraversion)
                    {
                        case > 12:
                            switch (neuroticism)
                            {
                                case > 12:
                                    type = "Вы холерик";
                                    break;
                                case < 12:
                                    type = "Вы сангвиник";
                                    break;
                                default:
                                    type = "Между холериком и сангвиником";
                                    break;
                            }
                            break;
                        case < 12:
                            switch (neuroticism)
                            {
                                case > 12:
                                    type = "Вы меланхолик";
                                    break;
                                case < 12:
                                    type = "Вы флегматик";
                                    break;
                                default:
                                    type = "Между меланхоликом и флегматиком";
                                    break;
                            }
                            break;
                        default:
                            switch (neuroticism)
                            {
                                case > 12:
                                    type = "Между меланхоликом и холериком";
                                    break;
                                case < 12:
                                    type = "Между флегматиком и сангвиником";
                                    break;
                                default:
                                    type = "Наблюдаются черты всех типов";
                                    break;
                            }
                            break;
                    }


                    StackPanel stackPanel = new StackPanel() { VerticalAlignment = System.Windows.VerticalAlignment.Top, Orientation = System.Windows.Controls.Orientation.Vertical };

                    (TextBlock titleText, TextBlock textText, List<string> types) = Title(type);
                    viewModel.FunFact = new FunFact(types);
                    viewModel.FunFrame = new Frame()
                    {
                        Content = viewModel.FunFact,
                        Visibility = Visibility.Hidden,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    };
                    WpfPlot plot = viewModel.Plot(stackPanel, extraversion, neuroticism, ThisGrid);

                    stackPanel.Children.Add(titleText);
                    stackPanel.Children.Add(plot);
                    stackPanel.Children.Add(textText);
                    ThisGrid.Children.Add(viewModel.FunFrame);
            

            Scroll.Content = stackPanel;
            }
            else MessageBox.Show("Error 234-56:98");
            //}

        }

        private void PreviousQuestion(object sender, EventArgs e)
        {
            var previousQuestion = viewModel.PreviousQuestion();
            if (previousQuestion != null)
            {
                Question.Initialize(previousQuestion, false);
            }
        }

        private new (TextBlock title, TextBlock text, List<string> types) Title(string type)
        {
            TextBlock title = new TextBlock()
            {
                Text = type,
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                TextAlignment= System.Windows.TextAlignment.Center,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            };
            TextBlock text = new TextBlock()
            {
                FontSize = 16,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 20, 0, 0)
            };
            List<string> types = new List<string>();
            if (type.ToLower().Contains("меланхолик") || type == "Наблюдаются черты всех типов")
            {
                text.Text += "   У меланхолика реакция часто не соответствует силе раздражителя, присутствует глубина и устойчивость чувств при слабом их выражении. " +
                    "Ему трудно долго на чем-то сосредоточиться. Сильные воздействия часто вызывают у меланхолика продолжительную тормозную реакцию " +
                    "(опускаются руки). Ему свойственны сдержанность и приглушенность моторики и речи, застенчивость, робость, нерешительность. " +
                    "В нормальных условиях меланхолик — человек глубокий, содержательный, может быть хорошим тружеником, успешно " +
                    "справляться с жизненными задачами. При неблагоприятных условиях может превратиться в замкнутого, боязливого, тревожного, " +
                    "ранимого человека, склонного к тяжелым внутренним переживаниям таких жизненных обстоятельств, которые вовсе этого не заслуживают.\n\n";
                types.Add("Меланхолик");
            }
            if (type.ToLower().Contains("холерик") || type == "Наблюдаются черты всех типов")
            {
                text.Text += "   Холерик отличается повышенной возбудимостью, действия прерывисты. Ему свойственны резкость и стремительность движений, сила, " +
                        "импульсивность, яркая выраженность эмоциональных переживаний. Вследствие неуравновешенности, увлекшись делом, склонен действовать изо " +
                        "всех сил, истощаться больше, чем следует. Имея общественные интересы, темперамент проявляет в инициативности, энергичности, " +
                        "принципиальности. При отсутствии духовной жизни холерический темперамент часто проявляется в раздражительности, эффективности, " +
                        "несдержанности, вспыльчивости, неспособности к самоконтролю при эмоциональных обстоятельствах.\n\n";
                types.Add("Холерик");
            }
            if (type.ToLower().Contains("флегматик") || type == "Наблюдаются черты всех типов")
            {
                text.Text += "   Флегматик характеризуется сравнительно низким уровнем активности поведения, новые формы которого вырабатываются медленно, " +
                    "но являются стойкими. Обладает медлительностью и спокойствием в действиях, мимике и речи, ровностью, постоянством, глубиной чувств " +
                    "и настроений. Настойчивый и упорный «труженик жизни», он редко выходит из себя, не склонен к аффектам, рассчитав свои силы, доводит " +
                    "дело до конца, ровен в отношениях, в меру общителен, не любит попусту болтать. Экономит силы, попусту их не тратит. В зависимости " +
                    "от условий в одних случаях флегматик может характеризоваться «положительными» чертами — выдержкой, глубиной мыслей, постоянством, " +
                    "основательностью и т. д., в других — вялостью, безучастностью к окружающему, ленью и безволием, бедностью и слабостью эмоций, " +
                    "склонностью к выполнению одних лишь привычных действий.\n\n";
                types.Add("Флегматик");
            }
            if (type.ToLower().Contains("сангвиник") || type == "Наблюдаются черты всех типов") 
            {
                text.Text += "   Сангвиник быстро приспосабливается к новым условиям, быстро сходится с людьми, общителен. Чувства легко возникают и " +
                     "сменяются, эмоциональные переживания, как правило, неглубоки. Мимика богатая, подвижная, выразительная. Несколько непоседлив, " +
                     "нуждается в новых впечатлениях, недостаточно регулирует свои импульсы, не умеет строго придерживаться выработанного распорядка, " +
                     "жизни, системы в работе. В связи с этим не может успешно выполнять дело, требующее равной затраты сил, длительного и методичного " +
                     "напряжения, усидчивости, устойчивости внимания, терпения. При отсутствии серьезных целей, глубоких мыслей, творческой деятельности " +
                     "вырабатываются поверхностность и непостоянство.\n\n";
                types.Add("Сангвиник"); 
                
            }
            ThisGrid.SizeChanged += (s, e) =>
            {
                float plotSize;
                if (ActualWidth > ActualHeight)
                    plotSize = (float)(ActualHeight * 0.7);
                else
                    plotSize = (float)(ActualWidth * 0.7);
                title.FontSize = plotSize / 15; text.FontSize = plotSize / 30;
            };


            return (title, text, types);
        }

        

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            viewModel.ChangeMargin(Question.ActualWidth, Question.ActualHeight);
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите покинуть тест?", "Выход из теста", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Exit();
            }

        }

        private void Exit()
        {
            MainViewModel.Back();
            MainViewModel.CurrentTest = null;
        }
    }
}
