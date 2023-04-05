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

namespace PsychoTestProject.View.TestKinds
{
    /// <summary>
    /// Логика взаимодействия для AizenkTest.xaml
    /// </summary>
    public partial class AizenkTest : Page
    {
        AizenkTestViewModel viewModel;

        enum PsychoType { Меланхолик, Холерик, Сангвиник, Флегматик, НеОпределён }

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
            viewModel.AnswersArray[viewModel.CurrentQuestion.Id - 1] = (int)Question.CheckAnswer();
            var nextQuestion = viewModel.NextQuestion();

            if (nextQuestion != null)
            {
                Question.Initialize(nextQuestion, false);
            }
            else
            {
                if (viewModel.CalculateIndicator(viewModel.indexAPos, viewModel.indexANeg) <= 4)
                {
                    int extraversion = viewModel.CalculateIndicator(viewModel.indexBPos, viewModel.indexBNeg);
                    int neuroticism = viewModel.CalculateIndicator(viewModel.indexCPos);
                    //int extraversion = 11;
                    //int neuroticism = 15;

                    PsychoType type;
                    switch (extraversion)
                    {
                        case > 12:
                            switch (neuroticism)
                            {
                                case > 12:
                                    type = PsychoType.Холерик;
                                    break;
                                case < 12:
                                    type = PsychoType.Сангвиник;
                                    break;
                                default:
                                    type = PsychoType.НеОпределён;
                                    break;
                            }
                            break;
                        case < 12:
                            switch (neuroticism)
                            {
                                case > 12:
                                    type = PsychoType.Меланхолик;
                                    break;
                                case < 12:
                                    type = PsychoType.Флегматик;
                                    break;
                                default:
                                    type = PsychoType.НеОпределён;
                                    break;
                            }
                            break;
                        default:
                            type = PsychoType.НеОпределён;
                            break;
                    }


                    StackPanel stackPanel = new StackPanel() { VerticalAlignment = System.Windows.VerticalAlignment.Top, Orientation = System.Windows.Controls.Orientation.Vertical };

                    WpfPlot plot = Plot(stackPanel, extraversion, neuroticism);
                    (TextBlock titleText, TextBlock textText) = Title(type, plot);

                    stackPanel.Children.Add(titleText);
                    stackPanel.Children.Add(plot);
                    stackPanel.Children.Add(textText);

                    Scroll.Content = stackPanel;
                }
                else MessageBox.Show("Error 234-56:98");
            }

        }

        private void PreviousQuestion(object sender, EventArgs e)
        {
            var previousQuestion = viewModel.PreviousQuestion();
            if (previousQuestion != null)
            {
                Question.Initialize(previousQuestion, false);
            }
        }

