﻿using System;
using System.Collections.Generic;
using System.Reflection;
using SlySoft.RestResource.Client.Generators;

namespace SlySoft.RestResource.Client;

/// <summary>
/// Create a runtime class that implements an interface to access a resource. The interface does not have to match the resource completely, but [LinkCheck] properties should be used
/// before executing links to make sure that they exist. Properties that do not exist in the resource will return default values.
/// </summary>
public static class ResourceAccessorFactory {
    private static readonly Dictionary<Type, Type> CreatedTypes = new();

    /// <summary>
    /// Create a runtime class that implements an interface to access a resource. The interface does not have to match the resource completely, but [LinkCheck] properties should be used
    /// before executing links to make sure that they exist. Properties that do not exist in the resource will return default values.
    /// </summary>
    /// <typeparam name="T">Interface to implement when creating the access class</typeparam>
    /// <param name="clientResource">Resource that will be accessed</param>
    /// <param name="restClient">RestClient so that links can be called</param>
    /// <returns>An object that implements T interface to access properties of the passed in resource</returns>
    public static T CreateAccessor<T>(ClientResource clientResource, IRestClient restClient) {
        var typeToCreate = typeof(T);

        Type accessorType;

        lock (CreatedTypes) {
            if (!CreatedTypes.ContainsKey(typeToCreate)) {
                var factory = new ResourceAccessorGenerator<T>();
                CreatedTypes[typeToCreate] = factory.GeneratedType();
            }

            accessorType = CreatedTypes[typeToCreate];
        }

        if (Activator.CreateInstance(accessorType, clientResource, restClient) is not T accessor) {
            throw new CreateAccessorException($"Create Instance for type {accessorType.Name} returned a null.");
        }

        return accessor;
    }

    private static MethodInfo? _createAccessorMethod;

    internal static object CreateAccessor(Type interfaceType, ClientResource clientResource, IRestClient restClient) {
        if (_createAccessorMethod == null) {
            _createAccessorMethod = typeof(ResourceAccessorFactory).GetMethod("CreateAccessor", BindingFlags.Public | BindingFlags.Static);
        }

        if (_createAccessorMethod == null) {
            throw new CreateAccessorException("Method 'CreateAccessor' not found in ResourceAccessorFactory.");
        }

        var genericCreateAccessorMethod = _createAccessorMethod.MakeGenericMethod(interfaceType);
        if (genericCreateAccessorMethod == null) {
            throw new CreateAccessorException($"'CreateAccessor' could not be called with generic type of '{interfaceType.Name}'.");
        }

        return genericCreateAccessorMethod.Invoke(null, new object[] { clientResource, restClient })!;
    }
}