using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using SlySoft.RestResource.Serializers;

namespace SlySoft.RestResource.AspNetCoreUtils; 

public sealed class ResourceSlysoftJsonFormatter : TextOutputFormatter {
    public ResourceSlysoftJsonFormatter() {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(new StringSegment("application/slysoft+json")));

        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override bool CanWriteType(Type type) {
        return typeof(Resource).IsAssignableFrom(type) && base.CanWriteType(type);
    }

    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding) {
        var response = context.HttpContext.Response;

        context.HttpContext.Response.Headers["Content-Type"] ="application/slysoft+json";

        var resource = context.Object as Resource;
        return response.WriteAsync(resource?.ToSlySoftHalJson());
    }
}