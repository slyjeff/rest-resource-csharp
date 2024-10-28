using System.Collections;
using Newtonsoft.Json.Linq;

namespace SlySoft.RestResource.Client.ResourceDeserializers;

public static class FromJsonExtensions {
    /// <summary>
    /// Populate a resource from a JSON string in slysoft.hal+json or hal+json format
    /// </summary>
    /// <param name="clientResource">The resource to populate</param>
    /// <param name="json">JSON slysoft+json format</param>
    /// <returns>The resource for ease of reference</returns>
    public static ClientResource FromJson(this ClientResource clientResource, string json) {
        return clientResource.FromJson(JObject.Parse(json));
    }

    private static ClientResource FromJson(this ClientResource clientResource, JObject o) {
        clientResource.GetUri(o);

        clientResource.GetData(o);

        clientResource.GetLinks(o);

        return clientResource;
    }

    private static void GetUri(this ClientResource clientResource, JObject o) {
        var links = o["_links"];
        if (links == null) {
            return;
        }

        if (links["self"] is not JObject self) {
            return;
        }

        var href = self["href"];
        if (href == null) {
            return;
        }

        clientResource.Uri = href.ToString();
    }

    private static void GetData(this ClientResource clientResource, JObject o) {
        clientResource.Data.PopulateFromJObject(o);
    }

    private static void PopulateFromJObject(this ObjectData objectData, JObject o) {
        foreach (var item in o) {
            if (item.Key is "_links") {
                continue;
            }

            switch (item.Value) {
                case JValue value:
                    objectData[item.Key] = value.Value;
                    break;
                case JArray jArray:
                    objectData[item.Key] = jArray.ToList();
                    break;
                case JObject jObject: {
                    if (jObject["_links"] != null) {
                        var clientResource = new ClientResource();
                        clientResource.FromJson(jObject);
                        objectData[item.Key] = clientResource;
                        continue;
                    }

                    var childObjectData = new ObjectData();
                    childObjectData.PopulateFromJObject(jObject);
                    objectData[item.Key] = childObjectData;
                    break;
                }
            }
        }
    }

    private static IEnumerable ToList(this JArray jArray) {
        var firstValue = jArray.FirstOrDefault();
        if (firstValue == default) {
            return new List<object>();
        }

        if (firstValue is JValue) {
            return jArray.OfType<JValue>().Select(x => x.Value).ToList();
        }

        if (firstValue["_links"] != null) {
            var resourceList = new ResourceListData();
            foreach (var item in jArray.OfType<JObject>()) {
                var clientResource = new ClientResource();
                clientResource.FromJson(item);
                resourceList.Add(clientResource);
            }

            return resourceList;
        }

        var list = new ListData();
        foreach (var item in jArray.OfType<JObject>()) {
            var objectData = new ObjectData();
            objectData.PopulateFromJObject(item);
            list.Add(objectData);
        }

        return list;
    }

    private static void GetLinks(this ClientResource clientResource, JObject o) {
        if (o["_links"] is not JObject links) {
            return;
        }

        foreach (var linkObject in links) {
            clientResource.GetLink(linkObject);
        }
    }

    private static void GetLink(this ClientResource clientResource, KeyValuePair<string, JToken?> linkObject) {
        if (linkObject.Key == "self") {
            return;
        }

        if (linkObject.Value is not JObject linkData) {
            return;
        }

        if (linkData["href"] is not JValue href) {
            return;
        }

        var verb = "GET";
        if (linkData["verb"] is JValue verbValue) {
            verb = verbValue.ToString();
        }

        var templated = linkData["templated"] != null;

        var timeout = 0;
        if (linkData["timeout"] is JValue timeoutValue) {
            timeout = Convert.ToInt32(timeoutValue.Value);
        }

        var link = new Link(linkObject.Key, href.ToString(), verb: verb, templated: templated, timeout: timeout);

        var linkParameters = linkData["parameters"] as JObject ?? linkData["fields"] as JObject;

        if (linkParameters != null) {
            foreach (var linkParameter in linkParameters) {
                link.GetLinkParameter(linkParameter);
            }
        }

        clientResource.Links.Add(link);
    }

    private static void GetLinkParameter(this Link link, KeyValuePair<string, JToken?> linkParameterKeyValuePair) {
        var linkParameter = new LinkParameter(linkParameterKeyValuePair.Key);
        link.Parameters.Add(linkParameter);

        if (linkParameterKeyValuePair.Value is not JObject linkParameterValue) {
            return;
        }

        if (linkParameterValue["defaultValue"] is JValue defaultValue) {
            linkParameter.DefaultValue = defaultValue.ToString();
        }

        if (linkParameterValue["type"] is JValue typeValue) {
            linkParameter.Type = typeValue.ToString();
        }

        if (linkParameterValue["listOfValues"] is JArray listOfValues) {
            foreach (var value in listOfValues) {
                linkParameter.ListOfValues.Add(value.ToString());
            }
        }
    }
}