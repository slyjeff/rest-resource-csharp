using System.Text.Json.Serialization;

namespace SlySoft.RestResource;

/// <summary>
/// Represents a REST resource that adds links and embedded resources to existing data
/// </summary>
public class Resource {
    /// <summary>
    /// Links for this resource
    /// </summary>
    [JsonIgnore]
    public IList<Link> Links { get; } = new List<Link>();
}