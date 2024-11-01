namespace SlySoft.RestResource.Utils;

public static class ObjectExtensions {
    public static string? DefaultValueToString(this object? defaultValue) {
        if (defaultValue == null) {
            return null;
        }

#if NET8_0_OR_GREATER
        if (defaultValue is DateOnly defaultDateOnly) {
            return defaultDateOnly.ToString("yyyy-MM-dd");
        }
            
        if (defaultValue is TimeOnly defaultTimeOnly) {
            return defaultTimeOnly.ToString("HH:mm:ss");
        }
#endif

        if (defaultValue is DateTime defaultValueDateTime) {
            return defaultValueDateTime.ToString("s");
        }
        
        return defaultValue.ToString();
    }
}