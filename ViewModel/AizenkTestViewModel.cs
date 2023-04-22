using PsychoTestProject.Extensions;
using PsychoTestProject.View.TestKinds;
using ScottPlot.Plottable;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace PsychoTestProject.ViewModel
{
    class AizenkTestViewModel : MultiTestViewModel
    {
        private int[] indexAPos = new int[] { 6, 24, 36 };
        private int[] indexANeg = new int[] { 12, 18, 30, 42, 48, 54 };
        private int[] indexBPos = new int[] { 1, 3, 8, 10, 13, 17, 22, 25, 27, 39, 44, 46, 49, 53, 56 };
        private int[] indexBNeg = new int[] { 5, 15, 20, 29, 32, 34, 37, 41, 51 };
        private int[] indexCPos = new int[] { 2, 4, 7, 9, 11, 14, 16, 19, 21, 23, 26, 28, 31, 33, 35, 38, 40, 43, 45, 47, 50, 52, 55, 57 };

        private FunFact FunFact { get; set; }
        private Frame FunFrame { get; set; }
        private double PxPerUnit { get => (plot.Plot.XAxis.Dims.PxPerUnit + plot.Plot.YAxis.Dims.PxPerUnit) / 2; }
        private Point hlPoint { get; set; }
        private double pointHoverTarget = 0.5;
        WpfPlot plot { get; set; }

        public AizenkTestViewModel() : base()
        {
            Title = "Тест айзенка";
            string testPath = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, $"Tests\\{Title}"), "*.xml")[0];
            MainViewModel.CurrentTest = new TestClass(true) 
            { 
                Name = Path.GetFileNameWithoutExtension(testPath), 
                Filename = testPath 
            };
            MainViewModel.CurrentQuestionNumber = 1;
        }


        public override void PrintResults(Grid ThisGrid, ScrollViewer Scroll)
        {
            if (CalculateIndicator(indexAPos, indexANeg) <= 4)
            {
                int extraversion = CalculateIndicator(indexBPos, indexBNeg);
                int neuroticism = CalculateIndicator(indexCPos);
                Random random = new Random();
                //int extraversion = random.Next(0, 24);
                //int neuroticism = random.Next(0, 24);
                string type = ExecuteType(extraversion, neuroticism);

                StackPanel stackPanel = new StackPanel() { VerticalAlignment = System.Windows.VerticalAlignment.Top, Orientation = System.Windows.Controls.Orientation.Vertical };

                (TextBlock titleText, TextBlock textText, List<string> types) = CreateTitle(type, ThisGrid);
                FunFact = new FunFact(types);
                FunFrame = new Frame()
                {
                    Content = FunFact,
                    Visibility = Visibility.Hidden,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                };

                FunFact.MouseMove += (s, e) =>
                {
                    Point point = Mouse.GetPosition(ThisGrid);
                    if (point.X < hlPoint.X + PxPerUnit * pointHoverTarget && point.Y < hlPoint.Y + PxPerUnit * pointHoverTarget)
                    {
                        double bottomSpace = ThisGrid.ActualHeight - point.Y - FunFact.ActualHeight;
                        if (bottomSpace > 0)
                        {
                            bottomSpace = 0;
                        }
                        FunFrame.Margin = new Thickness(point.X, point.Y + bottomSpace, 0, 0);

                    }
                    else
                        FunFrame.Visibility = Visibility.Hidden;

                };

                ThisGrid.SizeChanged += (s, e) =>
                {
                    if (ThisGrid.ActualHeight > ThisGrid.ActualWidth)
                    {
                        FunFact.Width = ThisGrid.ActualWidth / 3;
                        FunFact.Height = FunFact.Width * 1.5;
                    }
                    else
                    {
                        FunFact.Height = ThisGrid.ActualHeight / 1.7;
                        FunFact.Width = FunFact.Height / 1.5;
                    }

                };
                WpfPlot plot = Plot(stackPanel, extraversion, neuroticism, ThisGrid);

                stackPanel.Children.Add(titleText);
                stackPanel.Children.Add(plot);
                stackPanel.Children.Add(textText);
                ThisGrid.Children.Add(FunFrame);


                Scroll.Content = stackPanel;



                SizeChangedInfo sifo = new SizeChangedInfo(ThisGrid, new Size(Double.NaN, Double.NaN), true, true);
                SizeChangedEventArgs ea = typeof(System.Windows.SizeChangedEventArgs).GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).FirstOrDefault().Invoke(new object[] { (ThisGrid as FrameworkElement), sifo }) as SizeChangedEventArgs;
                ea.RoutedEvent = Panel.SizeChangedEvent;
                ThisGrid.RaiseEvent(ea);

            }

            else MessageBox.Show("Error 234-56:98");
        }


        private (TextBlock title, TextBlock text, List<string> types) CreateTitle(string type, Grid ThisGrid)
        {
            TextBlock title = new TextBlock()
            {
                Text = type,
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                TextAlignment = System.Windows.TextAlignment.Center,
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
            List<string> types = TypesDescription(type, text);
            ThisGrid.SizeChanged += (s, e) =>
            {
                float plotSize;
                if (ThisGrid.ActualWidth > ThisGrid.ActualHeight)
                    plotSize = (float)(ThisGrid.ActualHeight * 0.7);
                else
                    plotSize = (float)(ThisGrid.ActualWidth * 0.7);
                title.FontSize = plotSize / 15; text.FontSize = plotSize / 30;
            };


            return (title, text, types);
        }

        private WpfPlot Plot(StackPanel stackPanel, int extraversion, int neuroticism, Grid ThisGrid)
        {
            plot = new WpfPlot();

            plot.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            plot.RightClicked -= plot.DefaultRightClickEvent;
            plot.Configuration.MiddleClickAutoAxis = false;
            plot.Configuration.MiddleClickDragZoom = false;
            plot.Configuration.RightClickDragZoom = false;
            plot.Configuration.LeftClickDragPan = false;
            plot.Configuration.ScrollWheelZoom = false;
            plot.Configuration.DoubleClickBenchmark = false;


            plot.Plot.SetAxisLimits(0, 24, 0, 24);
            plot.Plot.XAxis.ManualTickSpacing(2);
            plot.Plot.YAxis.ManualTickSpacing(2);

            ArrowCoordinated arrow = plot.Plot.AddArrow(24, 12, 0, 12, 1, System.Drawing.Color.Black);
            plot.Plot.AddArrow(12, 24, 12, 0, 1, System.Drawing.Color.Black);
            Text text1 = plot.Plot.AddText("Меланхолик", 0, 24, 14, System.Drawing.Color.Gray);
            Text text2 = plot.Plot.AddText("Холерик", 0, 24, 14, System.Drawing.Color.Gray);
            Text text3 = plot.Plot.AddText("Флегматик", 0, 24, 14, System.Drawing.Color.Gray);
            Text text4 = plot.Plot.AddText("Сангвиник", 0, 24, 14, System.Drawing.Color.Gray);

            MarkerPlot highlightedPoint = plot.Plot.AddPoint(0, 0);
            highlightedPoint.Color = System.Drawing.Color.Red;
            highlightedPoint.MarkerSize = 10;
            highlightedPoint.MarkerShape = MarkerShape.openCircle;
            highlightedPoint.IsVisible = false;

            double[] xs = new double[] { extraversion };
            double[] ys = new double[] { neuroticism };

            ScatterPlot scatterPlot = plot.Plot.AddScatterPoints(xs, ys);
            scatterPlot.Color = System.Drawing.Color.Red;

            ThisGrid.SizeChanged += (s, e) =>
            {
                PlotResize(stackPanel, text1, text2, text3, text4, highlightedPoint, scatterPlot, ThisGrid, plot);
                plot.Refresh();
            };

            plot.MouseMove += (s, e) =>
            {
                (double mouseCoordX, double mouseCoordY) = plot.GetMouseCoordinates();
                double xyRatio = plot.Plot.XAxis.Dims.PxPerUnit / plot.Plot.YAxis.Dims.PxPerUnit;
                (double pointX, double pointY, int pointIndex) = scatterPlot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
                double diffX = mouseCoordX - pointX;
                double diffY = mouseCoordY - pointY;
                hlPoint = new Point()
                {
                    X = Mouse.GetPosition(ThisGrid).X - diffX * plot.Plot.XAxis.Dims.PxPerUnit,
                    Y = Mouse.GetPosition(ThisGrid).Y - diffY * plot.Plot.YAxis.Dims.PxPerUnit
                };

                if (Math.Abs(diffX) < pointHoverTarget && Math.Abs(diffY) < pointHoverTarget)
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

                if (highlightedPoint.IsVisible)
                {
                    Point position = Mouse.GetPosition(ThisGrid);
                    if (!FunFrame.IsVisible)
                    {
                        FunFrame.Visibility = Visibility.Visible;
                    }
                    double bottomSpace = ThisGrid.ActualHeight - Mouse.GetPosition(ThisGrid).Y - FunFact.ActualHeight;
                    if (bottomSpace > 0)
                    {
                        bottomSpace = 0;
                    }
                    FunFrame.Margin = new Thickness(position.X, position.Y + bottomSpace, 0, 0);
                }
                else
                {
                    if (FunFrame.IsVisible)
                    {
                        FunFrame.Visibility = Visibility.Hidden;
                    }
                }

            };

            PlotResize(stackPanel, text1, text2, text3, text4, highlightedPoint, scatterPlot, ThisGrid, plot);
            plot.Refresh();
            return plot;
        }

        private void PlotResize(StackPanel stackPanel, Text text1, Text text2, Text text3, Text text4, MarkerPlot highlightedPoint, ScatterPlot scatterPlot, Grid ThisGrid, WpfPlot plot)
        {
            stackPanel.Margin = new Thickness(ThisGrid.ActualWidth / 20, ThisGrid.ActualHeight / 20, ThisGrid.ActualWidth / 20, ThisGrid.ActualHeight / 20);

            if (ThisGrid.ActualWidth > ThisGrid.ActualHeight)
            {
                stackPanel.Width = ThisGrid.ActualHeight * 0.7;
            }
            else
            {
                stackPanel.Width = ThisGrid.ActualWidth * 0.7;
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
            text.PixelOffsetX = (float)(sheetX / 4 - text.FontSize * text.Label.Length * 0.632 / 2 + (sheetX * xMultiplier));
            text.PixelOffsetY = -(float)(sheetY / 4 - text.FontSize * 2.1633333333333318 / 2 + (sheetY * yMultiplier));
        }

        private int CalculateIndicator(int[] posIndicators, int[] negIndicators = null)
        {
            int result = 0;
            foreach (int i in posIndicators)
            {
                if (AnswersArray[i - 1] == 1)
                    result++;
            }
            if (negIndicators != null)
            {
                foreach (int i in negIndicators)
                {
                    if (AnswersArray[i - 1] == 1)
                        result--;
                }
            }
            if (result < 0) return 0;
            return result;
        }

        private string ExecuteType(int extraversion, int neuroticism)
        {
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

            return type;
        }

        private List<string> TypesDescription(string type, TextBlock text)
        {
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

            return types;
        }
    }
}
