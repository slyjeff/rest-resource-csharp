﻿using System.Collections;
using System.Xml.Linq;

namespace SlySoft.RestResource.Hal;

public static class FromHalXmlExtensions {
    /// <summary>
    /// Populate a resource from an XML string in slysoft.hal+xml or hal+xml format
    /// </summary>
    /// <param name="resource">The resource to populate</param>
    /// <param name="xml">XML in slysoft.hal+xml format or hal+lmx format</param>
    /// <returns>The resource for ease of reference</returns>
    public static T FromSlySoftHalXml<T>(this T resource, string xml) where T : Resource, new() {
        return new T();
        //return resource.FromHalXml(XElement.Parse(xml));
    }

    /*private static T FromHalXml<T>(this T resource, XElement xElement) where T : Resource {
        resource.GetUri(xElement);

        resource.GetData(xElement);

        resource.GetLinks(xElement);

        resource.GetEmbedded(xElement);

        return resource;
    }

    private static void GetUri(this Resource resource, XElement xElement) {
        var hrefElement = xElement.Attributes().FirstOrDefault(x => x.Name == "href");
        if (hrefElement == null) {
            return;
        }

        resource.Uri = hrefElement.Value;
    }

    private static void GetData(this Resource resource, XContainer xElement) {
        PopulateFromXElement(resource.Data, xElement);
    }

    private static void PopulateFromXElement(this ObjectData objectData, XContainer xElement) {
        foreach (var element in xElement.Elements()) {
            if (element.Name.LocalName is "link" or "resource") {
                continue;
            }

            var children = element.Elements().ToList();
            if (children.Count > 1 && children.All(x => x.Name.LocalName == "value")) {
                objectData[element.Name.LocalName] = children.ToList();
                continue;
            }

            if (children.Any()) {
                var childObjectData = new ObjectData();
                childObjectData.PopulateFromXElement(element);
                objectData[element.Name.LocalName] = childObjectData;
                continue;
            }

            objectData[element.Name.LocalName] = element.Value;
        }
    }

    private static IEnumerable ToList(this List<XElement> xElements) {
        var firstValue = xElements.FirstOrDefault();
        if (firstValue == default) {
            return new List<object>();
        }

        if (!firstValue.Elements().Any()) {
            return xElements.Select(x => (object?)x.Value).ToList();
        }

        var list = new ListData();
        foreach (var childElement in xElements) {
            var objectData = new ObjectData();
            objectData.PopulateFromXElement(childElement);
            list.Add(objectData);
        }

        return list;
    }

    private static void GetLinks(this Resource resource, XContainer xElements) {
        foreach (var element in xElements.Elements()) {
            if (element.Name.LocalName != "link") {
                continue;
            }

            var name = element.Attribute("rel")?.Value;
            if (name == null) {
                continue;
            }

            var href = element.Attribute("href")?.Value;
            if (href == null) {
                continue;
            }

            var verb = element.Attribute("verb")?.Value ?? "GET";

            bool.TryParse(element.Attribute("templated")?.Value ?? "false", out var templated);

            int.TryParse(element.Attribute("timeout")?.Value ?? "0", out var timeout);

            var link = new Link(name, href, verb: verb, templated: templated, timeout: timeout);

            foreach (var linkParameter in element.Elements()) {
                link.GetLinkParameter(linkParameter);
            }

            resource.Links.Add(link);
        }
    }

    private static void GetLinkParameter(this Link link, XElement? linkParameterElement) {
        if (linkParameterElement?.Name.LocalName is not ("parameter" or "field")) {
            return;
        }

        var linkParameterName = linkParameterElement.Attribute("name")?.Value;
        if (linkParameterName == null) {
            return;
        }

        var linkParameter = new LinkParameter(linkParameterName);
        link.Parameters.Add(linkParameter);

        foreach (var linkParameterDataElement in linkParameterElement.Elements()) {
            switch (linkParameterDataElement.Name.LocalName) {
                case "defaultValue":
                    linkParameter.DefaultValue = linkParameterDataElement.Value;
                    break;
                case "type":
                    linkParameter.Type = linkParameterDataElement.Value;
                    break;
                case "listOfValues": {
                    foreach (var value in linkParameterDataElement.Elements()) {
                        linkParameter.ListOfValues.Add(value.Value);
                    }

                    break;
                }
            }
        }
    }

    private static void GetEmbedded(this Resource resource, XContainer xElements) {
        var resourceLists = new Dictionary<string, IList<Resource>>();
        foreach (var element in xElements.Elements()) {
            if (element.Name.LocalName != "resource") {
                continue;
            }

            var name = element.Attribute("rel")?.Value;
            if (string.IsNullOrEmpty(name)) {
                continue;
            }

            if (name != null && !resourceLists.ContainsKey(name)) {
                resourceLists[name] = new List<Resource>();
            }

            if (name != null) {
                resourceLists[name].Add(new Resource().FromHalXml(element));
            }
        }

        foreach (var resourceList in resourceLists) {
            if (resourceList.Value.Count == 1) {
                resource.EmbeddedResources[resourceList.Key] = resourceList.Value.First();
                continue;
            }
            resource.EmbeddedResources[resourceList.Key] = resourceList.Value;
        }
    }*/
}