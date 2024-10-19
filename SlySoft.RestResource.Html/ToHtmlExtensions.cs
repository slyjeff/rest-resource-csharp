using System.Collections;
using System.Text;
using System.Web.UI;
using SlySoft.RestResource.Html.Utils;

namespace SlySoft.RestResource.Html;

public static class ToHtmlExtensions {
    /// <summary>
    /// Create a resource, formatted as HTML
    /// MIME type = text/html
    /// </summary>
    /// <param name="resource">Resource that contains the data to represent as HTML</param>
    /// <returns>HTML text</returns>
    public static string ToHtml(this Resource resource) {
        using var stringWriter = new StringWriter();
        using (var htmlWriter = new HtmlTextWriter(stringWriter)) {
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Html);

            WriteStyles(htmlWriter);

            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Body);

            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "container");
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);

            var scripts = new List<string>();

            htmlWriter.WriteResourceAsHtml(resource, scripts);

            WriteScripts(htmlWriter, scripts);

            htmlWriter.RenderEndTag(); //div
            htmlWriter.RenderEndTag(); //body
            htmlWriter.RenderEndTag(); //html

            htmlWriter.Close();
        }

        return stringWriter.ToString();
    }

    private const string Css =
        @"body{
    margin: 0;
    padding: 0;
    color: #333;
    background-color: #eee;
    font: 1em/1.2 'Helvetica Neue', Helvetica, Arial, Geneva, sans-serif;
}

h1,h2,h3 {
    margin: 0 0 .5em;
    font-weight: 500;
    line-height: 1.1;
}

h1 { font-size: 2.25em; }
h2 { font-size: 1.375em; }
h3 { font-size: 1.375em; background: lightgrey; padding: 0.25em }

p {
    margin: 0 0 1.5em;
    line-height: 1.5;
}

 

table {
    background-color: transparent;
    border-spacing: 0;
    border-collapse: collapse;
    border-top: 1px solid #ddd;
    width: 100%
}

th, td {
    padding: .5em 1em;
    vertical-align: center;
    text-align: left;
    border-bottom: 1px solid #ddd;
}

td:last-child {
    width:100%;
}


.btn {
    color: #fff !important;
    background-color: GRAY;
    border-color: #222;
    display: inline-block;
    padding: .5em 1em;
    font-weight: 400;
    line-height: 1.2;
    text-align: center;
    white-space: nowrap;
    vertical-align: middle;
    cursor: pointer;
    border: 1px solid transparent;
    border-radius: .2em;
    text-decoration: none;
}

.btn:hover {
    color: #fff !important;
    background-color: #555;
}

.btn:focus {
    color: #fff !important;
    background-color: #555;
}

.btn:active {
    color: #fff !important;
    background-color: #555;
}

.container {
    max-width: 70em;
    margin: 0 auto;
    background-color: #fff;
}

.header {
    color: #fff;
    background: #555;
    p
}

.subheader {
    color: #fff;
    background: #555;
    p
}

.header-heading { margin: 0; }

.content { padding: 1em 1.25em; }

.embedded { padding-left: 1.5em }

