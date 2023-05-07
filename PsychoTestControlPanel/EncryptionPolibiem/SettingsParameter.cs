using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PsychoTestControlPanel
{
    public class SettingsParameter
    {
        public string Property { get; set; }
        public bool Value { get; set; }

        Properties.Settings prop = Properties.Settings.Default;

        public SettingsParameter(string name) 
        {
            Property = name;
            Value = (bool)prop[name];
        }
    }
}
