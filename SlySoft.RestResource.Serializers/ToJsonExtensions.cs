using System.Collections;
using Newtonsoft.Json.Linq;
using SlySoft.RestResource.Serializers.Utils;

namespace SlySoft.RestResource.Serializers;

public static class ToJsonExtensions {
    /// <summary>
    /// Create a resource, formatted as JSON that contains links.
    /// MIME type = application/slysoft+json
    /// </summary>
    /// <param name="resource">Resource that contains the data to represent as json</param>
    /// <returns>JSON text in slysoft+json format</returns>
    public static string ToJson(this Resource resource) {
        return resource.CreateJObject().ToString();
    }

    private static JObject CreateJObject(this object obj) {
        var o = new JObject();

        var properties = obj.GetType().GetProperties();
        foreach (var property in properties) {
            if (property.Name.IsResourceProperty()) {
                continue;
            }
            
            o.AddData(new KeyValuePair<string, object?>(property.Name.ToCamelCase(), property.GetValue(obj)));
        }

        if (obj is not Resource resource) {
            return o;
        }
        
        foreach (var link in resource.Links) {
            o.AddLink(link);
        }

        return o;
    }
    
    private static void AddData(this JObject o, KeyValuePair<string, object?> data) {
        if (data.Value == null) {
            return;
        }
        
        switch (data.Value) {
            case string stringValue:
                o[data.Key] = stringValue;
                break;
            case IEnumerable enumerable:
                var array = new JArray();
                foreach (var item in enumerable) {
                    if (item is not string & item.GetType().IsClass) {
                        array.Add(item.CreateJObject());
                        continue;
                    }
                    array.Add(item);
                }
                o[data.Key] = array;
                break;
            default:
                if (data.Value.GetType().IsClass) {
                    o[data.Key] = data.Value.CreateJObject();
                    return;
                }

                o[data.Key] = new JValue(data.Value);
                break;
        }
    }    

    private static void AddLink(this JObject o, Link link) {
        if (!o.ContainsKey("_links")) {
            o["_links"] = new JObject();
        }

        var links = o["_links"];
        if (links == null) {
            return;
        }

        var linkObject = new JObject {
            ["href"] = link.Href
        };

        if (link.Templated) {
            linkObject["templated"] = true;
        }

        if (link.Verb != "GET") {
            linkObject["verb"] = link.Verb;
        }

        if (link.Timeout != 0) {
            linkObject["timeout"] = link.Timeout;
        }

        if (link.Parameters.Any()) {
            linkObject.AddLinkParameters(link);
        }

        links[link.Name] = linkObject;
    }

    private static void AddLinkParameters(this JObject linkObject, Link link) {
        var linkParameters = new JObject();
        foreach (var linkParameter in link.Parameters) {
            var linkParameterObject = new JObject();

            if (!string.IsNullOrEmpty(linkParameter.Type)) {
                linkParameterObject["type"] = linkParameter.Type;
            }

            if (!string.IsNullOrEmpty(linkParameter.DefaultValue)) {
                linkParameterObject["defaultValue"] = linkParameter.DefaultValue;
            }

            if (linkParameter.ListOfValues.Any()) {
                linkParameterObject["listOfValues"] = new JArray(linkParameter.ListOfValues);
            }

            linkParameters[linkParameter.Name] = linkParameterObject;
        }

        var linkParameterName = link.GetParameterTypeName() + "s";
        linkObject[linkParameterName] = linkParameters;
    }

    private static void AddEmbeddedResource(this JObject o, string name, object resourceObject) {
        if (!o.ContainsKey("_embedded")) {
            o["_embedded"] = new JObject();
        }

        var embedded = o["_embedded"];
        if (embedded == null) {
            return;
        }

        switch (resourceObject) {
            case Resource resource: {
                embedded[name] = resource.CreateJObject();
                return;
            }

            case IList<Resource> resourceList: {
                var jArray = new JArray();
                foreach (var resource in resourceList) {
                    jArray.Add(resource.CreateJObject());
                }
                embedded[name] = jArray;
                break;
            }
        }
    }
}