using System.Globalization;

namespace SafeLink.Converters;

/// <summary>
/// bool → Color konverter.
///
/// XAML da ishlatish:
/// ConverterParameter="TrueColor|FalseColor"
///
/// Misol:
/// Converter="{StaticResource BoolToColorConverter}" ConverterParameter="White|Transparent"
/// true  → White
/// false → Transparent
///
/// Nima uchun bu kerak?
/// XAML da if/else yo'q. bool qiymatiga qarab rang o'zgartirish uchun
/// shu converter ishlatiladi.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue) return Colors.Transparent;
        if (parameter is not string param) return Colors.Transparent;

        var parts = param.Split('|');
        if (parts.Length != 2) return Colors.Transparent;

        var colorName = boolValue ? parts[0] : parts[1];
        return Color.FromArgb(colorName.StartsWith('#') ? colorName : GetNamedColor(colorName));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();

    private static string GetNamedColor(string name) => name switch
    {
        "White" => "#FFFFFF",
        "Transparent" => "#00000000",
        "Red" => "#E11D48",
        "Green" => "#16A34A",
        _ => "#FFFFFF"
    };
}
