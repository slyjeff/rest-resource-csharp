using System.Collections;
using System.Reflection;
using SlySoft.RestResource.Client.Accessors;

namespace SlySoft.RestResource.Client.Extensions;

internal static class AccessorExtensions {
    internal static T? GetData<T>(this ClientResource clientResource, string name, IRestClient restClient) {
        return clientResource.Data.GetData<T>(name);
    }

    internal static T? GetData<T>(this ObjectData objectData, string name) {
        foreach (var data in objectData) {
            if (data.Key.Equals(name, StringComparison.CurrentCultureIgnoreCase)) {
                return (T?)ParseValue(data.Value, typeof(T));
            }
        }

        return default;
    }

    private static IList CreateListOfType(Type type) {
        var listType = typeof(List<>);
        var genericListType = listType.MakeGenericType(type);
        if (Activator.CreateInstance(genericListType) is not IList list) {
            throw new CreateAccessorException($"Error creating list of type {type.Name}");
        }

        return list;
    }

    private static object CreateEditableAccessorList(Type type, object sourceList) {
        var listType = typeof(EditableAccessorList<>);
        var genericListType = listType.MakeGenericType(type);
        return Activator.CreateInstance(genericListType, sourceList)
               ?? throw new CreateAccessorException($"Error creating editable accessor list of type {type.Name}");
    }


    private static object? ParseValue(object? value, Type type) {
        if (value == null) {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        if (value.GetType() == type) {
            return value;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>) && value is IEnumerable enumerable) {
            return ParseList(enumerable, type);
        }

        if (type.IsInterface && value is ObjectData objectData) {
            return ObjectDataAccessorFactory.CreateAccessor(type, objectData);
        }

        if (type.IsClass && value is ObjectData classObjectData) {
            return ParseObject(classObjectData, type);
        }

        if (type == typeof(string)) {
            return value;
        }

        if (type.IsEnum) {
            return Enum.Parse(type, value.ToString() ?? string.Empty);
        }

        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null) {
            return string.IsNullOrEmpty(value.ToString()) ? null : ParseValue(value, underlyingType);
        }

        var parseMethod = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string) }, null);
        if (parseMethod != null) {
            return parseMethod.Invoke(null, new object[] { value.ToString() ?? string.Empty });
        }

        return default;
    }

    private static object ParseList(IEnumerable enumerable, Type type) {
        var genericArgumentType = type.GetGenericArguments()[0];
        var list = CreateListOfType(genericArgumentType);
        foreach (var item in enumerable) {
            list.Add(ParseValue(item, genericArgumentType));
        }

        return CreateEditableAccessorList(genericArgumentType, list);
    }

    private static object? ParseObject(ObjectData objectData, Type type) {
        var newObject = Activator.CreateInstance(type);
        foreach (var property in type.GetProperties()) {
            foreach (var data in objectData) {
                if (data.Key.Equals(property.Name, StringComparison.CurrentCultureIgnoreCase)) {
                    property.SetValue(newObject, ParseValue(data.Value, property.PropertyType));
                    break;
                }
            }
        }

        return newObject;
    }
}
