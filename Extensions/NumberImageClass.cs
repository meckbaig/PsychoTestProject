using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychoTestProject.Extensions
{
    public class NumberImageClass
    {
        public double Angle { get; set; }
        public double WidthToWidthRatio { get; set; }
        public double HeightToHeightRatio { get; set; }
        public double XtoXRatio { get; set; }
        public double YtoYRatio { get; set; }

        public NumberImageClass(double angle, double wtow, double htoh, double xtox, double ytoy)
        {
            Angle = angle;
            WidthToWidthRatio = wtow;
            HeightToHeightRatio = htoh;
            XtoXRatio = xtox;
            YtoYRatio = ytoy;
        }
    }
}