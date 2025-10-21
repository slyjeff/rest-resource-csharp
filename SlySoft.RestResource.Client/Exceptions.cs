using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using SlySoft.RestResource.Client.Extensions;

namespace SlySoft.RestResource.Client; 

public abstract class RestResourceClientException : Exception {
    protected RestResourceClientException(string message) : base(message) {
    }
    protected RestResourceClientException(string message, Exception innerException) : base(message, innerException) {
    }
}


/// <summary>
/// An error thrown when creating an accessor fails
/// </summary>
public sealed class CreateAccessorException : RestResourceClientException {
    internal CreateAccessorException(string message) : base(message) {
    }

    internal CreateAccessorException(string message, Exception innerException) : base(message, innerException) {
    }
}

/// <summary>
/// An error thrown when a REST call fails
/// </summary>
public sealed class RestCallException : RestResourceClientException {
    internal RestCallException(string message) : base(message) {
    }

    internal RestCallException(string message, Exception innerException) : base(message, innerException) {
    }
}

/// <summary>
/// An error thrown when calling a link fails
/// </summary>
public sealed class CallLinkException : RestResourceClientException {
    internal CallLinkException(string message) : base(message) {
    }

    internal CallLinkException(string message, Exception innerException) : base(message, innerException) {
    }
}


public sealed class ResponseErrorCodeException : RestResourceClientException {
    public ResponseErrorCodeException(HttpResponseMessage response) : base(response.GetContent().RemoveOuterQuotes()) {
        StatusCode = response.StatusCode;
    }

    public HttpStatusCode StatusCode { get; }

    public T? Content<T>() {
        if (string.IsNullOrWhiteSpace(Message)) return default;
        if (typeof(T) == typeof(string)) return (T)(object)Message;
        try {
            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (!targetType.IsEnum)
                return System.Text.Json.JsonSerializer.Deserialize<T>(Message, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var trimmed = Message.Trim().Trim('"');
            var enumObj = long.TryParse(trimmed, out var numeric) 
                ? Enum.ToObject(targetType, numeric) 
                : Enum.Parse(targetType, trimmed, ignoreCase: true);
            return (T)enumObj;
        } catch (Exception ex) {
            throw new RestCallException($"Failed to deserialize response content to {typeof(T).Name}.", ex);
        }    
    }
}