using Microsoft.AspNetCore.Mvc;
using SlySoft.RestResource;
using System.Text.Json.Serialization;

namespace WebTest.Controllers;

[Route("[controller]")]
public sealed class ListController : ControllerBase {
    private sealed class ListResource : Resource {
        public ListResource(IEnumerable<ListItemResource> items) {
            Items = items;
        }
        public IEnumerable<ListItemResource> Items { get; }
    }

    private sealed class ListItemResource : Resource {
        public ListItemResource(int id) {
            Id = id.ToString();
            this.Get("self", $"/list/{id}");
        }

        public string Id { get; set; }
    }


    [HttpGet]
    public IActionResult GetList() {
        var listItems = new List<ListItemResource>();
        for (var x = 1; x < 10; ++x) {
            listItems.Add(new ListItemResource(x));
        }

        var resource = new ListResource(listItems);

        return StatusCode(200, resource);
    }
}
