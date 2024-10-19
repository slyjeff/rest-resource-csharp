using Microsoft.AspNetCore.Mvc;
using SlySoft.RestResource;
using System.Text.Json.Serialization;

namespace WebTest.Controllers;

[Route("[controller]")]
public sealed class TestController  : ControllerBase {
    private class TestResource : Resource {
        public string Description { get; } = "Tests used by the ClientTest app.";
    }
    
    [HttpGet]
    public IActionResult GetTests() {
        var resource = new TestResource()
            .Get("notFound", "/test/notFound")
            .Get("text", "/test/text")
            .Get("templatedGet", "test/templated/{value1}/{value2}", templated: true)
            .Query("query", "test/query")
                .Parameter("parameter1")
                .Parameter("parameter2")
            .EndQuery()
            .Post("post", "test/post")
                .Field("parameter1")
                .Field("parameter2")
            .EndBody()
            .Delete("delete", "test/delete")
            .Put("list", "test/list")
                .Field("list")
            .EndBody()
            .QueryWithAllParameters<BodyWithBool>("queryBool", "test/queryBool")
            .Post<BodyWithBool>("bool", "test/bool")
                .Field(x => x.Value, defaultValue: false)
            .EndBody()
            .PostWithAllFields<BodyWithInt>("int", "test/int")
            .PostWithAllFields<BodyWithEnum>("enum", "test/enum");

        return StatusCode(200, resource);
    }

    [HttpGet("notFound")]
    public IActionResult NotFoundTest() {
        return StatusCode(404, "Resource not found.");
    }

    [HttpGet("text")]
    public IActionResult Text() {
        return StatusCode(200, "Non-Resource text.");
    }

    private class TemplatedGetResource : Resource {
        public string Value1 { get; set; } = "";
        public string Value2 { get; set; } = "";
    }
    
    [HttpGet("templated/{value1}/{value2}")]
    public IActionResult TemplatedGet(string value1, string value2) {
        var resource = new TemplatedGetResource {
            Value1 = value1,
            Value2 = value2
        };

        return StatusCode(200, resource);
    }

    private class QueryResource : Resource {
        public string Parameter1 { get; set; } = "";
        public string Parameter2 { get; set; } = "";
    }    
    
    [HttpGet("query")]
    public IActionResult Query([FromQuery] string parameter1, [FromQuery] string parameter2) {
        var resource = new QueryResource{
            Parameter1 = parameter1,
            Parameter2 = parameter2
        };

        return StatusCode(200, resource);
    }

    public class PostBody {
        public string Parameter1 { get; set; } = string.Empty;
        public string Parameter2 { get; set; } = string.Empty;
    }

    private class PostResource : Resource {
        public string Parameter1 { get; set; } = "";
        public string Parameter2 { get; set; } = "";
    }      
    
    [HttpPost("post")]
    public IActionResult Post([FromBody] PostBody body) {
        var resource = new PostResource{
            Parameter1 = body.Parameter1,
            Parameter2 = body.Parameter2
        };

        return StatusCode(200, resource);
    }

    [HttpDelete("delete")]
    public IActionResult Delete() {
        return StatusCode(200);
    }

    public class ListBody {
        public IList<string> List { get; set; } = new List<string>();
    }

    private class ListResource : Resource {
        public IList<string> List { get; set; } = new List<string>();
    }
    
    [HttpPut("list")]
    public IActionResult List([FromBody] ListBody body) {
        var resource = new ListResource {
            List = body.List
        };

        return StatusCode(200, resource);
    }

    public class BodyWithBool {
        [JsonConverter(typeof(JsonConverter<bool>))]
        public bool Value { get; set; }
    }

    private class BoolResource : Resource {
        public bool Bool { get; set; }
    }
    
    [HttpGet("queryBool")]
    public IActionResult BoolTest([FromQuery] bool value) {
        var resource = new BoolResource {
            Bool = value
        };
        
        return StatusCode(200, resource);
    }

    [HttpPost("bool")]
    public IActionResult BoolTest([FromBody] BodyWithBool? body) {
        if (body == null) {
            return StatusCode(500, "Could not parse value");
        }

        var resource = new BoolResource {
            Bool = body.Value
        };
        
        return StatusCode(200, resource);
    }

    public class BodyWithInt {
        public int Value { get; set; } = 0;
    }

    private class IntResource : Resource {
        public int Int { get; set; }
    }
    
    [HttpPost("int")]
    public IActionResult IntTest([FromBody] BodyWithInt? body) {
        if (body == null) {
            return StatusCode(500, "Could not parse value");
        }

        var resource = new IntResource {
            Int = body.Value
        };

        return StatusCode(200, resource);
    }

    public enum BodyEnum { Value1, Value2 }
    public class BodyWithEnum {
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public BodyEnum Value { get; set; } = BodyEnum.Value1;
    }
    
    private class EnumResource : Resource {
        public BodyEnum Enum { get; set; } = BodyEnum.Value1;
    }


    [HttpPost("enum")]
    public IActionResult EnumTest([FromBody] BodyWithEnum? body) {
        if (body == null) {
            return StatusCode(500, "Could not parse value");
        }

        var resource = new EnumResource {
            Enum = body.Value
        };

        return StatusCode(200, resource);
    }
}