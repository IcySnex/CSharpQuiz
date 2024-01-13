using System;
using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace CSharpQuiz.Converter;

internal class BooleanToAppearanceConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        (bool)value ? ControlAppearance.Success : ControlAppearance.Danger;

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture) =>
        (ControlAppearance)value == ControlAppearance.Success;
}