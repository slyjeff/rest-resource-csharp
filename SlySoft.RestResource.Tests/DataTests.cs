using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlySoft.RestResource.Tests;

[TestClass]
public class DataTests {
    private class TestObject {
        public int Id { get; set; }
        public string LastName { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public int Age { get; init; }
    }
    
    private class TestResource(TestObject testData, LinkSetup? linkSetup = null) : Resource(testData, linkSetup) {
        public int Id { get; set; }
        public string LastName { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public int Age { get; init; }
    }
    
    [TestMethod]
    public void ResourceMustAutomapProperties() {
        //arrange
        var testObject = new TestObject {
            LastName = "Blow",
            FirstName = "Joe",
            Age = 42,
        };

        //act
        var resource = new TestResource(testObject);

        //assert
        Assert.AreEqual(testObject.Id, resource.Id);
        Assert.AreEqual(testObject.LastName, resource.LastName);
        Assert.AreEqual(testObject.FirstName, resource.FirstName);
        Assert.AreEqual(testObject.Age, resource.Age);
    }
    
    [TestMethod]
    public void ResourceMustSetLinksOnCreate() {
        //arrange
        var testObject = new TestObject {
            Id       = 14,
            LastName = "Blow",
            FirstName = "Joe",
            Age = 42,
        };

        //act
        var resource = new TestResource(testObject, x => {
            if (x is not TestResource resource) {
                return;
            }

            x.Get("self", $"test/{resource.Id}");
        });

        //assert
        Assert.AreEqual(testObject.Id, resource.Id);
        Assert.AreEqual(testObject.LastName, resource.LastName);
        Assert.AreEqual(testObject.FirstName, resource.FirstName);
        Assert.AreEqual(testObject.Age, resource.Age);
    }    
}