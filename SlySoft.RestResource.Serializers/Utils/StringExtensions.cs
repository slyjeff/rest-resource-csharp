namespace SlySoft.RestResource.Serializers.Utils; 

internal static class StringExtensions {
    public static string ToCamelCase(this string value) {
        if (value.Length < 1) {
            return string.Empty;
        }

        return value.First().ToString().ToLower() + value.Substring(1);
    }
    
    public static bool IsResourceProperty(this string value) {
        return value is "Links";
    }
}