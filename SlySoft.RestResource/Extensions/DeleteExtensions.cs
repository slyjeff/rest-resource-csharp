﻿using SlySoft.RestResource.Utils;

// ReSharper disable once CheckNamespace
namespace SlySoft.RestResource;

public static class DeleteExtensions {
    /// <summary>
    /// Create a DELETE link
    /// </summary>
    /// <param name="resource">The DELETE link will be added to this resource</param>
    /// <param name="name">Name of the element- will be converted to camelcase</param>
    /// <param name="href">HREF of the link</param>
    /// <param name="templated">Whether or not the URI is templated</param>
    /// <returns>The resource so further calls can be chained</returns>
    public static Resource Delete(this Resource resource, string name, string href, bool templated = false) {
        resource.Links.Add(new Link(name.ToCamelCase(), href, verb: "DELETE", templated: templated));
        return resource;
    }
}