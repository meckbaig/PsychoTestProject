using PsychoTestProject.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PsychoTestProject.View
{
    interface IEditorView
    {
        Thickness DeleteButtonThickness { get; set; }
        Command DeleteAnswerCommand { get; }
        Command AddVariableAnswerCommand { get; }
    }
}
