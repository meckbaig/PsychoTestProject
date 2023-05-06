using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.IO;

namespace PsychoTestControlPanel
{
    internal class MainViewModel
    {
        public static Frame? MainFrame { get; set; }
        public MainViewModel(Frame mainFrame)
        {
            MainFrame = mainFrame;
        }

        public static void MouseHover(Button button)
        {
            System.Windows.Media.Brush MouseLeave = button.Background;
            System.Windows.Media.Color bgcolor = (button.Background as System.Windows.Media.SolidColorBrush).Color;
            if (bgcolor.A < 10)
                bgcolor.A = 10;
            else
            {
                if (bgcolor.R < 50)
                    bgcolor.R = 50;
                if (bgcolor.G < 50)
                    bgcolor.G = 50;
                if (bgcolor.B < 50)
                    bgcolor.B = 50;
            }
            System.Windows.Media.Brush MouseEnter = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(255, (byte)(bgcolor.R - 50), (byte)(bgcolor.G - 50), (byte)(bgcolor.B - 50)));

            button.MouseEnter += (s, e) =>
            {
                if (button.IsEnabled)
                    button.Background = MouseEnter;
            };
            button.MouseLeave += (s, e) =>
            {
                if (button.IsEnabled)
                    button.Background = MouseLeave;
            };
            button.IsEnabledChanged += (s, e) =>
            {
                if (button.IsEnabled)
                    button.Background = MouseLeave;
                else
                    button.Background = MouseEnter;
            };
        }

        public static void AllButtonsHover(object pageContent)
        {
            foreach (Button button in GetVisualChilds<Button>(pageContent as DependencyObject))
            {
                if (button.Background.GetType() != new ImageBrush().GetType())
                {
                    MouseHover(button);
                }
            }
        }

        public static List<T> GetVisualChilds<T>(DependencyObject parent) where T : DependencyObject
        {
            List<T> childs = new List<T>();
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(parent, i);
                if (v is T)
                    childs.Add(v as T);
                childs.AddRange(GetVisualChilds<T>(v));
            }
            return childs;
        }
    }
}
