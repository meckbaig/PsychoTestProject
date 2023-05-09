using PsychoTestProject.Extensions;
using PsychoTestProject.View.TestKinds;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace PsychoTestProject.ViewModel
{
    class LeongardTestViewModel : MultiTestViewModel
    {
        private List<int[]> types = new List<int[]>()
        {
            new int[] {1,14,27,40,53,66,79,92},   // параноик.
            new int[] {2,15,28,41,54,57,80,93},   // эпилептоид.
            new int[] {3,19,29,42,55,68,81,94},   // гипертим.
            new int[] {4,17,30,43,56,69,82,95},   // истероид.
            new int[] {5,18,31,44,57,70,83,96},   // шизоид.
            new int[] {6,19,32,45,58,71,84,97},   // психастеноид.
            new int[] {7,20,33,46,59,72,85,98},   // сензитив.
            new int[] {8,21,34,47,60,73,86,99},   // гипотим.
            new int[] {9,22,35,48,61,74,87,100},  // конформный тип.
            new int[] {10,23,36,49,62,75,88,101}, // неустойчивый тип.
            new int[] {11,24,37,50,63,76,89,102}, // астеник.
            new int[] {12,25,38,51,64,77,90,103}, // лабильный тип.
            new int[] {13,26,39,52,65,78,91,104}  // циклоид.
        };

        private string[] Labels = new string[]
        {
            "параноик",
            "эпилептоид",
            "гипертим",
            "истероид",
            "шизоид",
            "психастеноид",
            "сензитив",
            "гипотим",
            "конформный тип",
            "неустойчивый тип",
            "астеник",
            "лабильный тип",
            "циклоид"
        };

        private Frame LeongardFrame { get; set; }
        private LeongardFact LeongardFact { get; set; } = new LeongardFact(0);
        private double BarBottomPoint { get; set; }
        private double[] Results { get; set; }

        int roundedX { get; set; }
        double barY { get; set; }

        WpfPlot plot { get; set; }

        public LeongardTestViewModel()
        {
            string testPath = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, $"Tests\\{MainViewModel.MainWindow.Title}"), "*.xml")[0];
            MainViewModel.CurrentTest = new TestClass(true)
            {
                Name = Path.GetFileNameWithoutExtension(testPath),
                Filename = testPath
            };
            MainViewModel.CurrentQuestionNumber = 1;

        }

        public override void PrintResults(Grid thisGrid, ScrollViewer scroll)
        {
            StackPanel stackPanel = new StackPanel()
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Orientation = System.Windows.Controls.Orientation.Vertical
            };
            CalculateResults();
            var results = new double[][]
            {
                Results
                //new double[]
                //{
                //    7,2,3,-4,5,6,6,-7,8,9,11,1,2
                //}
            };
            LeongardFrame = new Frame()
            {
                Visibility = Visibility.Hidden,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            };
            plot = Plot(thisGrid, stackPanel, results);


            TextBlock resultText = new TextBlock()
            {
                Text = "Наиболее выражены ",
                FontSize = 16,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 20, 0, 0)
            };
            thisGrid.SizeChanged += (s, e) =>
            {
                float plotSize;
                if (thisGrid.ActualWidth > thisGrid.ActualHeight)
                    plotSize = (float)(thisGrid.ActualHeight * 0.9);
                else
                    plotSize = (float)(thisGrid.ActualWidth * 0.9);
                resultText.FontSize = plotSize / 30;
            };

            List<double> res = results[0].ToList();
            
            for (int i = 0; i < 4; i++)
            {
                string typeFolder = Path.Combine(Environment.CurrentDirectory, $"Tests\\Тест «Акцентуации характера К. Леонгард»\\{res.IndexOf(res.Max())}");
                string file = Directory.GetFiles(typeFolder, "*.text")[0];
                resultText.Text += Path.GetFileNameWithoutExtension(file).ToLower().Replace(" тип", ", ");
                res[res.IndexOf(res.Max())] = 0;
            }
            resultText.Text = resultText.Text.Remove(resultText.Text.Length - 2) + " типы.";

            stackPanel.Children.Add(plot);
            thisGrid.Children.Add(LeongardFrame);
            stackPanel.Children.Add(resultText);
            scroll.Content = stackPanel;



            SizeChangedInfo sifo = new SizeChangedInfo(thisGrid, new System.Windows.Size(Double.NaN, Double.NaN), true, true);
            SizeChangedEventArgs ea = typeof(System.Windows.SizeChangedEventArgs).GetConstructors(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).FirstOrDefault().Invoke(
                new object[] { (thisGrid as FrameworkElement), sifo }) as SizeChangedEventArgs;
            ea.RoutedEvent = Panel.SizeChangedEvent;
            thisGrid.RaiseEvent(ea);
        }

        private WpfPlot Plot(Grid thisGrid, StackPanel stackPanel, double[][] results)
        {
            var errors0 = new double[][]
            {
                new double[]
                {
                    0,0,0,0,0,0,0,0,0,0,0,0,0
                }
            };

            WpfPlot plot = new WpfPlot();

            plot.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            plot.RightClicked -= plot.DefaultRightClickEvent;
            plot.Configuration.MiddleClickAutoAxis = false;
            plot.Configuration.MiddleClickDragZoom = false;
            plot.Configuration.RightClickDragZoom = false;
            plot.Configuration.LeftClickDragPan = false;
            plot.Configuration.ScrollWheelZoom = false;
            plot.Configuration.DoubleClickBenchmark = false;


            plot.Plot.AddHorizontalLine(10, System.Drawing.Color.Red);
            plot.Plot.AddHorizontalLine(-10, System.Drawing.Color.Red);
            plot.Plot.AddBarGroups(Labels, new string[] { "" }, results, errors0);
            plot.Plot.AddLine(-10, -16, -10, 16, System.Drawing.Color.Black);
            plot.Plot.SetAxisLimits(-0.5, 12.5, -16, 16);

            thisGrid.SizeChanged += (s, e) =>
            {
                FrameResize(thisGrid);
                PlotResize(stackPanel, thisGrid);
                plot.Refresh();
            };

            plot.MouseMove += (s, e) =>
            {
                MouseMove(thisGrid, results, plot);
            };

            LeongardFrame.MouseMove += (s, e) =>
            {
                MouseMove(thisGrid, results, plot);
            };

            plot.Plot.XAxis.TickLabelStyle(rotation: 45);
            plot.Refresh();
            return plot;
        }

        private void FrameResize(Grid thisGrid)
        {
            if (thisGrid.ActualHeight > thisGrid.ActualWidth)
            {
                LeongardFact.Width = thisGrid.ActualWidth / 1.7;
                LeongardFact.Height = LeongardFact.Width / 1.5;
            }
            else
            {
                LeongardFact.Height = thisGrid.ActualHeight / 2;
                LeongardFact.Width = LeongardFact.Height * 1.5;
            }
        }

        private void MouseMove(Grid thisGrid, double[][] results, WpfPlot plot)
        {
            double mcX, mcY;
            (mcX, mcY) = plot.GetMouseCoordinates();
            roundedX = (int)Math.Round(mcX, 0);
            if (mcX < roundedX + 0.33 && mcX > roundedX - 0.33 && roundedX < 13 && roundedX >= 0)
            {
                barY = results[0][roundedX];

                double pixel;
                if (barY < 0)
                    pixel = plot.Plot.GetPixelY(barY);
                else
                    pixel = plot.Plot.GetPixelY(0);
                BarBottomPoint = plot.PointToScreen(new Point() { Y = pixel, X = roundedX } ).Y;

                if (barY < 0 && mcY < 0 && mcY > barY && BarBottomPoint > thisGrid.PointToScreen(Mouse.GetPosition(thisGrid)).Y)
                {
                    ShowFunFrame(thisGrid, roundedX);
                }
                else if (barY > 0 && mcY > 0 && mcY < barY && BarBottomPoint > thisGrid.PointToScreen(Mouse.GetPosition(thisGrid)).Y)
                {
                    ShowFunFrame(thisGrid, roundedX);
                }
                else if (LeongardFrame.Visibility == Visibility.Visible)
                        LeongardFrame.Visibility = Visibility.Hidden;
            }
            else if (LeongardFrame.Visibility == Visibility.Visible)
                    LeongardFrame.Visibility = Visibility.Hidden;
        }

        private void ShowFunFrame(Grid thisGrid, int id)
        {
            Point position = Mouse.GetPosition(thisGrid);

            if (!LeongardFrame.IsVisible)
            {
                LeongardFact = new LeongardFact(id);
                LeongardFrame.Content = LeongardFact;
                LeongardFrame.Visibility = Visibility.Visible; 
                FrameResize(thisGrid);
            }
            else if (this.LeongardFact.ID != id)
            {
                LeongardFact = new LeongardFact(id);
                LeongardFrame.Content = LeongardFact;
                FrameResize(thisGrid);
            }
            double leftSpace = thisGrid.ActualWidth - Mouse.GetPosition(thisGrid).X - LeongardFact.ActualWidth;
            if (leftSpace > 0)
            {
                leftSpace = 0;
            }
            LeongardFrame.Margin = new Thickness(position.X + leftSpace, position.Y, 0, 0);

        }

        private void PlotResize(StackPanel stackPanel, Grid ThisGrid)
        {
            stackPanel.Margin = new Thickness(ThisGrid.ActualWidth / 20, ThisGrid.ActualHeight / 20, ThisGrid.ActualWidth / 20, ThisGrid.ActualHeight / 20);

            if (ThisGrid.ActualWidth > ThisGrid.ActualHeight)
            {
                stackPanel.Width = ThisGrid.ActualHeight * 0.8;
            }
            else
            {
                stackPanel.Width = ThisGrid.ActualWidth * 0.8;
            }
        }

        private void CalculateResults()
        {
            Results = new double[13];
            for (int i = 0; i < 13; i++)
            {
                foreach (int n in types[i])
                {
                    Results[i] += AnswersArray[n - 1];
                }
            }
        }
    }
}
