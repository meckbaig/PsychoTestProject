using PsychoTestCourseProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PsychoTestCourseProject.Extensions
{
    public static class Supporting
    {

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
