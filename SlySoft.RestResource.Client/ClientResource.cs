namespace SlySoft.RestResource.Client;

/// <summary>
/// Represents a REST resource that can contain data, links, and embedded resource
/// </summary>
public sealed class ClientResource {
    /// <summary>
    /// URI of the resource that will be used to construct a "self" link
    /// </summary>
    public string Uri { get; set; } = string.Empty;

    /// <summary>
    /// Data values contained by this resource
    /// </summary>
    public ObjectData Data { get; } = new();

    /// <summary>
    /// Links for this resource
    /// </summary>
    public IList<Link> Links { get; } = new List<Link>();
}