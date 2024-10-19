/*namespace SlySoft.RestResource.Client.Tests;

public static class TestData {
    public static Resource CreateUserListResource(params string[] lastNames) {
        var userResourceList = lastNames.Select(lastName => new Resource().Data("lastName", lastName)).ToList();
        return new Resource().Embedded("users", userResourceList);
    }
}*/