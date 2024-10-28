namespace SlySoft.RestResource.Utils;

public static class ObjectExtensions {
    public static string? DefaultValueToString(this object? defaultValue) {
        if (defaultValue == null) {
            return null;
        }

        if (defaultValue is DateTime defaultValueDateTime) {
            return defaultValueDateTime.ToString("s");
        }

        return defaultValue.ToString();
    }
}