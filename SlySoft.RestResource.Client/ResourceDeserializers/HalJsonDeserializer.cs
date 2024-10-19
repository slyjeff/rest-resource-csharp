using SlySoft.RestResource.Client.Extensions;

namespace SlySoft.RestResource.Client.ResourceDeserializers;

internal class HalJsonDeserializer : IResourceDeserializer {
    public bool CanDeserialize(HttpResponseMessage response) {
        var contentType = response.GetContentType();
        if (!contentType.StartsWith("application", StringComparison.CurrentCultureIgnoreCase)) {
            return false;
        }

        return contentType.Contains("json", StringComparison.CurrentCultureIgnoreCase);
    }

    public ClientResource Deserialize(HttpResponseMessage response) {
        try {
            var content = response.GetContent();
            return new ClientResource().FromJson(content);
        } catch {
            return new ClientResource();
        }
    }
}
