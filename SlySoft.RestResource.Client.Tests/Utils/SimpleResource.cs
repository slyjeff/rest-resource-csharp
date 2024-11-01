using SlySoft.RestResource.Client.Accessors;
using TestUtils;

namespace SlySoft.RestResource.Client.Tests.Utils;

public enum OptionEnum { Option1, Option2 };

public interface ISimpleResource : IEditableAccessor {
    string Message { get; set; }
    int Number { get; }
    OptionEnum Option { get; }
    bool? IsOptional { get; }
    DateTime Date { get;  }
    DateOnly DateOnly { get; }
    TimeOnly TimeOnly { get; }
    IList<string> Strings { get; }
    IList<int> Numbers { get; }
    ChildResource Child { get; }
    IChildResource ChildInterface { get; }
    IList<ChildResource> Children { get; }
    IList<IChildResource> ChildInterfaces { get; }
}

public sealed class SimpleResource : Resource {
    public SimpleResource() {
        Child = new ChildResource();
        Child.Get("self", "/child/1");

        ChildInterface = new ChildResource();
        ChildInterface.Get("self", "/child/1");

        Children =  new List<ChildResource> { new(), new(), new() };
        Children[0].Get("self", "/child/1");
        Children[1].Get("self", "/child/2");
        Children[2].Get("self", "/child/3");

        ChildInterfaces =  new List<ChildResource> { new(), new(), new() };
        ChildInterfaces[0].Get("self", "/child/1");
        ChildInterfaces[1].Get("self", "/child/2");
        ChildInterfaces[2].Get("self", "/child/3");
    }

    public string Message { get; set; } = GenerateRandom.String();
    public int Number { get; set; } = GenerateRandom.Int();
    public OptionEnum Option { get; set; } = OptionEnum.Option2;
    public bool? IsOptional { get; set; } = true;
    public DateTime Date { get; set; } = DateTime.Now;
    public DateOnly DateOnly { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public TimeOnly TimeOnly { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
    public IList<string> Strings { get; } = new List<string> { GenerateRandom.String(), GenerateRandom.String(), GenerateRandom.String() };
    public IList<int> Numbers { get; } = new List<int> { GenerateRandom.Int(), GenerateRandom.Int(), GenerateRandom.Int() };
    public ChildResource Child { get; }
    public ChildResource ChildInterface { get; }
    public IList<ChildResource> Children { get; }
    public IList<ChildResource> ChildInterfaces { get; }
}

public interface IChildResource {
    string ChildMessage { get; set; }
    int ChildNumber { get; }
}

public sealed class ChildResource : Resource {
    public string ChildMessage { get; set; } = GenerateRandom.String();
    public int ChildNumber { get; set; } = GenerateRandom.Int();
}
