using System.Text.Json.Serialization;
using SlySoft.RestResource.Utils;

namespace SlySoft.RestResource;

public delegate void LinkSetup(Resource resource);

/// <summary>
/// Represents a REST resource that adds links and embedded resources to existing data
/// </summary>
public class Resource {
    public Resource(object? sourceData = null, LinkSetup? linkSetup = null) {
        if (sourceData != null) {
            var destinationProperties = GetType().GetAllProperties().ToList();

            var type = sourceData.GetType();
            foreach (var sourceProperty in type.GetAllProperties()) {
                var destinationProperty = destinationProperties.FirstOrDefault(x => x.Name == sourceProperty.Name && x.PropertyType == sourceProperty.PropertyType && x.CanWrite);
                if (destinationProperty == null) {
                    continue;
                }
                
                destinationProperty.SetValue(this, sourceProperty.GetValue(sourceData));
            }
        }

        linkSetup?.Invoke(this);
    }
    
    /// <summary>
    /// Links for this resource
    /// </summary>
    [JsonIgnore]
    public IList<Link> Links { get; } = new List<Link>();
}