        private (TextBlock title, TextBlock text) Title(PsychoType type, WpfPlot plot)
        {
            TextBlock title = new TextBlock()
            {
                Text = "Вы "+type.ToString(),
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
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
            switch (type)
            {
                case PsychoType.Меланхолик:
                    text.Text = "   У меланхолика реакция часто не соответствует силе раздражителя, присутствует глубина и устойчивость чувств при слабом их выражении. " +
                        "Ему трудно долго на чем-то сосредоточиться. Сильные воздействия часто вызывают у меланхолика продолжительную тормозную реакцию " +
                        "(опускаются руки). Ему свойственны сдержанность и приглушенность моторики и речи, застенчивость, робость, нерешительность. " +
                        "В нормальных условиях меланхолик — человек глубокий, содержательный, может быть хорошим тружеником, успешно " +
                        "справляться с жизненными задачами. При неблагоприятных условиях может превратиться в замкнутого, боязливого, тревожного, " +
                        "ранимого человека, склонного к тяжелым внутренним переживаниям таких жизненных обстоятельств, которые вовсе этого не заслуживают.";
                    break;
                case PsychoType.Холерик:
                    text.Text = "   Холерик отличается повышенной возбудимостью, действия прерывисты. Ему свойственны резкость и стремительность движений, сила, " +
                        "импульсивность, яркая выраженность эмоциональных переживаний. Вследствие неуравновешенности, увлекшись делом, склонен действовать изо " +
                        "всех сил, истощаться больше, чем следует. Имея общественные интересы, темперамент проявляет в инициативности, энергичности, " +
                        "принципиальности. При отсутствии духовной жизни холерический темперамент часто проявляется в раздражительности, эффективности, " +
                        "несдержанности, вспыльчивости, неспособности к самоконтролю при эмоциональных обстоятельствах.";
                    break;
                case PsychoType.Флегматик:
                    text.Text = "   Флегматик характеризуется сравнительно низким уровнем активности поведения, новые формы которого вырабатываются медленно, " +
                        "но являются стойкими. Обладает медлительностью и спокойствием в действиях, мимике и речи, ровностью, постоянством, глубиной чувств " +
                        "и настроений. Настойчивый и упорный «труженик жизни», он редко выходит из себя, не склонен к аффектам, рассчитав свои силы, доводит " +
                        "дело до конца, ровен в отношениях, в меру общителен, не любит попусту болтать. Экономит силы, попусту их не тратит. В зависимости " +
                        "от условий в одних случаях флегматик может характеризоваться «положительными» чертами — выдержкой, глубиной мыслей, постоянством, " +
                        "основательностью и т. д., в других — вялостью, безучастностью к окружающему, ленью и безволием, бедностью и слабостью эмоций, " +
                        "склонностью к выполнению одних лишь привычных действий.";
                    break;
                case PsychoType.Сангвиник:
                    text.Text = "   Сангвиник быстро приспосабливается к новым условиям, быстро сходится с людьми, общителен. Чувства легко возникают и " +
                        "сменяются, эмоциональные переживания, как правило, неглубоки. Мимика богатая, подвижная, выразительная. Несколько непоседлив, " +
                        "нуждается в новых впечатлениях, недостаточно регулирует свои импульсы, не умеет строго придерживаться выработанного распорядка, " +
                        "жизни, системы в работе. В связи с этим не может успешно выполнять дело, требующее равной затраты сил, длительного и методичного " +
                        "напряжения, усидчивости, устойчивости внимания, терпения. При отсутствии серьезных целей, глубоких мыслей, творческой деятельности " +
                        "вырабатываются поверхностность и непостоянство.";
                    break;
            };
            ThisGrid.SizeChanged += (s, e) => 
            {
                float plotSize;
                if (ActualWidth > ActualHeight)
                    plotSize = (float)(ActualHeight * 0.7);
                else
                    plotSize = (float)(ActualWidth * 0.7);
                title.FontSize = plotSize / 15; text.FontSize = plotSize / 30; 
            };


            return (title, text);
        }

        private WpfPlot Plot(StackPanel stackPanel, int extraversion, int neuroticism)
        {
            WpfPlot plot = new WpfPlot();

            plot.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            plot.RightClicked -= plot.DefaultRightClickEvent;
            plot.Configuration.MiddleClickAutoAxis = false;
            plot.Configuration.MiddleClickDragZoom = false;
            plot.Configuration.RightClickDragZoom = false;
            plot.Configuration.LeftClickDragPan= false;
            plot.Configuration.ScrollWheelZoom = false;
            plot.Configuration.DoubleClickBenchmark= false;


            plot.Plot.SetAxisLimits(0, 24, 0, 24);
            plot.Plot.XAxis.ManualTickSpacing(2);
            plot.Plot.YAxis.ManualTickSpacing(2);

            ArrowCoordinated arrow = plot.Plot.AddArrow(24, 12, 0, 12, 1, System.Drawing.Color.Black);
            plot.Plot.AddArrow(12, 24, 12, 0, 1, System.Drawing.Color.Black);
            Text text1 = plot.Plot.AddText("Меланхолик", 0, 24, 14, System.Drawing.Color.Gray);
            Text text2 = plot.Plot.AddText("Холерик",    0, 24, 14, System.Drawing.Color.Gray);
            Text text3 = plot.Plot.AddText("Флегматик",  0, 24, 14, System.Drawing.Color.Gray);
            Text text4 = plot.Plot.AddText("Сангвиник",  0, 24, 14, System.Drawing.Color.Gray);

            MarkerPlot highlightedPoint = plot.Plot.AddPoint(0, 0);
            highlightedPoint.Color = System.Drawing.Color.Red;
            highlightedPoint.MarkerSize = 10;
            highlightedPoint.MarkerShape = MarkerShape.openCircle;
            highlightedPoint.IsVisible = false;

            double[] xs = new double[] { extraversion };
            double[] ys = new double[] { neuroticism };

            ScatterPlot scatterPlot = plot.Plot.AddScatterPoints(xs, ys);
            scatterPlot.Color = System.Drawing.Color.Red;

            ThisGrid.SizeChanged += (s,e) =>
            {
                PlotResize(stackPanel, text1, text2, text3, text4, highlightedPoint, scatterPlot);
                plot.Refresh();
            };

            plot.MouseMove += (s, e) =>
            {
                (double mouseCoordX, double mouseCoordY) = plot.GetMouseCoordinates();
                double xyRatio = plot.Plot.XAxis.Dims.PxPerUnit / plot.Plot.YAxis.Dims.PxPerUnit;
                (double pointX, double pointY, int pointIndex) = scatterPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
                double diffX = Math.Abs(mouseCoordX - pointX);
                double diffY = Math.Abs(mouseCoordY - pointY);

                if (diffX < 1 && diffY < 1)
                {
                    highlightedPoint.X = pointX;
                    highlightedPoint.Y = pointY;
                    highlightedPoint.IsVisible = true;
                    plot.Refresh();
                }
                else if (highlightedPoint.IsVisible)
                {
                    highlightedPoint.X = 0;
                    highlightedPoint.Y = 0;
                    highlightedPoint.IsVisible = false;
                    plot.Refresh();
                }
            };

            PlotResize(stackPanel, text1, text2, text3, text4, highlightedPoint, scatterPlot);
            plot.Refresh();
            return plot;
        }

        private void PlotResize(StackPanel stackPanel, Text text1, Text text2, Text text3, Text text4, MarkerPlot highlightedPoint, ScatterPlot scatterPlot)
        {
            stackPanel.Margin = new Thickness(ActualWidth / 20, ActualWidth / 20, ActualWidth / 20, ActualWidth / 20);

            if (ActualWidth > ActualHeight)
            {
                stackPanel.Width = ActualHeight * 0.7;
            }
            else
            {
                stackPanel.Width = ActualWidth * 0.7;
            }
            text1.FontSize = text2.FontSize = text3.FontSize = text4.FontSize = (float)(stackPanel.Width / 30);
            float sheetX = (float)(stackPanel.Width - 68);
            float sheetY = (float)(stackPanel.Width - 65);
            TextOffset(text1, sheetX, 0, sheetY, 0);
            TextOffset(text2, sheetX, 0.5, sheetY, 0);
            TextOffset(text3, sheetX, 0, sheetY, 0.5);
            TextOffset(text4, sheetX, 0.5, sheetY, 0.5);

            scatterPlot.MarkerSize = (float)(stackPanel.Width / 100);
            highlightedPoint.MarkerSize = scatterPlot.MarkerSize + 5;
        }

        private static void TextOffset(Text text, float sheetX, double xMultiplier, float sheetY, double yMultiplier)
        {
            text.PixelOffsetX = (float)(sheetX/4 - text.FontSize * text.Label.Length * 0.632/2 + (sheetX*xMultiplier));
            text.PixelOffsetY = -(float)(sheetY/4 - text.FontSize * 2.1633333333333318/2 + (sheetY * yMultiplier));
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
