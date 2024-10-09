using Microsoft.AspNetCore.Mvc;
using SlySoft.RestResource;

namespace WebTest.Controllers;

[Route("")]
public sealed class ApplicationController  : ControllerBase {
    private class ApiResource : Resource {
        public string Information { get; set; } = "This is a test web service for demonstrating how to use Slysoft.RestResource and related libraries.";
        public string Hateoas { get; set; } = "Hypermedia as the engine of application state (HATEOAS) is essentially embedding links in resources so endpoints are browseable.";
        public string Hal { get; set; } = "This library is based on HAL, bot the json and xml variations, but should be flexible enough to use any format.";
        public string AcceptHeader { get; set; } = "Change how this data is retrieved by changing the accept header in the request: text/html, application/json, application/xml.";
        public string GetTests { get; set; } = "/test";
    }
    
    [HttpGet]
    public IActionResult GetApplication() {
        var resource = new ApiResource()
            .Get("self", "/");

        return StatusCode(200, resource);
    }
}