@media (min-width: 42em) {
    .header { padding: 1.5em 3em; }
    .subheader { padding: .2em 3em; }
    .content { padding: 2em 3em; }
}";

    private static void WriteStyles(HtmlTextWriter htmlWriter) {
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Style);
        htmlWriter.Write(Css);
        htmlWriter.RenderEndTag(); //style
    }

    private static void WriteHeader(HtmlTextWriter htmlWriter, Resource resource, bool isRoot) {
        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, isRoot ? "header" : "subheader");
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);

        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "header-heading");
        htmlWriter.RenderBeginTag(isRoot ? HtmlTextWriterTag.H1 : HtmlTextWriterTag.H2);

        htmlWriter.Write(resource.GetResourceName());
        htmlWriter.RenderEndTag(); // H1/H2

        htmlWriter.RenderEndTag(); //div
    }

    private static string GetResourceName(this Resource resource) {
        var selfLink = resource.GetLink("self");
        if (selfLink == null) {
            return "application";
        }
        
        if (selfLink.Href == "/") {
            return "application";
        }

        //find the last segment that isn't a number
        var segments = selfLink.Href.Split('/');
        foreach (var segment in segments.Reverse()) {
            if (!int.TryParse(segment, out var _)) {
                return segment;
            }
        }

        return selfLink.Href;
    }

    private static void WriteResourceAsHtml(this HtmlTextWriter htmlWriter, Resource resource, IList<string> scripts, bool isRoot = true) {
        WriteHeader(htmlWriter, resource, isRoot);

        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "content");
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
        htmlWriter.WriteSelfLink(resource);
        htmlWriter.WriteData(resource, scripts);
        htmlWriter.WriteLinks(resource, scripts);
        htmlWriter.RenderEndTag(); //div
    }

    private static void WriteSelfLink(this HtmlTextWriter htmlWriter, Resource resource) {
        var selfLink = resource.GetLink("self");
        if (selfLink == null) {
            return;
        }
        
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);

        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, selfLink.Href);
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
        htmlWriter.Write(selfLink.Href);
        htmlWriter.RenderEndTag(); //a

        htmlWriter.RenderEndTag(); //div

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Br);
        htmlWriter.RenderEndTag(); //br
    }

    private static void WriteData(this HtmlTextWriter htmlWriter, object o, IList<string> scripts) {
        var properties = o.GetType().GetProperties().Where(p => !p.Name.IsResourceProperty()).ToList();
        
        if (!properties.Any()) {
            return;
        }

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Table);

        foreach (var property in properties) {
            var name = property.Name.ToLower();
            var value = property.GetValue(o);
            
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);

            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlWriter.Write(name);
            htmlWriter.RenderEndTag(); //td

            switch (value) {
                case string stringValue:
                    htmlWriter.WriteValue(stringValue, scripts);
                    break;
                case IEnumerable enumerable:
                    htmlWriter.WriteEnumerable(enumerable, scripts);
                    break;
                default:
                    htmlWriter.WriteValue(value, scripts);
                    break;
            }
            
            htmlWriter.RenderEndTag(); //tr
        }

        htmlWriter.RenderEndTag(); //table
    }

    private static void WriteEnumerable(this HtmlTextWriter htmlWriter, IEnumerable enumerable, IList<string> scripts) {
        var list = enumerable.Cast<object>().ToList();
        var firstItem = list.FirstOrDefault();
        if (firstItem == null) {
            return;
        }
        
        if (firstItem is string || !firstItem.GetType().IsClass)  {
            var values = (from object item in list select item.ToString()).ToList();
            htmlWriter.WriteValue(string.Join(", ", values), scripts);
            return;
        }

        foreach (var item in list) {
            htmlWriter.WriteData(item, scripts);
        }
    }

    private static void WriteValue(this HtmlTextWriter htmlWriter, object? value, IList<string> scripts) {
        if (value == null) {
            return;
        }
        
        if (value is Resource resource) {
            htmlWriter.WriteResourceAsHtml(resource, scripts, isRoot: false);
            return;
        }

        if (value is string stringValue) {
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlWriter.Write(stringValue);
            htmlWriter.RenderEndTag(); //td
            return;
        }
        
        if (value.GetType().IsClass) {
            htmlWriter.WriteData(value, scripts);
            return;
        }
        
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
        htmlWriter.Write(value);
        htmlWriter.RenderEndTag(); //td
    }

    private static void WriteLinks(this HtmlTextWriter htmlWriter, Resource resource, ICollection<string> scripts) {
        var linkCount = resource.Links.Count; 
        if (resource.GetLink("self") != null) {
            linkCount--;
        }
        
        if (linkCount == 0) {
            return;
        }

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Br);
        htmlWriter.RenderEndTag(); //br

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.H3);
        htmlWriter.Write("Actions");
        htmlWriter.RenderEndTag(); //h3

        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "embedded");

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Div);
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Table);

        foreach (var link in resource.Links) {
            if (link.Name == "self") {
                continue;
            }
            
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);

            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlWriter.Write(link.Name);
            htmlWriter.RenderEndTag(); //td

            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);

            var templatedParameters = link.GetParameters().ToList();

            switch (link.Verb) {
                case "GET" when link.Parameters.Count > 0: {
                    var href = link.Href;

                    htmlWriter.AddAttribute("action", href);
                    htmlWriter.AddAttribute("method", "GET");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Form);

                    foreach (var templatedParameter in templatedParameters) {
                        WriterParameter(htmlWriter, new LinkParameter(templatedParameter));
                    }

                    foreach (var queryParameter in link.Parameters) {
                        WriterParameter(htmlWriter, queryParameter);
                    }

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "btn");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Value, "Query");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                    htmlWriter.RenderEndTag(); //input

                    htmlWriter.RenderEndTag(); //form
                    break;
                } case "GET" when templatedParameters.Any(): {
                    var beforeAndAfter = link.Href.GetTextBeforeAndAfterParameter(templatedParameters.First());
                    var href = beforeAndAfter.Before;

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Id, link.Name + "_link");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, href);
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                    htmlWriter.Write(href);
                    htmlWriter.RenderEndTag(); //a

                    var parameterInfoList = new List<UrlParameterInfo>();
                    foreach (var parameter in templatedParameters) {
                        htmlWriter.Write("&nbsp;");
                        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Id, $"{link.Name}_{parameter}_input");
                        htmlWriter.AddAttribute("placeholder", parameter);
                        htmlWriter.AddAttribute("size", "5");
                        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                        htmlWriter.RenderEndTag(); //input

                        if (parameter == templatedParameters.Last()) {
                            parameterInfoList.Add(new UrlParameterInfo(parameter, beforeAndAfter.After));
                            continue;
                        }

                        var nextParameter = templatedParameters[templatedParameters.IndexOf(parameter) + 1];
                        beforeAndAfter = beforeAndAfter.After.GetTextBeforeAndAfterParameter(nextParameter);
                        htmlWriter.Write("&nbsp;");
                        htmlWriter.Write(beforeAndAfter.Before);
                        parameterInfoList.Add(new UrlParameterInfo(parameter, beforeAndAfter.Before));
                    }

                    scripts.Add(CreateUpdateLinkScript(link.Name, href, parameterInfoList));
                    break;
                }
                case "GET":
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, link.Href);
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                    htmlWriter.Write(link.Href);
                    htmlWriter.RenderEndTag(); //a
                    break;
                case "POST" or "PUT" or "PATCH": {
                    htmlWriter.AddAttribute("action", link.Href);
                    htmlWriter.AddAttribute("method", "POST");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Form);

                    foreach (var formField in link.Parameters) {
                        WriterParameter(htmlWriter, formField);
                    }

                    switch (link.Verb) {
                        case "PUT":
                            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Name, "_isPut");
                            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                            htmlWriter.RenderEndTag(); //input
                            break;
                        case "PATCH":
                            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Name, "_isPatch");
                            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                            htmlWriter.RenderEndTag(); //input
                            break;
                    }

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "btn");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Value, link.Verb == "PUT" ? "Put" : "Post");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                    htmlWriter.RenderEndTag(); //input

                    htmlWriter.RenderEndTag(); //form
                    break;
                }
                case "DELETE":
                    htmlWriter.AddAttribute("action", link.Href);
                    htmlWriter.AddAttribute("method", "POST");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Form);

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Name, "_isDelete");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                    htmlWriter.RenderEndTag(); //input

                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, "btn");
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Value, "Delete");
                    htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.MarginTop, "1em");
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
                    htmlWriter.RenderEndTag(); //input

                    htmlWriter.RenderEndTag(); //form
                    break;
            }
            
            htmlWriter.RenderEndTag(); //td
            htmlWriter.RenderEndTag(); //tr
        }

        htmlWriter.RenderEndTag(); //table
        htmlWriter.RenderEndTag(); //div
    }

    private static void WriterParameter(HtmlTextWriter htmlWriter, LinkParameter parameter) {
        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Name, parameter.Name);
        htmlWriter.AddAttribute("placeholder", parameter.Name);

        if (!string.IsNullOrEmpty(parameter.DefaultValue)) {
            htmlWriter.AddAttribute("value", parameter.DefaultValue);
        }

        if (parameter.ListOfValues.Any()) {
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Select);
            foreach (var value in parameter.ListOfValues) {
                htmlWriter.AddAttribute("value", value);
                if (value == parameter.DefaultValue) {
                    htmlWriter.AddAttribute("selected", null);
                }
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Option);
                htmlWriter.Write(value);
                htmlWriter.RenderEndTag(); //option
            }

            htmlWriter.RenderEndTag(); //select
        } else {
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Input);
            htmlWriter.RenderEndTag(); //input
        }

        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Br);
        htmlWriter.RenderEndTag(); //br
    }

    private static void WriteScripts(HtmlTextWriter htmlWriter, List<string> scripts) {
        htmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
        
        htmlWriter.RenderBeginTag(HtmlTextWriterTag.Script);
        
        foreach (var script in scripts) {
            htmlWriter.WriteLine(script);
        }
        
        htmlWriter.RenderEndTag(); //script
    }

    private const string GetLink = "var {link}_link= document.getElementById('{link}_link');";

    private const string GetParameterValue = " + encodeURIComponent({link}_{parameter}_input.value)";

    private const string HookInput =
        @"var {link}_{parameter}_input= document.getElementById('{link}_{parameter}_input');
              {link}_{parameter}_input.onchange={link}_{parameter}_input.onkeyup= function() {
                  {link}_link.href = '{href}'{getParametersText};
              };";

    private class UrlParameterInfo {
        public UrlParameterInfo(string name, string afterText) {
            Name = name;
            AfterText = afterText;
        }

        public string Name { get; }
        public string AfterText { get; }
    }

    private static string CreateUpdateLinkScript(string linkName, string href, IList<UrlParameterInfo> parameters) {
        var scriptBuilder = new StringBuilder();
        scriptBuilder.AppendLine(GetLink.Replace("{link}", linkName));

        var getParameterValuesText = CreateGetParameterValuesText(linkName, parameters);

        var hookInputBaseText = HookInput
            .Replace("{link}", linkName)
            .Replace("{href}", href);
        foreach (var parameter in parameters) {
            var hookInput = hookInputBaseText
                .Replace("{parameter}", parameter.Name)
                .Replace("{getParametersText}", getParameterValuesText);
            scriptBuilder.AppendLine(hookInput);
        }

        var script = scriptBuilder.ToString();

        return script;
    }

    private static string CreateGetParameterValuesText(string linkName, IEnumerable<UrlParameterInfo> parameters) {
        var getParameterValueBaseText = GetParameterValue.Replace("{link}", linkName);

        var parameterBuilder = new StringBuilder();
        foreach (var parameter in parameters) {
            var getParameterValueText = getParameterValueBaseText
                .Replace("{parameter}", parameter.Name);

            if (!string.IsNullOrEmpty(parameter.AfterText)) {
                getParameterValueText += $" + '{parameter.AfterText}'";
            }

            parameterBuilder.Append(getParameterValueText);
        }

        return parameterBuilder.ToString();
    }
}
