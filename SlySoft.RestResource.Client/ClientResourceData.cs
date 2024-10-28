namespace SlySoft.RestResource.Client;

public sealed class ObjectData : Dictionary<string, object?> {
}

public sealed class ListData : List<ObjectData> {
}

public sealed class ResourceListData : List<ClientResource> {
}