using PsychoTestProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PsychoTestProject.Extensions
{
    class AnimationClass 
    {
        public static List<Button> Buttons;
        public static List<Button> TestButtons;
        public static List<double> MarginX;
        public static List<double> MarginY;
        public static List<double> Polarity;
        public AnimationClass(object pageContent, bool animationOpened, int duration)
        {
            double side = animationOpened? 1 : -1;
            Buttons = MainViewModel.GetVisualChilds<Button>(pageContent as DependencyObject);
            TestButtons = GetTestButtons();
            MarginX = new List<double>() { 56 * side, 28 * side };
            MarginY = new List<double>() { 0, 175 * side };
            Polarity = new List<double>() { 1, -1 };

            int i = 0;
            foreach (var y in MarginY)
            {
                if (y == 0)
                {
                    foreach (var p in Polarity)
                    {
                        Welcome.CreateAnimation(TestButtons[i], duration, y, MarginX[0] * p);
                        i++;
                    }
                }
                else
                    foreach (var p1 in Polarity)
                    {
                        foreach (var p2 in Polarity)
                        {
                            Welcome.CreateAnimation(TestButtons[i], duration, y * p1, MarginX[1] * p2);
                            i++;
                        }
                    }
            }
            Welcome.CreateAnimation(Buttons.FirstOrDefault(b => b.Name.Equals("LectionsButton")), duration, 0, 0, 130 * side);
        }

        private static List<Button> GetTestButtons()
        {
            List<Button> testButtons = new List<Button>();
            foreach (Button button in Buttons)
            {
                if (button.Name.ToString().Contains("TestButton"))
                {
                    testButtons.Add(button);
                }
            }
            return testButtons;
        }
    }
}
