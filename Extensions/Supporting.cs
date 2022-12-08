using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoTestCourseProject.Extensions
{
    public static class Supporting
    {
        public static object testFrame { get; set; }
        public static bool testStarted { get; set; }

        public static TestClass CurrentTest { get; set; }
        public static int CurrentQuestion { get; set; }

        public static List<T> Shuffle<T>(List<T> list)
        {
            Random rand = new Random();

            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T temporal = list[j];
                list[j] = list[i];
                list[i] = temporal;
            }
            return list;
        }
    }
}
