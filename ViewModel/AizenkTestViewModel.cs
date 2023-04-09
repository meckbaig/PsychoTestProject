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

namespace PsychoTestProject.ViewModel
{
    class AizenkTestViewModel : INotifyPropertyChanged
    {
        public int[] AnswersArray { get; set; }
        public int[] indexAPos = new int[] { 6, 24, 36 };
        public int[] indexANeg = new int[] { 12, 18, 30, 42, 48, 54 };
        public int[] indexBPos = new int[] { 1, 3, 8, 10, 13, 17, 22, 25, 27, 39, 44, 46, 49, 53, 56 };
        public int[] indexBNeg = new int[] { 5, 15, 20, 29, 32, 34, 37, 41, 51 };
        public int[] indexCPos = new int[] { 2, 4, 7, 9, 11, 14, 16, 19, 21, 23, 26, 28, 31, 33, 35, 38, 40, 43, 45, 47, 50, 52, 55, 57 };
        public FunFact FunFact { get; set; }
        public Frame FunFrame { get; set; }

        public AizenkTestViewModel()
        {
            MainViewModel.CurrentQuestionNumber = 1;
        }
        public Thickness Margin { get; set; }


        public QuestionClass CurrentQuestion
        {
            get => MainViewModel.CurrentTest.Questions[MainViewModel.CurrentQuestionNumber - 1];
        }


        public QuestionClass NextQuestion()
        {
            if (MainViewModel.CurrentQuestionNumber == (MainViewModel.CurrentTest.Questions.Count))
            {
                return null;
            }
            return MainViewModel.CurrentTest.Questions[MainViewModel.CurrentQuestionNumber++];
        }
        public QuestionClass PreviousQuestion()
        {
            if (MainViewModel.CurrentQuestionNumber == 1)
            {
                return null;
            }
            return MainViewModel.CurrentTest.Questions[MainViewModel.CurrentQuestionNumber--];
        }

        public WpfPlot Plot(StackPanel stackPanel, int extraversion, int neuroticism, Grid ThisGrid)
        {
            WpfPlot plot = new WpfPlot();

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
                PlotResize(stackPanel, text1, text2, text3, text4, highlightedPoint, scatterPlot, ThisGrid);
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

                if (highlightedPoint.IsVisible)
                {
                    Point position = Mouse.GetPosition(ThisGrid);
                    if (!FunFrame.IsVisible)
                    {
                        FunFrame.Visibility = Visibility.Visible;
                    }
                    FunFrame.Margin = new Thickness(position.X, position.Y, 0, 0);
                }
                else
                {
                    if (FunFrame.IsVisible)
                    {
                        FunFrame.Visibility = Visibility.Hidden;
                    }
                }

            };


            PlotResize(stackPanel, text1, text2, text3, text4, highlightedPoint, scatterPlot, ThisGrid);
            plot.Refresh();
            return plot;
        }

        private void PlotResize(StackPanel stackPanel, Text text1, Text text2, Text text3, Text text4, MarkerPlot highlightedPoint, ScatterPlot scatterPlot, Grid ThisGrid)
        {
            stackPanel.Margin = new Thickness(ThisGrid.ActualWidth / 20, ThisGrid.ActualWidth / 20, ThisGrid.ActualWidth / 20, ThisGrid.ActualWidth / 20);

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

        public int CalculateIndicator(int[] posIndicators, int[] negIndicators = null)
        {
            int result = 0;
            foreach (int i in posIndicators)
            {
                if (AnswersArray[i-1] == 1)
                    result++;
            }
            if (negIndicators != null)
            {
                foreach (int i in negIndicators)
                {
                    if (AnswersArray[i-1] == 1)
                        result--;
                }
            }
            if (result < 0) return 0;
            return result;
        }

        public void ChangeMargin(double width, double height)
        {
            Margin = new Thickness(width / 10, height / 15, width / 10, height / 15);
            OnPropertyChanged("Margin");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
