using SlySoft.RestResource.Client.Extensions;

namespace SlySoft.RestResource.Client.Accessors;

public abstract class ObjectDataAccessor(ObjectData objectData, IRestClient restClient) : Accessor(restClient) {
    protected override T? CreateData<T>(string name) where T : default {
        return objectData.GetData<T>(name, RestClient);
    }
}