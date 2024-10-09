using System.Collections;
using System.Xml;
using SlySoft.RestResource.Serializers.Utils;

namespace SlySoft.RestResource.Serializers;

public static class ToXmlExtensions {
    /// <summary>
    /// Create a resource, formatted as XML using HAL, with extensions to support expanded links.
    /// MIME type = application/slysoft.hal+xml
    /// </summary>
    /// <param name="resource">Resource that contains the data to represent as xml</param>
    /// <returns>XML text in a HAL format (with slysoft extensions)</returns>
    public static string ToSlySoftHalXml(this Resource resource) {
        using var stringWriter = new StringWriter();
        using (var xmlWriter = XmlWriter.Create(stringWriter)) {
            xmlWriter.WriteObject(resource, "self");
        }

        return stringWriter.ToString();
    }

    private static void WriteObject(this XmlWriter xmlWriter, object o, string name) {
        var resource = o as Resource;
        if (resource != null) {
            xmlWriter.WriteStartElement("resource");
            
            //if there's no URI and this is the root, we don't need to write "self"
            xmlWriter.WriteAttributeString("rel", name.ToCamelCase());
        } else {
            xmlWriter.WriteStartElement(name.ToCamelCase());
        }

        foreach (var property in o.GetType().GetProperties()) {
            if (property.Name.IsResourceProperty()) {
                continue;
            }

            var value = property.GetValue(o, null);
            xmlWriter.AddData(new KeyValuePair<string, object?>(property.Name.ToCamelCase(), value));
        }
        
        if (resource != null) {
            foreach (var link in resource.Links) {
                xmlWriter.AddLink(link);
            }
        }

        xmlWriter.WriteEndElement();
    }

    private static void AddData(this XmlWriter xmlWriter, KeyValuePair<string, object?> data) {
        if (data.Value == null) {
            return;
        }

        var dataType = data.Value.GetType();
        if (typeof(IEnumerable).IsAssignableFrom(dataType) && data.Value is not string) {
            xmlWriter.WriteStartElement(data.Key);
            foreach (var item in (IEnumerable)data.Value) {
                xmlWriter.AddData(new KeyValuePair<string,object?>("value", item));
            }
            xmlWriter.WriteEndElement();
        } else if (dataType.IsClass && data.Value is not string) {
            xmlWriter.WriteObject(data.Value, data.Key);
        } else {
            xmlWriter.WriteStartElement(data.Key);
            xmlWriter.WriteValue(data.Value);
            xmlWriter.WriteEndElement();
        }
    }
    private static void AddLink(this XmlWriter xmlWriter, Link link) {
        xmlWriter.WriteStartElement("link");
        xmlWriter.WriteAttributeString("rel", link.Name);
        xmlWriter.WriteAttributeString("href", link.Href);

        if (link.Templated) {
            xmlWriter.WriteAttributeString("templated", "true");
        }

        if (link.Verb != "GET") {
            xmlWriter.WriteAttributeString("verb", link.Verb);
        }
        
        if (link.Timeout != 0) {
            xmlWriter.WriteAttributeString("timeout", link.Timeout.ToString());
        }

        foreach (var linkParameter in link.Parameters) {
            xmlWriter.WriteStartElement(link.GetParameterTypeName());
            xmlWriter.WriteAttributeString("name", linkParameter.Name);

            if (!string.IsNullOrEmpty(linkParameter.Type)) {
                xmlWriter.WriteStartElement("type");
                xmlWriter.WriteValue(linkParameter.Type!);
                xmlWriter.WriteEndElement();
            }

            if (!string.IsNullOrEmpty(linkParameter.DefaultValue)) {
                xmlWriter.WriteStartElement("defaultValue");
                xmlWriter.WriteValue(linkParameter.DefaultValue!);
                xmlWriter.WriteEndElement();
            }

            if (linkParameter.ListOfValues.Any()) {
                xmlWriter.WriteStartElement("listOfValues");
                foreach (var value in linkParameter.ListOfValues) {
                    xmlWriter.WriteStartElement("value");
                    xmlWriter.WriteValue(value);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        xmlWriter.WriteEndElement();
    }

    private static void AddEmbedded(this XmlWriter xmlWriter, string name, object resourceObject) {
        switch (resourceObject) {
            case Resource resource:
                xmlWriter.WriteObject(resource, name);
                return;
            case IList<Resource> resourceList: {
                foreach (var resourceListItem in resourceList) {
                    xmlWriter.WriteObject(resourceListItem, name);
                }
                return;
            }
        }
    }
}