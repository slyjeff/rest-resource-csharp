using SlySoft.RestResource.Client.Accessors;

namespace ClientTest;

public interface IApplicationResource {
    string Information { get; }
    Task<ITestsResource> GetTests();

    Task<IListResource> GetList();
}

public interface ITestsResource : IResourceAccessor {
    string Description { get; }
    Task<string> NotFound();
    Task<string> Text();
    Task<IQueryResultResource> Query(string parameter1, string parameter2);
    Task<IPostResultResource> Post(string parameter1, string parameter2);
    Task<IListResultResource> List(IList<string> list);
    Task<IDateTimeResource> DateTime(DateTime value);
}

public interface IDateTimeResource {
    public DateTime Value { get; set; }
}

public interface IQueryResultResource {
    string Parameter1 { get; }
    string Parameter2 { get; }
}

public interface IPostResultResource {
    string Parameter1 { get; }
    string Parameter2 { get; }
}

public interface IListResultResource {
    public IList<string> List { get; }
}

public interface IListResource : IResourceAccessor {
    IList<IListItem> Items { get; set; }
}

public interface IListItem {
    string Id { get; set; }
    Task<string> Self();
}