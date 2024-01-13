using System;
using System.Globalization;
using System.IO;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace MetaRename.ViewModels.Converters;

public class UriToExtensionConverter : IValueConverter
{
    public static readonly UriToExtensionConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is string sourceText && targetType.IsAssignableTo(typeof(string))) {
            return Path.GetExtension(sourceText);
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return value;
    }
}