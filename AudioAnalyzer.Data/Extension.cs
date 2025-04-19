using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AudioAnalyzer.Data;

public static class Extension
{
    public static string GetNameAttribute(this Enum enumValue)
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            .GetName();
    }
}
