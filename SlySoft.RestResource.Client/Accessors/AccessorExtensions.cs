namespace SlySoft.RestResource.Client.Accessors;

public static class AccessorExtensions {
    /// <summary>
    /// Find a link in a resource
    /// </summary>
    /// <param name="clientResource">Resource to search</param>
    /// <param name="linkName">Name of the link to find= case insensitive</param>
    /// <returns>Link matching the name, if one exists</returns>
    public static Link? GetLink(this ClientResource clientResource, string linkName) {
        return clientResource.Links.FirstOrDefault(x => x.Name.Equals(linkName, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Find a parameter in a link
    /// </summary>
    /// <param name="link">link to search</param>
    /// <param name="parameterName">Name of the parameter to find- case insensitive</param>
    /// <returns>Parameter matching the name, if one exists</returns>
    public static LinkParameter? GetParameter(this Link link, string parameterName) {
        return link.Parameters.FirstOrDefault(x =>
            x.Name.Equals(parameterName, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Get the type of parameter this link supports (parameter, field)
    /// </summary>
    /// <param name="link">link with the parameter</param>
    /// <returns>Type of parameter this link supports (parameter, field)</returns>
    public static string GetParameterTypeName(this Link link) {
        return link.Verb.Equals("GET", StringComparison.CurrentCultureIgnoreCase) ? "parameter" : "field";
    }

    /// <summary>
    /// Get a list of parameters in a templated link href
    /// </summary>
    /// <param name="link">Link containing the parameters</param>
    /// <returns>List of parameters</returns>
    public static IEnumerable<string> GetParameters(this Link link) {
        var parameters = new List<string>();
        if (!link.Templated) {
            return parameters;
        }

        for (var index = 0; index < link.Href.Length; index++) {
            if (link.Href[index] != '{') {
                continue;
            }

            var closingBracketIndex = link.Href.IndexOf('}', index);
            if (closingBracketIndex < index) {
                continue;
            }

            var parameterStart = index + 1;
            var parameterEnd = closingBracketIndex - 1;
            parameters.Add(link.Href.Substring(parameterStart, parameterEnd - parameterStart + 1));
        }

        return parameters;
    }
}